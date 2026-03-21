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
    [Authorize]
    public class NhanViensController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public NhanViensController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // ================= DROPDOWN TÀI KHOẢN (Chỉ lấy tài khoản Nhân Viên) =================
        private void LoadTaiKhoanDropdown(int? selectedId = null, int? currentMaNV = null)
        {
            var usedIds = _context.NhanViens
                .Where(n => n.MaTaiKhoan != null && n.MaNv != currentMaNV)
                .Select(n => n.MaTaiKhoan!.Value)
                .ToList();

            ViewBag.MaTaiKhoan = new SelectList(
                _context.TaiKhoans
                    .Where(t => t.VaiTro == "NhanVien" && !usedIds.Contains(t.MaTaiKhoan))
                    .Select(t => new { t.MaTaiKhoan, Display = t.Username + " - " + t.VaiTro }),
                "MaTaiKhoan", "Display", selectedId
            );
        }

        // ================= DANH SÁCH NHÂN VIÊN (Chỉ Admin) =================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.NhanViens.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(n => n.HoTen.Contains(search) || n.DienThoai.Contains(search) || n.Email.Contains(search));
            }

            ViewBag.CurrentSearch = search;
            return View(await query.OrderBy(n => n.MaNv).ToListAsync());
        }

        // ================= THÊM NHÂN VIÊN =================
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            LoadTaiKhoanDropdown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(NhanVien nv)
        {
            if (ModelState.IsValid)
            {
                if (nv.MaTaiKhoan == null)
                {
                    var tk = new TaiKhoan
                    {
                        Username = "nv" + DateTime.Now.Ticks.ToString().Substring(10),
                        PasswordHash = "123456",
                        VaiTro = "NhanVien",
                        TrangThai = "Active"
                    };
                    _context.TaiKhoans.Add(tk);
                    await _context.SaveChangesAsync();
                    nv.MaTaiKhoan = tk.MaTaiKhoan;
                }

                _context.NhanViens.Add(nv);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "🎉 Đã thêm nhân viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            LoadTaiKhoanDropdown(nv.MaTaiKhoan);
            return View(nv);
        }

        // ================= SỬA NHÂN VIÊN =================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var nv = await _context.NhanViens.FindAsync(id);
            if (nv == null) return NotFound();

            LoadTaiKhoanDropdown(nv.MaTaiKhoan, nv.MaNv);
            return View(nv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, NhanVien nv)
        {
            if (id != nv.MaNv) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(nv);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "🎉 Cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            LoadTaiKhoanDropdown(nv.MaTaiKhoan, nv.MaNv);
            return View(nv);
        }

        // ================= CHI TIẾT NHÂN VIÊN (Admin xem) =================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var nv = await _context.NhanViens.FirstOrDefaultAsync(n => n.MaNv == id);
            if (nv == null) return NotFound();
            return View(nv);
        }

        // ================= XÓA NHÂN VIÊN =================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var nv = await _context.NhanViens.FirstOrDefaultAsync(n => n.MaNv == id);
            if (nv == null) return NotFound();
            return View(nv);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nv = await _context.NhanViens.Include(n => n.MaTaiKhoanNavigation).FirstOrDefaultAsync(n => n.MaNv == id);
            if (nv != null)
            {
                if (nv.MaTaiKhoanNavigation != null) _context.TaiKhoans.Remove(nv.MaTaiKhoanNavigation);
                _context.NhanViens.Remove(nv);
                await _context.SaveChangesAsync();
            }
            TempData["SuccessMessage"] = "🎉 Đã xóa nhân viên!";
            return RedirectToAction(nameof(Index));
        }

        // ================= HỒ SƠ CÁ NHÂN (Dành cho Nhân viên đăng nhập) =================
        [Authorize(Roles = "NhanVien")]
        public async Task<IActionResult> MyProfile()
        {
            var currentUsername = User.Identity.Name;
            var myProfile = await _context.NhanViens
                .Include(n => n.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(n => n.MaTaiKhoanNavigation.Username == currentUsername);

            if (myProfile == null) return NotFound("Không tìm thấy hồ sơ của bạn.");
            return View(myProfile);
        }
    }
}