using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize]
    [Route("Admin/[controller]")]
    public class ShopOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ShopOrder
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(string? status)
        {
            var query = _context.ShopOrders
                .Include(o => o.ShopOrderItems)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            var orders = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
            
            ViewBag.StatusFilter = status;
            ViewBag.Statuses = new[] { "Chờ xác nhận", "Đang giao", "Hoàn thành", "Hủy" };

            return View("~/Views/Admin/ShopOrder/Index.cshtml", orders);
        }

        // GET: Admin/ShopOrder/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.ShopOrders
                .Include(o => o.ShopOrderItems)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/ShopOrder/Details.cshtml", order);
        }

        // POST: Admin/ShopOrder/UpdateStatus
        [HttpPost]
        [Route("UpdateStatus")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            try
            {
                if (request == null || request.OrderId <= 0 || string.IsNullOrEmpty(request.Status))
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                var order = await _context.ShopOrders.FindAsync(request.OrderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Đơn hàng không tồn tại" });
                }

                var validStatuses = new[] { "Chờ xác nhận", "Đang giao", "Hoàn thành", "Hủy" };
                if (!validStatuses.Contains(request.Status))
                {
                    return Json(new { success = false, message = "Trạng thái không hợp lệ" });
                }

                order.Status = request.Status;
                order.UpdatedAt = DateTime.Now;

                // Nếu đơn hàng hoàn thành, tự động cập nhật trạng thái thanh toán thành Paid
                if (request.Status == "Hoàn thành" && !string.IsNullOrEmpty(order.PaymentStatus))
                {
                    order.PaymentStatus = "Paid";
                }

                _context.Update(order);
                await _context.SaveChangesAsync();
                
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        public class UpdateStatusRequest
        {
            public int OrderId { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        // POST: Admin/ShopOrder/UpdatePaymentStatus
        [HttpPost]
        [Route("UpdatePaymentStatus")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdatePaymentStatus([FromBody] UpdatePaymentStatusRequest request)
        {
            try
            {
                if (request == null || request.OrderId <= 0 || string.IsNullOrEmpty(request.PaymentStatus))
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                var order = await _context.ShopOrders.FindAsync(request.OrderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Đơn hàng không tồn tại" });
                }

                var validPaymentStatuses = new[] { "waiting_payment", "Paid", "Canceled", "Failed" };
                if (!validPaymentStatuses.Contains(request.PaymentStatus))
                {
                    return Json(new { success = false, message = "Trạng thái thanh toán không hợp lệ" });
                }

                order.PaymentStatus = request.PaymentStatus;
                order.UpdatedAt = DateTime.Now;

                _context.Update(order);
                await _context.SaveChangesAsync();
                
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        public class UpdatePaymentStatusRequest
        {
            public int OrderId { get; set; }
            public string PaymentStatus { get; set; } = string.Empty;
        }
    }
}

