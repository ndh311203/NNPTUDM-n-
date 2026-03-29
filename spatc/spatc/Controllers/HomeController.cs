using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy dịch vụ nổi bật (trạng thái = true)
            var dichVuNoiBat = await _context.DichVus
                .Where(d => d.TrangThai == true)
                .OrderByDescending(d => d.NgayTao)
                .Take(6)
                .ToListAsync();

            // Lấy sản phẩm bán chạy (top 6)
            var bestSellingProducts = new List<ProductDto>();
            
            var topPets = await _context.PetProducts
                .Where(p => p.TrangThai == true)
                .OrderByDescending(p => p.SoldCount)
                .Take(2)
                .Select(p => new ProductDto { 
                    Id = p.Id, 
                    Name = p.Name, 
                    Price = p.Price, 
                    OldPrice = p.OldPrice,
                    Image = p.Images != null ? p.Images.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() : null,
                    Rating = p.AverageRating,
                    SoldCount = p.SoldCount,
                    Type = "pet"
                })
                .ToListAsync();
            bestSellingProducts.AddRange(topPets);

            var topFoods = await _context.FoodProducts
                .Where(f => f.TrangThai == true)
                .OrderByDescending(f => f.SoldCount)
                .Take(2)
                .Select(f => new ProductDto { 
                    Id = f.Id, 
                    Name = f.Name, 
                    Price = f.Price, 
                    OldPrice = f.OldPrice,
                    Image = f.Image,
                    Rating = f.AverageRating,
                    SoldCount = f.SoldCount,
                    Type = "food"
                })
                .ToListAsync();
            bestSellingProducts.AddRange(topFoods);

            var topAccessories = await _context.AccessoryProducts
                .Where(a => a.TrangThai == true)
                .OrderByDescending(a => a.SoldCount)
                .Take(2)
                .Select(a => new ProductDto { 
                    Id = a.Id, 
                    Name = a.Name, 
                    Price = a.Price, 
                    OldPrice = a.OldPrice,
                    Image = a.Image,
                    Rating = a.AverageRating,
                    SoldCount = a.SoldCount,
                    Type = "accessory"
                })
                .ToListAsync();
            bestSellingProducts.AddRange(topAccessories);

            // Sắp xếp lại theo số lượt bán
            bestSellingProducts = bestSellingProducts
                .OrderByDescending(p => p.SoldCount)
                .Take(6)
                .ToList();

            // Lấy voucher đang hoạt động
            var activeVouchers = await _context.Vouchers
                .Where(v => v.Status == "active" && 
                            v.StartDate <= DateTime.Now && 
                            v.EndDate >= DateTime.Now)
                .OrderByDescending(v => v.CreatedAt)
                .Take(3)
                .ToListAsync();

            // Lấy reviews gần nhất (top 6)
            var recentReviews = await _context.ProductReviews
                .Where(r => r.Status == "approved")
                .OrderByDescending(r => r.CreatedAt)
                .Take(6)
                .ToListAsync();

            ViewBag.DichVuNoiBat = dichVuNoiBat;
            ViewBag.BestSellingProducts = bestSellingProducts;
            ViewBag.ActiveVouchers = activeVouchers;
            ViewBag.RecentReviews = recentReviews;
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Action để kiểm tra kết nối database
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                // Kiểm tra kết nối database
                var canConnect = await _context.Database.CanConnectAsync();
                
                if (canConnect)
                {
                    var connection = _context.Database.GetDbConnection();
                    var connectionString = _context.Database.GetConnectionString();
                    
                    // Mở connection để lấy thông tin
                    if (connection.State != System.Data.ConnectionState.Open)
                    {
                        await connection.OpenAsync();
                    }
                    
                    try
                    {
                        ViewBag.Message = "✅ Kết nối database thành công!";
                        ViewBag.ConnectionString = connectionString;
                        ViewBag.ServerVersion = connection.ServerVersion;
                        ViewBag.DatabaseName = connection.Database;
                        ViewBag.DataSource = connection.DataSource;
                    }
                    finally
                    {
                        // Đóng connection sau khi lấy thông tin
                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            await connection.CloseAsync();
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "❌ Không thể kết nối đến database!";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"❌ Lỗi kết nối database: {ex.Message}";
                ViewBag.ErrorDetails = ex.ToString();
            }

            return View();
        }
    }
}
