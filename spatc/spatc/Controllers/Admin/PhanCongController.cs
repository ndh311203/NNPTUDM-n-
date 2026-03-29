using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Antiforgery;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize(Roles = "Admin,NhanVien")]
    [Route("Admin/[controller]")]
    public class PhanCongController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhanCongController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/PhanCong
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(int? month, int? year, int? nhanVienId)
        {
            var currentDate = DateTime.Now;
            var selectedMonth = month ?? currentDate.Month;
            var selectedYear = year ?? currentDate.Year;
            var selectedNhanVienId = nhanVienId;

            // Lấy danh sách nhân viên
            var nhanViens = await _context.NhanViens
                .Where(n => n.TrangThai == true)
                .OrderBy(n => n.HoTen)
                .ToListAsync();

            // Lấy danh sách công việc
            var congViecs = await _context.CongViecs
                .OrderBy(c => c.TenCongViec)
                .ToListAsync();

            // Lấy lịch phân công theo tháng
            var startDate = new DateTime(selectedYear, selectedMonth, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var query = _context.LichPhanCongs
                .Include(l => l.NhanVien)
                .Include(l => l.CongViec)
                .Where(l => l.NgayLam >= startDate && l.NgayLam <= endDate)
                .AsQueryable();

            if (selectedNhanVienId.HasValue)
            {
                query = query.Where(l => l.NhanVienId == selectedNhanVienId.Value);
            }

            var lichPhanCongs = await query.ToListAsync();

            // Tạo view model cho grid
            var viewModel = new List<PhanCongGridViewModel>();

            foreach (var nv in nhanViens)
            {
                var nvPhanCongs = lichPhanCongs.Where(l => l.NhanVienId == nv.Id).ToList();
                var gridRow = new PhanCongGridViewModel
                {
                    NhanVienId = nv.Id,
                    TenNhanVien = nv.HoTen,
                    PhanCongs = new Dictionary<DateTime, LichPhanCong>()
                };

                // Tạo dictionary với tất cả các ngày trong tháng
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var phanCong = nvPhanCongs.FirstOrDefault(p => p.NgayLam.Date == date.Date);
                    gridRow.PhanCongs[date] = phanCong;
                }

                viewModel.Add(gridRow);
            }

            ViewBag.NhanViens = new SelectList(nhanViens, "Id", "HoTen", selectedNhanVienId);
            ViewBag.CongViecs = congViecs;
            ViewBag.SelectedMonth = selectedMonth;
            ViewBag.SelectedYear = selectedYear;
            ViewBag.SelectedNhanVienId = selectedNhanVienId;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.DaysInMonth = DateTime.DaysInMonth(selectedYear, selectedMonth);

            return View("~/Views/Admin/PhanCong/Index.cshtml", viewModel);
        }

        // GET: Admin/PhanCong/Create
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create(DateTime? ngayLam, int? nhanVienId)
        {
            var nhanViens = await _context.NhanViens
                .Where(n => n.TrangThai == true)
                .OrderBy(n => n.HoTen)
                .ToListAsync();

            var congViecs = await _context.CongViecs
                .OrderBy(c => c.TenCongViec)
                .ToListAsync();

            ViewBag.NhanVienId = new SelectList(nhanViens, "Id", "HoTen", nhanVienId);
            ViewBag.CongViecId = new SelectList(congViecs, "Id", "TenCongViec");
            ViewBag.NgayLam = ngayLam ?? DateTime.Now;

            return View("~/Views/Admin/PhanCong/Create.cshtml");
        }

        // POST: Admin/PhanCong/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NhanVienId,CongViecId,NgayLam,CaLam,TrangThai,GhiChu")] LichPhanCong lichPhanCong)
        {
            if (ModelState.IsValid)
            {
                lichPhanCong.NgayTao = DateTime.Now;
                if (string.IsNullOrEmpty(lichPhanCong.TrangThai))
                {
                    lichPhanCong.TrangThai = "Chưa làm";
                }

                _context.Add(lichPhanCong);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm phân công thành công!";
                return RedirectToAction(nameof(Index), new { 
                    month = lichPhanCong.NgayLam.Month, 
                    year = lichPhanCong.NgayLam.Year,
                    nhanVienId = lichPhanCong.NhanVienId
                });
            }

            var nhanViens = await _context.NhanViens
                .Where(n => n.TrangThai == true)
                .OrderBy(n => n.HoTen)
                .ToListAsync();

            var congViecs = await _context.CongViecs
                .OrderBy(c => c.TenCongViec)
                .ToListAsync();

            ViewBag.NhanVienId = new SelectList(nhanViens, "Id", "HoTen", lichPhanCong.NhanVienId);
            ViewBag.CongViecId = new SelectList(congViecs, "Id", "TenCongViec", lichPhanCong.CongViecId);

            return View("~/Views/Admin/PhanCong/Create.cshtml", lichPhanCong);
        }

        // GET: Admin/PhanCong/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichPhanCong = await _context.LichPhanCongs.FindAsync(id);
            if (lichPhanCong == null)
            {
                return NotFound();
            }

            var nhanViens = await _context.NhanViens
                .Where(n => n.TrangThai == true)
                .OrderBy(n => n.HoTen)
                .ToListAsync();

            var congViecs = await _context.CongViecs
                .OrderBy(c => c.TenCongViec)
                .ToListAsync();

            ViewBag.NhanVienId = new SelectList(nhanViens, "Id", "HoTen", lichPhanCong.NhanVienId);
            ViewBag.CongViecId = new SelectList(congViecs, "Id", "TenCongViec", lichPhanCong.CongViecId);

            return View("~/Views/Admin/PhanCong/Edit.cshtml", lichPhanCong);
        }

        // POST: Admin/PhanCong/Edit/5
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NhanVienId,CongViecId,NgayLam,CaLam,TrangThai,GhiChu,NgayTao")] LichPhanCong lichPhanCong)
        {
            if (id != lichPhanCong.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichPhanCong);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật phân công thành công!";
                    return RedirectToAction(nameof(Index), new { 
                        month = lichPhanCong.NgayLam.Month, 
                        year = lichPhanCong.NgayLam.Year,
                        nhanVienId = lichPhanCong.NhanVienId
                    });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichPhanCongExists(lichPhanCong.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            var nhanViens = await _context.NhanViens
                .Where(n => n.TrangThai == true)
                .OrderBy(n => n.HoTen)
                .ToListAsync();

            var congViecs = await _context.CongViecs
                .OrderBy(c => c.TenCongViec)
                .ToListAsync();

            ViewBag.NhanVienId = new SelectList(nhanViens, "Id", "HoTen", lichPhanCong.NhanVienId);
            ViewBag.CongViecId = new SelectList(congViecs, "Id", "TenCongViec", lichPhanCong.CongViecId);

            return View("~/Views/Admin/PhanCong/Edit.cshtml", lichPhanCong);
        }

        // POST: Admin/PhanCong/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lichPhanCong = await _context.LichPhanCongs.FindAsync(id);
            if (lichPhanCong != null)
            {
                var month = lichPhanCong.NgayLam.Month;
                var year = lichPhanCong.NgayLam.Year;
                var nhanVienId = lichPhanCong.NhanVienId;

                _context.LichPhanCongs.Remove(lichPhanCong);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa phân công thành công!";
                return RedirectToAction(nameof(Index), new { month, year, nhanVienId });
            }

            return RedirectToAction(nameof(Index));
        }

        // API: Copy lịch tuần này sang tuần sau
        [HttpPost]
        [Route("CopyWeek")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CopyWeek([FromBody] CopyWeekRequest request)
        {
            try
            {
                var fromStart = request.FromDate.Date;
                var fromEnd = fromStart.AddDays(6);
                var toStart = request.ToDate.Date;
                var daysDiff = (toStart - fromStart).Days;

                var sourcePhanCongs = await _context.LichPhanCongs
                    .Where(l => l.NgayLam >= fromStart && l.NgayLam <= fromEnd)
                    .ToListAsync();

                var newPhanCongs = new List<LichPhanCong>();

                foreach (var source in sourcePhanCongs)
                {
                    var newPhanCong = new LichPhanCong
                    {
                        NhanVienId = source.NhanVienId,
                        CongViecId = source.CongViecId,
                        NgayLam = source.NgayLam.AddDays(daysDiff),
                        CaLam = source.CaLam,
                        TrangThai = "Chưa làm",
                        GhiChu = source.GhiChu,
                        NgayTao = DateTime.Now
                    };
                    newPhanCongs.Add(newPhanCong);
                }

                _context.LichPhanCongs.AddRange(newPhanCongs);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"Đã copy {newPhanCongs.Count} phân công thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Lấy lịch theo tháng (AJAX)
        [HttpGet]
        [Route("GetByMonth")]
        public async Task<IActionResult> GetByMonth(int month, int year, int? nhanVienId)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var query = _context.LichPhanCongs
                .Include(l => l.NhanVien)
                .Include(l => l.CongViec)
                .Where(l => l.NgayLam >= startDate && l.NgayLam <= endDate)
                .AsQueryable();

            if (nhanVienId.HasValue)
            {
                query = query.Where(l => l.NhanVienId == nhanVienId.Value);
            }

            var lichPhanCongs = await query
                .Select(l => new
                {
                    id = l.Id,
                    nhanVienId = l.NhanVienId,
                    tenNhanVien = l.NhanVien != null ? l.NhanVien.HoTen : "",
                    congViecId = l.CongViecId,
                    tenCongViec = l.CongViec != null ? l.CongViec.TenCongViec : "",
                    ngayLam = l.NgayLam,
                    caLam = l.CaLam,
                    trangThai = l.TrangThai,
                    ghiChu = l.GhiChu
                })
                .ToListAsync();

            return Json(new { success = true, data = lichPhanCongs });
        }

        // GET: Admin/PhanCong/ExportExcel
        [HttpGet]
        [Route("ExportExcel")]
        public async Task<IActionResult> ExportExcel(int month, int year, int? nhanVienId)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var query = _context.LichPhanCongs
                .Include(l => l.NhanVien)
                .Include(l => l.CongViec)
                .Where(l => l.NgayLam >= startDate && l.NgayLam <= endDate)
                .AsQueryable();

            if (nhanVienId.HasValue)
            {
                query = query.Where(l => l.NhanVienId == nhanVienId.Value);
            }

            var lichPhanCongs = await query
                .OrderBy(l => l.NhanVien.HoTen)
                .ThenBy(l => l.NgayLam)
                .ToListAsync();

            // Tạo CSV content
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Nhân viên,Công việc,Ngày làm,Ca làm,Trạng thái,Ghi chú");

            foreach (var pc in lichPhanCongs)
            {
                csv.AppendLine($"{pc.NhanVien?.HoTen},{pc.CongViec?.TenCongViec},{pc.NgayLam:dd/MM/yyyy},{pc.CaLam},{pc.TrangThai},\"{pc.GhiChu}\"");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"LichPhanCong_{month}_{year}.csv");
        }

        private bool LichPhanCongExists(int id)
        {
            return _context.LichPhanCongs.Any(e => e.Id == id);
        }
    }

    public class PhanCongGridViewModel
    {
        public int NhanVienId { get; set; }
        public string TenNhanVien { get; set; } = string.Empty;
        public Dictionary<DateTime, LichPhanCong?> PhanCongs { get; set; } = new();
    }

    public class CopyWeekRequest
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}

