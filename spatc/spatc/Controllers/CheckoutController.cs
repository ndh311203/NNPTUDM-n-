using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;
using spatc.Services;

namespace spatc.Controllers
{
    [Route("Checkout")]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly VietQRService _vietQRService;
        private readonly NotificationService _notificationService;

        public CheckoutController(
            ApplicationDbContext context, 
            VietQRService vietQRService,
            NotificationService notificationService)
        {
            _context = context;
            _vietQRService = vietQRService;
            _notificationService = notificationService;
        }

        // GET: Checkout
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            var qrInfo = _vietQRService.GetVietQRInfo();
            ViewBag.VietQRInfo = qrInfo;
            return View();
        }

        // POST: Checkout/PlaceOrder
        [HttpPost]
        [Route("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] CheckoutRequest request)
        {
            try
            {
                if (request.Items == null || !request.Items.Any())
                {
                    return Json(new { success = false, message = "Giỏ hàng trống" });
                }

                // Tạo đơn hàng
                var order = new ShopOrder
                {
                    CustomerName = request.CustomerName,
                    Phone = request.Phone,
                    Email = request.Email,
                    Address = request.Address,
                    Note = request.Note,
                    PaymentMethod = request.PaymentMethod,
                    Status = "Chờ xác nhận",
                    TotalAmount = 0, // Sẽ tính sau
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Tính tổng tiền và tạo order items
                decimal totalAmount = 0;
                var orderItems = new List<ShopOrderItem>();

                foreach (var item in request.Items)
                {
                    // Kiểm tra sản phẩm và lấy giá
                    decimal price = 0;
                    string productName = "";

                    switch (item.ProductType.ToLower())
                    {
                        case "pet":
                            var pet = await _context.PetProducts.FirstOrDefaultAsync(p => p.Id == item.ProductId && p.TrangThai);
                            if (pet == null)
                            {
                                return Json(new { success = false, message = $"Thú cưng ID {item.ProductId} không tồn tại hoặc đã ngừng bán" });
                            }
                            price = pet.Price;
                            productName = pet.Name;
                            if (item.Quantity != 1)
                            {
                                return Json(new { success = false, message = "Thú cưng chỉ có thể mua số lượng 1" });
                            }
                            // Tăng số lượt đã bán
                            pet.SoldCount += item.Quantity;
                            break;

                        case "food":
                            var food = await _context.FoodProducts.FirstOrDefaultAsync(f => f.Id == item.ProductId && f.TrangThai);
                            if (food == null)
                            {
                                return Json(new { success = false, message = $"Thức ăn ID {item.ProductId} không tồn tại hoặc đã ngừng bán" });
                            }
                            if (food.SoLuongTon < item.Quantity)
                            {
                                return Json(new { success = false, message = $"Thức ăn '{food.Name}' chỉ còn {food.SoLuongTon} sản phẩm" });
                            }
                            price = food.Price;
                            productName = food.Name;
                            // Cập nhật số lượng tồn kho và số lượt đã bán
                            food.SoLuongTon -= item.Quantity;
                            food.SoldCount += item.Quantity;
                            break;

                        case "accessory":
                            var accessory = await _context.AccessoryProducts.FirstOrDefaultAsync(a => a.Id == item.ProductId && a.TrangThai);
                            if (accessory == null)
                            {
                                return Json(new { success = false, message = $"Phụ kiện ID {item.ProductId} không tồn tại hoặc đã ngừng bán" });
                            }
                            if (accessory.SoLuongTon < item.Quantity)
                            {
                                return Json(new { success = false, message = $"Phụ kiện '{accessory.Name}' chỉ còn {accessory.SoLuongTon} sản phẩm" });
                            }
                            price = accessory.Price;
                            productName = accessory.Name;
                            // Cập nhật số lượng tồn kho và số lượt đã bán
                            accessory.SoLuongTon -= item.Quantity;
                            accessory.SoldCount += item.Quantity;
                            break;

                        default:
                            return Json(new { success = false, message = $"Loại sản phẩm không hợp lệ: {item.ProductType}" });
                    }

                    var orderItem = new ShopOrderItem
                    {
                        ProductId = item.ProductId,
                        ProductType = item.ProductType,
                        ProductName = productName,
                        Quantity = item.Quantity,
                        Price = price,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    orderItems.Add(orderItem);
                    totalAmount += price * item.Quantity;
                }

                // Áp dụng voucher nếu có
                decimal discountAmount = 0;
                if (!string.IsNullOrEmpty(request.VoucherCode))
                {
                    var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => 
                        v.Code == request.VoucherCode && 
                        v.Status == "active" &&
                        v.StartDate <= DateTime.Now &&
                        v.EndDate >= DateTime.Now);

                    if (voucher != null && totalAmount >= voucher.MinOrderAmount)
                    {
                        if (voucher.Type == "percent")
                        {
                            discountAmount = totalAmount * voucher.Value / 100;
                            if (voucher.MaxDiscount.HasValue && discountAmount > voucher.MaxDiscount.Value)
                            {
                                discountAmount = voucher.MaxDiscount.Value;
                            }
                        }
                        else if (voucher.Type == "cash")
                        {
                            discountAmount = voucher.Value;
                            if (discountAmount > totalAmount)
                            {
                                discountAmount = totalAmount;
                            }
                        }
                        order.VoucherCode = voucher.Code;
                    }
                }

                order.DiscountAmount = discountAmount;
                order.TotalAmount = totalAmount - discountAmount;
                order.ShopOrderItems = orderItems;

                // Xử lý phương thức thanh toán
                if (request.PaymentMethod == "VietQR")
                {
                    order.PaymentStatus = "waiting_payment";
                    order.PaymentMethod = "VietQR";
                }
                else if (request.PaymentMethod == "BankTransfer")
                {
                    order.PaymentStatus = "waiting_payment";
                }
                else
                {
                    order.PaymentStatus = null; // COD không cần trạng thái thanh toán
                }

                // Lưu đơn hàng
                _context.ShopOrders.Add(order);
                await _context.SaveChangesAsync();

                // Tạo thông báo cho admin
                try
                {
                    System.Diagnostics.Debug.WriteLine($"=== BẮT ĐẦU TẠO THÔNG BÁO ĐƠN HÀNG ===");
                    System.Diagnostics.Debug.WriteLine($"Order ID: {order.Id}, Customer: {order.CustomerName}, Total: {order.TotalAmount}");
                    
                    await _notificationService.TaoThongBaoDonHangMoiAsync(order);
                    
                    System.Diagnostics.Debug.WriteLine("✅ Đã tạo thông báo đơn hàng thành công");
                }
                catch (Exception notifEx)
                {
                    // Log lỗi nhưng không làm gián đoạn flow đặt hàng
                    System.Diagnostics.Debug.WriteLine($"❌ Lỗi khi tạo thông báo đơn hàng: {notifEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"StackTrace: {notifEx.StackTrace}");
                }

                // Tạo QR code nếu là VietQR
                string? qrCodeDataUrl = null;
                string? transferContent = null;
                if (request.PaymentMethod == "VietQR")
                {
                    transferContent = $"Order_{order.Id}";
                    qrCodeDataUrl = _vietQRService.GetQRCodeDataUrl(order.TotalAmount, transferContent);
                }

                return Json(new { 
                    success = true, 
                    orderId = order.Id, 
                    message = "Đặt hàng thành công!",
                    paymentMethod = request.PaymentMethod,
                    qrCode = qrCodeDataUrl,
                    transferContent = transferContent,
                    totalAmount = order.TotalAmount,
                    discountAmount = discountAmount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // POST: Checkout/ValidateVoucher
        [HttpPost]
        [Route("ValidateVoucher")]
        public async Task<IActionResult> ValidateVoucher([FromBody] VoucherValidateRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.VoucherCode))
                {
                    return Json(new { success = false, message = "Vui lòng nhập mã voucher" });
                }

                var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == request.VoucherCode);

                if (voucher == null)
                {
                    return Json(new { success = false, message = "Mã voucher không tồn tại" });
                }

                if (voucher.Status != "active")
                {
                    return Json(new { success = false, message = "Mã voucher đã bị vô hiệu hóa" });
                }

                if (DateTime.Now < voucher.StartDate)
                {
                    return Json(new { success = false, message = "Mã voucher chưa có hiệu lực" });
                }

                if (DateTime.Now > voucher.EndDate)
                {
                    return Json(new { success = false, message = "Mã voucher đã hết hạn" });
                }

                if (request.TotalAmount < voucher.MinOrderAmount)
                {
                    return Json(new { 
                        success = false, 
                        message = $"Đơn hàng tối thiểu {voucher.MinOrderAmount:N0} đ để sử dụng voucher này" 
                    });
                }

                // Tính số tiền giảm
                decimal discountAmount = 0;
                if (voucher.Type == "percent")
                {
                    discountAmount = request.TotalAmount * voucher.Value / 100;
                    if (voucher.MaxDiscount.HasValue && discountAmount > voucher.MaxDiscount.Value)
                    {
                        discountAmount = voucher.MaxDiscount.Value;
                    }
                }
                else if (voucher.Type == "cash")
                {
                    discountAmount = voucher.Value;
                    if (discountAmount > request.TotalAmount)
                    {
                        discountAmount = request.TotalAmount;
                    }
                }

                return Json(new { 
                    success = true, 
                    discountAmount = discountAmount,
                    finalAmount = request.TotalAmount - discountAmount,
                    voucherType = voucher.Type,
                    voucherValue = voucher.Value
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }

    // DTOs
    public class CheckoutRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string PaymentMethod { get; set; } = "COD";
        public string? VoucherCode { get; set; }
        public List<CheckoutItem> Items { get; set; } = new();
    }

    public class CheckoutItem
    {
        public int ProductId { get; set; }
        public string ProductType { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public class VoucherValidateRequest
    {
        public string VoucherCode { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
