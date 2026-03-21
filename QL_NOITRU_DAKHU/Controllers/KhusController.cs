using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;

namespace QL_NOITRU_DAKHU.Controllers
{
    // Cho phép cả 3 quyền vào xem danh sách và chi tiết
    [Authorize(Roles = "Admin,NhanVien,SinhVien")]
    public class KhusController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public KhusController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // GET: Khus (Danh sách)
        public async Task<IActionResult> Index(string search)
        {

            var query = _context.Khus.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(k => k.TenKhu.Contains(search));
            }

            ViewBag.CurrentSearch = search; // Gửi lại từ khóa ra View để hiện ở ô nhập
            return View(await query.ToListAsync());
        }

        // GET: Khus/Details/5 (Chi tiết)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var khu = await _context.Khus
                .Include(k => k.ToaNhas) // Lấy kèm danh sách tòa nhà
                .FirstOrDefaultAsync(m => m.MaKhu == id);

            if (khu == null) return NotFound();

            return View(khu);
        }

        // GET: Khus/Create (Chỉ Admin/Nhân viên)
        [Authorize(Roles = "Admin,NhanVien")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Create(Khu khu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(khu);
                await _context.SaveChangesAsync();
                TempData["success"] = "Thêm khu mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(khu);
        }

        // GET: Khus/Edit/5 (Chỉ Admin/Nhân viên)
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var khu = await _context.Khus.FindAsync(id);
            if (khu == null) return NotFound();
            return View(khu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int id, Khu khu)
        {
            if (id != khu.MaKhu) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(khu);
                await _context.SaveChangesAsync();
                TempData["success"] = "Cập nhật khu thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(khu);
        }

        // GET: Khus/Delete/5 (Chỉ Admin mới có quyền xóa)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var khu = await _context.Khus.FirstOrDefaultAsync(m => m.MaKhu == id);
            if (khu == null) return NotFound();
            return View(khu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khu = await _context.Khus.FindAsync(id);
            if (khu != null) _context.Khus.Remove(khu);
            await _context.SaveChangesAsync();
            TempData["success"] = "Đã xóa khu vĩnh viễn!";
            return RedirectToAction(nameof(Index));
        }
    }
}