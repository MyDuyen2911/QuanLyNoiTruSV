using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize(Roles = "Admin,NhanVien,SinhVien")]
    public class LoaiPhongsController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public LoaiPhongsController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH LOẠI PHÒNG
        // GET: LoaiPhongs
        public async Task<IActionResult> Index(string search)
        {
            // Lấy toàn bộ danh sách loại phòng
            var query = _context.LoaiPhongs.AsQueryable();

            // Nếu người dùng có nhập chữ vào ô tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                // Tìm theo Tên Loại (ví dụ: "VIP", "Phòng 8")
                query = query.Where(l => l.TenLoai.Contains(search));
            }

            ViewBag.CurrentSearch = search; // Gửi lại từ khóa ra View để hiện ở ô nhập
            return View(await query.ToListAsync());
        }

        // 2. CHI TIẾT
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var loaiPhong = await _context.LoaiPhongs
                .Include(l => l.Phongs)
                .FirstOrDefaultAsync(m => m.MaLoaiPhong == id);
            if (loaiPhong == null) return NotFound();
            return View(loaiPhong);
        }

        // 3. THÊM MỚI (Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoaiPhong lp)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lp);
                await _context.SaveChangesAsync();
                TempData["success"] = "Thêm loại phòng thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(lp);
        }

        // 4. SỬA (Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var lp = await _context.LoaiPhongs.FindAsync(id);
            if (lp == null) return NotFound();
            return View(lp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoaiPhong lp)
        {
            if (id != lp.MaLoaiPhong) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(lp);
                await _context.SaveChangesAsync();
                TempData["success"] = "Cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(lp);
        }

        // 5. XÓA (Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var lp = await _context.LoaiPhongs.FirstOrDefaultAsync(m => m.MaLoaiPhong == id);
            if (lp == null) return NotFound();
            return View(lp);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lp = await _context.LoaiPhongs.FindAsync(id);
            if (lp != null)
            {
                _context.LoaiPhongs.Remove(lp);
                await _context.SaveChangesAsync();
                TempData["success"] = "Đã xóa loại phòng!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}