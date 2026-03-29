using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize(Roles = "Admin,NhanVien")]
    [Route("Admin/[controller]")]
    public class DichVuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DichVuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/DichVu
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            // Kiểm tra quyền
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (userRole != "Admin" && userRole != "NhanVien")
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này!";
                return RedirectToAction("Index", "Home");
            }

            var allServices = await _context.DichVus.OrderByDescending(d => d.NgayTao).ToListAsync();
            
            var viewModel = new DichVuIndexViewModel
            {
                Grooming = allServices.Where(x => x.LoaiDichVu == "Grooming").ToList(),
                KhachSan = allServices.Where(x => x.LoaiDichVu == "KhachSan").ToList()
            };
            
            return View("~/Views/Admin/DichVu/Index.cshtml", viewModel);
        }

        // GET: Admin/DichVu/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dichVu = await _context.DichVus.FirstOrDefaultAsync(m => m.Id == id);
            if (dichVu == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/DichVu/Details.cshtml", dichVu);
        }

        // GET: Admin/DichVu/Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/DichVu/Create.cshtml");
        }

        // POST: Admin/DichVu/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenDichVu,MoTa,Gia,HinhAnh,TrangThai,LoaiDichVu,ThoiGianThucHien,KichThuocThuCung,LoaiDichVuGrooming,SucChua,LoaiPhong,BaoGomBuaAn,DichVuBoSung,ThoiGianNhanTra")] DichVu dichVu)
        {
            if (ModelState.IsValid)
            {
                dichVu.NgayTao = DateTime.Now;
                dichVu.TrangThai = true; // Đảm bảo mặc định hiển thị ở trang công khai
                _context.Add(dichVu);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm dịch vụ thành công! Dịch vụ sẽ hiển thị ở trang công khai.";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/DichVu/Create.cshtml", dichVu);
        }

        // GET: Admin/DichVu/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dichVu = await _context.DichVus.FindAsync(id);
            if (dichVu == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/DichVu/Edit.cshtml", dichVu);
        }

        // POST: Admin/DichVu/Edit/5
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenDichVu,MoTa,Gia,HinhAnh,TrangThai,NgayTao,LoaiDichVu,ThoiGianThucHien,KichThuocThuCung,LoaiDichVuGrooming,SucChua,LoaiPhong,BaoGomBuaAn,DichVuBoSung,ThoiGianNhanTra")] DichVu dichVu)
        {
            if (id != dichVu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dichVu);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật dịch vụ thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DichVuExists(dichVu.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/DichVu/Edit.cshtml", dichVu);
        }

        // GET: Admin/DichVu/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dichVu = await _context.DichVus.FirstOrDefaultAsync(m => m.Id == id);
            if (dichVu == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/DichVu/Delete.cshtml", dichVu);
        }

        // POST: Admin/DichVu/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dichVu = await _context.DichVus.FindAsync(id);
            if (dichVu != null)
            {
                _context.DichVus.Remove(dichVu);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa dịch vụ thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DichVuExists(int id)
        {
            return _context.DichVus.Any(e => e.Id == id);
        }
    }
}

