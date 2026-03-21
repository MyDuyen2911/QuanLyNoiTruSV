using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize]
    public class PhongTaiSansController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public PhongTaiSansController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH (Tìm theo tên phòng hoặc tên tài sản)
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.PhongTaiSans
                .Include(p => p.MaPhongNavigation)
                .Include(p => p.MaTaiSanNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.MaPhongNavigation.TenPhong.Contains(search) ||
                                         p.MaTaiSanNavigation.TenTaiSan.Contains(search));
            }

            ViewBag.CurrentSearch = search;
            return View(await query.ToListAsync());
        }

        // 2. CHI TIẾT
        public async Task<IActionResult> Details(int? maPhong, int? maTaiSan)
        {
            if (maPhong == null || maTaiSan == null) return NotFound();

            var phongTaiSan = await _context.PhongTaiSans
                .Include(p => p.MaPhongNavigation)
                .Include(p => p.MaTaiSanNavigation)
                .FirstOrDefaultAsync(m => m.MaPhong == maPhong && m.MaTaiSan == maTaiSan);

            if (phongTaiSan == null) return NotFound();
            return View(phongTaiSan);
        }

        // 3. THÊM MỚI (Phân bổ tài sản vào phòng)
        [Authorize(Roles = "Admin,NhanVien")]
        public IActionResult Create()
        {
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong");
            ViewData["MaTaiSan"] = new SelectList(_context.TaiSans, "MaTaiSan", "TenTaiSan");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Create(PhongTaiSan phongTaiSan)
        {
            // Kiểm tra xem phòng đó đã có tài sản này chưa
            var exists = await _context.PhongTaiSans.AnyAsync(p => p.MaPhong == phongTaiSan.MaPhong && p.MaTaiSan == phongTaiSan.MaTaiSan);
            if (exists)
            {
                TempData["error"] = "Tài sản này đã được phân bổ trong phòng. Hãy dùng chức năng Sửa để cập nhật số lượng!";
                ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", phongTaiSan.MaPhong);
                ViewData["MaTaiSan"] = new SelectList(_context.TaiSans, "MaTaiSan", "TenTaiSan", phongTaiSan.MaTaiSan);
                return View(phongTaiSan);
            }

            // THÊM 2 DÒNG NÀY ĐỂ BỎ QUA LỖI KIỂM TRA ẢO 👇
            ModelState.Remove("MaPhongNavigation");
            ModelState.Remove("MaTaiSanNavigation");

            if (ModelState.IsValid)
            {
                _context.Add(phongTaiSan);
                await _context.SaveChangesAsync();
                TempData["success"] = "Phân bổ tài sản vào phòng thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Nếu vẫn lỗi thì nạp lại list
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", phongTaiSan.MaPhong);
            ViewData["MaTaiSan"] = new SelectList(_context.TaiSans, "MaTaiSan", "TenTaiSan", phongTaiSan.MaTaiSan);
            return View(phongTaiSan);
        }

        // 4. SỬA (Chỉ sửa Số Lượng và Tình Trạng)
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int? maPhong, int? maTaiSan)
        {
            if (maPhong == null || maTaiSan == null) return NotFound();

            var phongTaiSan = await _context.PhongTaiSans.FindAsync(maPhong, maTaiSan);
            if (phongTaiSan == null) return NotFound();

            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", phongTaiSan.MaPhong);
            ViewData["MaTaiSan"] = new SelectList(_context.TaiSans, "MaTaiSan", "TenTaiSan", phongTaiSan.MaTaiSan);
            return View(phongTaiSan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int maPhong, int maTaiSan, PhongTaiSan phongTaiSan)
        {
            if (maPhong != phongTaiSan.MaPhong || maTaiSan != phongTaiSan.MaTaiSan) return NotFound();

            // THÊM 2 DÒNG NÀY VÀO ĐÂY NỮA 👇
            ModelState.Remove("MaPhongNavigation");
            ModelState.Remove("MaTaiSanNavigation");

            if (ModelState.IsValid)
            {
                _context.Update(phongTaiSan);
                await _context.SaveChangesAsync();
                TempData["success"] = "Cập nhật tình trạng tài sản thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", phongTaiSan.MaPhong);
            ViewData["MaTaiSan"] = new SelectList(_context.TaiSans, "MaTaiSan", "TenTaiSan", phongTaiSan.MaTaiSan);
            return View(phongTaiSan);
        }

        // 5. XÓA (Thu hồi tài sản khỏi phòng)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? maPhong, int? maTaiSan)
        {
            if (maPhong == null || maTaiSan == null) return NotFound();

            var phongTaiSan = await _context.PhongTaiSans
                .Include(p => p.MaPhongNavigation)
                .Include(p => p.MaTaiSanNavigation)
                .FirstOrDefaultAsync(m => m.MaPhong == maPhong && m.MaTaiSan == maTaiSan);

            if (phongTaiSan == null) return NotFound();
            return View(phongTaiSan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int maPhong, int maTaiSan)
        {
            var phongTaiSan = await _context.PhongTaiSans.FindAsync(maPhong, maTaiSan);
            if (phongTaiSan != null)
            {
                _context.PhongTaiSans.Remove(phongTaiSan);
                await _context.SaveChangesAsync();
                TempData["success"] = "Đã thu hồi tài sản khỏi phòng!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}