using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize(Roles = "Admin,NhanVien,SinhVien")]
    public class ToaNhasController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public ToaNhasController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH TÒA NHÀ
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.ToaNhas.Include(t => t.MaKhuNavigation).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.TenToa.Contains(search) || t.MaKhuNavigation.TenKhu.Contains(search));
            }

            ViewBag.CurrentSearch = search; // Giữ lại chữ đã nhập trong ô tìm kiếm
            return View(await query.ToListAsync());
        }

        // 2. CHI TIẾT TÒA NHÀ (Xem kèm danh sách phòng)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var toaNha = await _context.ToaNhas
                .Include(t => t.MaKhuNavigation)
                .Include(t => t.Phongs) // Lấy luôn danh sách phòng trong tòa
                .FirstOrDefaultAsync(m => m.MaToa == id);

            if (toaNha == null) return NotFound();

            return View(toaNha);
        }

        // 3. THÊM MỚI (Chỉ Admin/NV)
        [Authorize(Roles = "Admin,NhanVien")]
        public IActionResult Create()
        {
            ViewData["MaKhu"] = new SelectList(_context.Khus, "MaKhu", "TenKhu");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ToaNha toaNha)
        {
            if (ModelState.IsValid)
            {
                _context.Add(toaNha);
                await _context.SaveChangesAsync();
                TempData["success"] = "Thêm tòa nhà thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKhu"] = new SelectList(_context.Khus, "MaKhu", "TenKhu", toaNha.MaKhu);
            return View(toaNha);
        }

        // 4. CHỈNH SỬA (Chỉ Admin/NV)
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var toaNha = await _context.ToaNhas.FindAsync(id);
            if (toaNha == null) return NotFound();

            ViewData["MaKhu"] = new SelectList(_context.Khus, "MaKhu", "TenKhu", toaNha.MaKhu);
            return View(toaNha);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ToaNha toaNha)
        {
            if (id != toaNha.MaToa) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(toaNha);
                await _context.SaveChangesAsync();
                TempData["success"] = "Cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKhu"] = new SelectList(_context.Khus, "MaKhu", "TenKhu", toaNha.MaKhu);
            return View(toaNha);
        }

        // 5. XÓA (Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var toaNha = await _context.ToaNhas.Include(t => t.MaKhuNavigation).FirstOrDefaultAsync(m => m.MaToa == id);
            if (toaNha == null) return NotFound();
            return View(toaNha);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var toaNha = await _context.ToaNhas.FindAsync(id);
            if (toaNha != null) _context.ToaNhas.Remove(toaNha);
            await _context.SaveChangesAsync();
            TempData["success"] = "Đã xóa tòa nhà!";
            return RedirectToAction(nameof(Index));
        }
    }
}