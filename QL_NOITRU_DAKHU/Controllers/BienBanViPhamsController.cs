using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;
using System.Security.Claims;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize]
    public class BienBanViPhamsController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public BienBanViPhamsController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH BIÊN BẢN
        public async Task<IActionResult> Index(string search)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdClaim);

            var query = _context.BienBanViPhams
                .Include(b => b.MaSvNavigation)
                .Include(b => b.MaViPhamNavigation)
                .AsQueryable();

            // Nếu là Sinh viên -> Chỉ hiện biên bản của sinh viên đó
            if (User.IsInRole("SinhVien"))
            {
                query = query.Where(b => b.MaSvNavigation.MaTaiKhoan == userId);
            }

            // Tìm kiếm theo tên SV hoặc tên lỗi
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.MaSvNavigation.HoTen.Contains(search)
                                      || b.MaViPhamNavigation.TenViPham.Contains(search));
            }

            ViewBag.CurrentSearch = search;
            return View(await query.OrderByDescending(b => b.NgayLap).ToListAsync());
        }

        // 2. CHI TIẾT
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var bienBan = await _context.BienBanViPhams
                .Include(b => b.MaSvNavigation)
                .Include(b => b.MaViPhamNavigation)
                .FirstOrDefaultAsync(m => m.MaBienBan == id);

            if (bienBan == null) return NotFound();
            return View(bienBan);
        }

        // 3. THÊM MỚI (Chỉ Admin, NV)
        [Authorize(Roles = "Admin,NhanVien")]
        public IActionResult Create()
        {
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "HoTen");
            ViewData["MaViPham"] = new SelectList(_context.ViPhams, "MaViPham", "TenViPham");

            // Tự động gán ngày lập là hôm nay cho tiện
            var newBienBan = new BienBanViPham { NgayLap = DateOnly.FromDateTime(DateTime.Now) };
            return View(newBienBan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Create(BienBanViPham bienBan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bienBan);
                await _context.SaveChangesAsync();
                TempData["success"] = "Lập biên bản vi phạm thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "HoTen", bienBan.MaSv);
            ViewData["MaViPham"] = new SelectList(_context.ViPhams, "MaViPham", "TenViPham", bienBan.MaViPham);
            return View(bienBan);
        }

        // 4. SỬA (Chỉ Admin, NV)
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var bienBan = await _context.BienBanViPhams.FindAsync(id);
            if (bienBan == null) return NotFound();

            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "HoTen", bienBan.MaSv);
            ViewData["MaViPham"] = new SelectList(_context.ViPhams, "MaViPham", "TenViPham", bienBan.MaViPham);
            return View(bienBan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int id, BienBanViPham bienBan)
        {
            if (id != bienBan.MaBienBan) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(bienBan);
                await _context.SaveChangesAsync();
                TempData["success"] = "Cập nhật biên bản thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "HoTen", bienBan.MaSv);
            ViewData["MaViPham"] = new SelectList(_context.ViPhams, "MaViPham", "TenViPham", bienBan.MaViPham);
            return View(bienBan);
        }

        // 5. XÓA (Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bienBan = await _context.BienBanViPhams
                .Include(b => b.MaSvNavigation)
                .Include(b => b.MaViPhamNavigation)
                .FirstOrDefaultAsync(m => m.MaBienBan == id);

            if (bienBan == null) return NotFound();
            return View(bienBan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bienBan = await _context.BienBanViPhams.FindAsync(id);
            if (bienBan != null)
            {
                _context.BienBanViPhams.Remove(bienBan);
                await _context.SaveChangesAsync();
                TempData["success"] = "Đã xóa biên bản vi phạm!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}