using Microsoft.AspNetCore.Mvc;

namespace spatc.Controllers
{
    public class LienHeController : Controller
    {
        public IActionResult Index()
        {
            // Thông tin liên hệ có thể lấy từ config hoặc database
            ViewBag.DiaChi = "123 Đường ABC, Quận XYZ, TP.HCM";
            ViewBag.Hotline = "0901234567";
            ViewBag.Zalo = "0901234567";
            ViewBag.Facebook = "https://facebook.com/spathucung";
            ViewBag.GioLamViec = "Thứ 2 - Chủ nhật: 8:00 - 20:00";
            ViewBag.Email = "info@spathucung.com";
            
            return View();
        }
    }
}

