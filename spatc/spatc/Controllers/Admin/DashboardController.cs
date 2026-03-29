using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize]
    [Route("Admin/[controller]")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Dashboard
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            // Tổng doanh thu
            var todayRevenue = await _context.ShopOrders
                .Where(o => o.CreatedAt.Date == today && 
                           (o.Status == "Hoàn thành" || o.PaymentStatus == "Paid"))
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            var monthRevenue = await _context.ShopOrders
                .Where(o => o.CreatedAt >= thisMonth && 
                           (o.Status == "Hoàn thành" || o.PaymentStatus == "Paid"))
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            var totalRevenue = await _context.ShopOrders
                .Where(o => o.Status == "Hoàn thành" || o.PaymentStatus == "Paid")
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            ViewBag.TodayRevenue = todayRevenue;
            ViewBag.MonthRevenue = monthRevenue;
            ViewBag.TotalRevenue = totalRevenue;

            // Đơn hàng gần nhất
            var recentOrders = await _context.ShopOrders
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .ToListAsync();

            ViewBag.RecentOrders = recentOrders;

            return View();
        }

        // API: Lấy dữ liệu doanh thu 7 ngày gần nhất
        [HttpGet]
        [Route("GetRevenueChart")]
        public async Task<IActionResult> GetRevenueChart()
        {
            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-6);

            var revenues = await _context.ShopOrders
                .Where(o => o.CreatedAt.Date >= startDate && 
                           o.CreatedAt.Date <= endDate &&
                           (o.Status == "Hoàn thành" || o.PaymentStatus == "Paid"))
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new
                {
                    date = g.Key.ToString("dd/MM"),
                    revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.date)
                .ToListAsync();

            // Fill missing dates with 0
            var result = new List<object>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var existing = revenues.FirstOrDefault(r => r.date == date.ToString("dd/MM"));
                result.Add(new
                {
                    date = date.ToString("dd/MM"),
                    revenue = existing?.revenue ?? 0
                });
            }

            return Json(result);
        }

        // API: Top 5 sản phẩm bán chạy
        [HttpGet]
        [Route("GetTopProducts")]
        public async Task<IActionResult> GetTopProducts()
        {
            var topProducts = await (from oi in _context.ShopOrderItems
                                    join o in _context.ShopOrders on oi.OrderId equals o.Id
                                    where o.Status == "Hoàn thành" || o.PaymentStatus == "Paid"
                                    group oi by new { oi.ProductName, oi.ProductType } into g
                                    select new
                                    {
                                        name = g.Key.ProductName ?? "Không tên",
                                        type = g.Key.ProductType ?? "N/A",
                                        quantity = g.Sum(oi => oi.Quantity)
                                    })
                                    .OrderByDescending(x => x.quantity)
                                    .Take(5)
                                    .ToListAsync();

            return Json(topProducts);
        }
    }
}

