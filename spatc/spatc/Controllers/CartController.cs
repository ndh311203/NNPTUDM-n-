using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;
using System.Text.Json;

namespace spatc.Controllers
{
    [Route("Cart")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cart
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        // API: Lấy thông tin sản phẩm để hiển thị trong giỏ hàng
        [HttpPost]
        [Route("GetProductInfo")]
        public async Task<IActionResult> GetProductInfo([FromBody] CartItemRequest request)
        {
            try
            {
                object? product = null;
                string? image = null;

                switch (request.ProductType.ToLower())
                {
                    case "pet":
                        var pet = await _context.PetProducts.FirstOrDefaultAsync(p => p.Id == request.ProductId && p.TrangThai);
                        if (pet != null)
                        {
                            product = new
                            {
                                id = pet.Id,
                                name = pet.Name,
                                price = pet.Price,
                                type = "pet"
                            };
                            // Lấy ảnh đầu tiên
                            if (!string.IsNullOrEmpty(pet.Images))
                            {
                                var images = pet.Images.Split(',', StringSplitOptions.RemoveEmptyEntries);
                                if (images.Length > 0)
                                {
                                    image = images[0].Trim();
                                }
                            }
                        }
                        break;

                    case "food":
                        var food = await _context.FoodProducts.FirstOrDefaultAsync(f => f.Id == request.ProductId && f.TrangThai);
                        if (food != null)
                        {
                            product = new
                            {
                                id = food.Id,
                                name = food.Name,
                                price = food.Price,
                                type = "food",
                                stock = food.SoLuongTon
                            };
                            image = food.Image;
                        }
                        break;

                    case "accessory":
                        var accessory = await _context.AccessoryProducts.FirstOrDefaultAsync(a => a.Id == request.ProductId && a.TrangThai);
                        if (accessory != null)
                        {
                            product = new
                            {
                                id = accessory.Id,
                                name = accessory.Name,
                                price = accessory.Price,
                                type = "accessory",
                                stock = accessory.SoLuongTon
                            };
                            image = accessory.Image;
                        }
                        break;
                }

                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại hoặc đã ngừng bán" });
                }

                return Json(new { success = true, product = product, image = image });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Kiểm tra số lượng tồn kho
        [HttpPost]
        [Route("CheckStock")]
        public async Task<IActionResult> CheckStock([FromBody] StockCheckRequest request)
        {
            try
            {
                int availableStock = 0;

                switch (request.ProductType.ToLower())
                {
                    case "pet":
                        var pet = await _context.PetProducts.FirstOrDefaultAsync(p => p.Id == request.ProductId && p.TrangThai);
                        availableStock = pet != null ? 1 : 0; // Thú cưng chỉ có 1
                        break;

                    case "food":
                        var food = await _context.FoodProducts.FirstOrDefaultAsync(f => f.Id == request.ProductId && f.TrangThai);
                        availableStock = food?.SoLuongTon ?? 0;
                        break;

                    case "accessory":
                        var accessory = await _context.AccessoryProducts.FirstOrDefaultAsync(a => a.Id == request.ProductId && a.TrangThai);
                        availableStock = accessory?.SoLuongTon ?? 0;
                        break;
                }

                return Json(new { success = true, stock = availableStock });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    // DTOs
    public class CartItemRequest
    {
        public int ProductId { get; set; }
        public string ProductType { get; set; } = string.Empty;
    }

    public class StockCheckRequest
    {
        public int ProductId { get; set; }
        public string ProductType { get; set; } = string.Empty;
    }
}

