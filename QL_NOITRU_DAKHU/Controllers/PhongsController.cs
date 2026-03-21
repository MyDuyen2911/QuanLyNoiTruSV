using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize(Roles = "Admin,NhanVien,SinhVien")]
    public class PhongsController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public PhongsController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH PHÒNG
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Phongs
                .Include(p => p.MaLoaiPhongNavigation)
                .Include(p => p.MaToaNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.TenPhong.Contains(search) || p.MaToaNavigation.TenToa.Contains(search));
            }

            ViewBag.CurrentSearch = search;
            return View(await query.ToListAsync());
        }

        // 2. CHI TIẾT PHÒNG
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var phong = await _context.Phongs
                .Include(p => p.MaToaNavigation)
                .Include(p => p.MaLoaiPhongNavigation)
                // KÉO THÊM DANH SÁCH HỢP ĐỒNG ĐANG Ở VÀ THÔNG TIN SINH VIÊN
                .Include(p => p.HopDongs.Where(h => h.TrangThai == "Đang ở"))
                    .ThenInclude(h => h.MaSvNavigation)
                .FirstOrDefaultAsync(m => m.MaPhong == id);

            if (phong == null) return NotFound();
            return View(phong);
        }

        // 3. THÊM MỚI
        [Authorize(Roles = "Admin,NhanVien")]
        public IActionResult Create()
        {
            ViewData["MaToa"] = new SelectList(_context.ToaNhas, "MaToa", "TenToa");
            ViewData["MaLoaiPhong"] = new SelectList(_context.LoaiPhongs, "MaLoaiPhong", "TenLoai");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Create(Phong phong)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phong);
                await _context.SaveChangesAsync();
                TempData["success"] = "Thêm phòng thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaToa"] = new SelectList(_context.ToaNhas, "MaToa", "TenToa", phong.MaToa);
            ViewData["MaLoaiPhong"] = new SelectList(_context.LoaiPhongs, "MaLoaiPhong", "TenLoai", phong.MaLoaiPhong);
            return View(phong);
        }

        // 4. SỬA
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var phong = await _context.Phongs.FindAsync(id);
            if (phong == null) return NotFound();

            ViewData["MaToa"] = new SelectList(_context.ToaNhas, "MaToa", "TenToa", phong.MaToa);
            ViewData["MaLoaiPhong"] = new SelectList(_context.LoaiPhongs, "MaLoaiPhong", "TenLoai", phong.MaLoaiPhong);
            return View(phong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int id, Phong phong)
        {
            if (id != phong.MaPhong) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(phong);
                await _context.SaveChangesAsync();
                TempData["success"] = "Cập nhật phòng thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaToa"] = new SelectList(_context.ToaNhas, "MaToa", "TenToa", phong.MaToa);
            ViewData["MaLoaiPhong"] = new SelectList(_context.LoaiPhongs, "MaLoaiPhong", "TenLoai", phong.MaLoaiPhong);
            return View(phong);
        }

        // 5. XÓA PHÒNG (Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var phong = await _context.Phongs
                .Include(p => p.MaToaNavigation)
                .Include(p => p.MaLoaiPhongNavigation)
                .FirstOrDefaultAsync(m => m.MaPhong == id);

            if (phong == null) return NotFound();
            return View(phong);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong != null)
            {
                try
                {
                    _context.Phongs.Remove(phong);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Đã xóa phòng thành công!";
                }
                catch (Exception)
                {
                    TempData["error"] = "Không thể xóa! Phòng này đang chứa dữ liệu liên quan (Hợp đồng, Sinh viên, Đơn đăng ký...). Vui lòng xóa dữ liệu liên quan trước!";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}