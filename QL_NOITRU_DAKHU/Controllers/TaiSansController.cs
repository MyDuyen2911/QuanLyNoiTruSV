using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize] // Yêu cầu đăng nhập
    public class TaiSansController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public TaiSansController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH (Có tìm kiếm)
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.TaiSans.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.TenTaiSan.Contains(search) || t.LoaiTaiSan.Contains(search));
            }

            ViewBag.CurrentSearch = search;
            return View(await query.ToListAsync());
        }

        // 2. CHI TIẾT
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var taiSan = await _context.TaiSans
                .Include(t => t.PhongTaiSans) // Kéo theo danh sách phòng đang có tài sản này
                .Include(t => t.SuaChuas)     // Kéo theo lịch sử sửa chữa
                .FirstOrDefaultAsync(m => m.MaTaiSan == id);

            if (taiSan == null) return NotFound();
            return View(taiSan);
        }

        // 3. THÊM MỚI (Chỉ Admin, NhanVien)
        [Authorize(Roles = "Admin,NhanVien")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Create(TaiSan taiSan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taiSan);
                await _context.SaveChangesAsync();
                TempData["success"] = "Thêm danh mục tài sản thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(taiSan);
        }

        // 4. SỬA (Chỉ Admin, NhanVien)
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var taiSan = await _context.TaiSans.FindAsync(id);
            if (taiSan == null) return NotFound();
            return View(taiSan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int id, TaiSan taiSan)
        {
            if (id != taiSan.MaTaiSan) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(taiSan);
                await _context.SaveChangesAsync();
                TempData["success"] = "Cập nhật tài sản thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(taiSan);
        }

        // 5. XÓA (Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var taiSan = await _context.TaiSans.FirstOrDefaultAsync(m => m.MaTaiSan == id);
            if (taiSan == null) return NotFound();

            return View(taiSan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taiSan = await _context.TaiSans.FindAsync(id);
            if (taiSan != null)
            {
                try
                {
                    _context.TaiSans.Remove(taiSan);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Đã xóa tài sản khỏi danh mục!";
                }
                catch (Exception)
                {
                    // Lỗi này xảy ra khi tài sản đang được bố trí trong phòng hoặc đang có đơn sửa chữa
                    TempData["error"] = "Không thể xóa! Tài sản này đang được phân bổ trong các phòng hoặc có lịch sử sửa chữa. Vui lòng kiểm tra lại.";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}