using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers
{
    public class DichVuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DichVuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DichVu - Trang chính với 2 tab
        [HttpGet]
        [Route("DichVu")]
        [Route("DichVu/Index")]
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách dịch vụ để đảm bảo view có model
            var allServices = await _context.DichVus
                .Where(d => d.TrangThai == true)
                .ToListAsync();
            
            return View(allServices);
        }

        // GET: DichVu/Grooming - Trang dịch vụ Grooming
        [HttpGet]
        [Route("DichVu/Grooming")]
        public async Task<IActionResult> Grooming()
        {
            // Lấy dịch vụ từ cả 2 bảng: DichVuGroomings và DichVu (với LoaiDichVu = "Grooming")
            var groomingServicesFromGrooming = await _context.DichVuGroomings
                .Where(d => d.TrangThai == true)
                .OrderBy(d => d.Gia)
                .ToListAsync();

            var groomingServicesFromDichVu = await _context.DichVus
                .Where(d => d.TrangThai == true && d.LoaiDichVu == "Grooming")
                .OrderBy(d => d.Gia)
                .ToListAsync();

            // Lưu vào ViewBag để view có thể hiển thị cả 2 loại
            ViewBag.DichVuGroomings = groomingServicesFromGrooming;
            ViewBag.DichVus = groomingServicesFromDichVu;
            
            // Giữ nguyên để tương thích với view hiện tại (DichVuGroomings)
            return View(groomingServicesFromGrooming);
        }

        // GET: DichVu/KhachSan - Trang khách sạn thú cưng
        [HttpGet]
        [Route("DichVu/KhachSan")]
        public async Task<IActionResult> KhachSan()
        {
            // Lấy dịch vụ khách sạn từ bảng DichVus với LoaiDichVu = "KhachSan"
            var dichVuKhachSans = await _context.DichVus
                .Where(d => d.TrangThai == true && d.LoaiDichVu == "KhachSan")
                .OrderBy(d => d.Gia)
                .ToListAsync();

            ViewBag.DichVuKhachSans = dichVuKhachSans;
            
            return View(dichVuKhachSans);
        }

        // GET: DichVu/Details/5 - Chi tiết dịch vụ
        [HttpGet]
        [Route("DichVu/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Tìm trong cả 2 bảng: DichVus và DichVuGroomings
            var dichVu = await _context.DichVus
                .Where(d => d.Id == id && d.TrangThai == true)
                .FirstOrDefaultAsync();

            if (dichVu == null)
            {
                // Nếu không tìm thấy trong DichVus, tìm trong DichVuGroomings
                var groomingService = await _context.DichVuGroomings
                    .Where(d => d.Id == id && d.TrangThai == true)
                    .FirstOrDefaultAsync();

                if (groomingService == null)
                {
                    return NotFound();
                }

                // Chuyển đổi DichVuGrooming sang DichVu để hiển thị
                dichVu = new DichVu
                {
                    Id = groomingService.Id,
                    TenDichVu = groomingService.TenDichVu,
                    MoTa = groomingService.MoTa,
                    Gia = groomingService.Gia,
                    HinhAnh = groomingService.HinhAnh,
                    TrangThai = groomingService.TrangThai,
                    LoaiDichVu = "Grooming"
                };
            }

            // Lấy các dịch vụ khác cùng loại để gợi ý
            var dichVuKhac = await _context.DichVus
                .Where(d => d.TrangThai == true && 
                           d.LoaiDichVu == dichVu.LoaiDichVu && 
                           d.Id != id)
                .Take(5)
                .ToListAsync();

            ViewBag.DichVuKhac = dichVuKhac;
            ViewBag.ServiceId = id;
            return View(dichVu);
        }
    }
}

