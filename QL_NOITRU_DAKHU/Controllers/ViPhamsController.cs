using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize] // Ai đăng nhập cũng được vào
    public class ViPhamsController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public ViPhamsController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH VI PHẠM (CÓ TÌM KIẾM)
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.ViPhams.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(v => v.TenViPham.Contains(search));
            }

            ViewBag.CurrentSearch = search;
            return View(await query.ToListAsync());
        }

        // 2. CHI TIẾT
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var viPham = await _context.ViPhams
                .Include(v => v.BienBanViPhams) // Lấy luôn danh sách các biên bản đã lập bằng lỗi này
                .FirstOrDefaultAsync(m => m.MaViPham == id);

            if (viPham == null) return NotFound();
            return View(viPham);
        }

        // 3. THÊM MỚI (Admin, Nhân viên)
        [Authorize(Roles = "Admin,NhanVien")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Create(ViPham viPham)
        {
            if (ModelState.IsValid)
            {
                _context.Add(viPham);
                await _context.SaveChangesAsync();
                TempData["success"] = "Thêm danh mục vi phạm thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(viPham);
        }

        // 4. SỬA (Admin, Nhân viên)
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var viPham = await _context.ViPhams.FindAsync(id);
            if (viPham == null) return NotFound();
            return View(viPham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int id, ViPham viPham)
        {
            if (id != viPham.MaViPham) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(viPham);
                await _context.SaveChangesAsync();
                TempData["success"] = "Cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(viPham);
        }

        // 5. XÓA (Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var viPham = await _context.ViPhams.FirstOrDefaultAsync(m => m.MaViPham == id);
            if (viPham == null) return NotFound();
            return View(viPham);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var viPham = await _context.ViPhams.FindAsync(id);
            if (viPham != null)
            {
                _context.ViPhams.Remove(viPham);
                await _context.SaveChangesAsync();
                TempData["success"] = "Đã xóa danh mục vi phạm!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}