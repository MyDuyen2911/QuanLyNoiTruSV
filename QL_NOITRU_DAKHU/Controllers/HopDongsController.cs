using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;
using System.Security.Claims;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize]
    public class HopDongsController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public HopDongsController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH HỢP ĐỒNG
        public async Task<IActionResult> Index(string search)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdClaim);

            var query = _context.HopDongs
                .Include(h => h.MaPhongNavigation)
                .Include(h => h.MaSvNavigation)
                .AsQueryable();

            // Sinh viên chỉ xem được hợp đồng của chính mình
            if (User.IsInRole("SinhVien"))
            {
                query = query.Where(h => h.MaSvNavigation.MaTaiKhoan == userId);
            }

            // Tìm kiếm theo tên SV hoặc tên phòng
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(h => h.MaSvNavigation.HoTen.Contains(search) || h.MaPhongNavigation.TenPhong.Contains(search));
            }

            ViewBag.CurrentSearch = search;
            return View(await query.OrderByDescending(h => h.NgayBatDau).ToListAsync());
        }

        // 2. CHI TIẾT HỢP ĐỒNG
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var hopDong = await _context.HopDongs
                .Include(h => h.MaPhongNavigation)
                .Include(h => h.MaSvNavigation)
                .FirstOrDefaultAsync(m => m.MaHopDong == id);

            if (hopDong == null) return NotFound();
            return View(hopDong);
        }

        // 3. THÊM MỚI (Chỉ Admin/NhanVien)
        [Authorize(Roles = "Admin,NhanVien")]
        public IActionResult Create()
        {
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong");
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "HoTen");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Create(HopDong hopDong)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hopDong);
                await _context.SaveChangesAsync();
                TempData["success"] = "Tạo hợp đồng thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", hopDong.MaPhong);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "HoTen", hopDong.MaSv);
            return View(hopDong);
        }

        // 4. CHỈNH SỬA (Chỉ Admin/NhanVien)
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong == null) return NotFound();

            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", hopDong.MaPhong);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "HoTen", hopDong.MaSv);
            return View(hopDong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int id, HopDong hopDong)
        {
            if (id != hopDong.MaHopDong) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(hopDong);
                await _context.SaveChangesAsync();
                TempData["success"] = "Cập nhật hợp đồng thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", hopDong.MaPhong);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "HoTen", hopDong.MaSv);
            return View(hopDong);
        }

        // 5. XÓA (Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var hopDong = await _context.HopDongs
                .Include(h => h.MaPhongNavigation)
                .Include(h => h.MaSvNavigation)
                .FirstOrDefaultAsync(m => m.MaHopDong == id);

            if (hopDong == null) return NotFound();
            return View(hopDong);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong != null)
            {
                _context.HopDongs.Remove(hopDong);
                await _context.SaveChangesAsync();
                TempData["success"] = "Đã xóa hợp đồng!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}