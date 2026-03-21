using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;
using System;
using System.Linq;
using System.Security.Claims; // SỬA LỖI THIẾU CLAIMTYPES Ở ĐÂY
using System.Threading.Tasks;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize]
    public class QLSinhViensController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public QLSinhViensController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // ================= DROPDOWN =================
        private void LoadTaiKhoanDropdown(int? selectedId = null, int? currentMaSv = null)
        {
            var usedIds = _context.SinhViens
                .Where(s => s.MaTaiKhoan != null && s.MaSv != currentMaSv)
                .Select(s => s.MaTaiKhoan!.Value)
                .ToList();

            ViewBag.MaTaiKhoan = new SelectList(
                _context.TaiKhoans
                    .Where(t => t.VaiTro == "SinhVien" && !usedIds.Contains(t.MaTaiKhoan))
                    .Select(t => new
                    {
                        t.MaTaiKhoan,
                        Display = t.Username + " - " + t.VaiTro
                    }),
                "MaTaiKhoan",
                "Display",
                selectedId
            );
        }

        // ================= INDEX =================
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            int pageSize = 5;
            var query = _context.SinhViens.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s =>
                    s.HoTen.Contains(search) ||
                    s.Cccd.Contains(search) ||
                    s.DienThoai.Contains(search));
            }

            int total = await query.CountAsync();

            var data = await query
                .OrderBy(s => s.MaSv)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentSearch = search;
            ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
            ViewBag.CurrentPage = page;

            return View(data);
        }

        // ================= CREATE =================
        [Authorize(Roles = "Admin,NhanVien")]
        public IActionResult Create()
        {
            LoadTaiKhoanDropdown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Create(SinhVien sv)
        {
            if (ModelState.IsValid)
            {
                if (sv.MaTaiKhoan == null)
                {
                    var tk = new TaiKhoan
                    {
                        Username = "sv" + DateTime.Now.Ticks.ToString().Substring(10),
                        PasswordHash = "123456",
                        VaiTro = "SinhVien",
                        TrangThai = "Active"
                    };

                    _context.TaiKhoans.Add(tk);
                    await _context.SaveChangesAsync();
                    sv.MaTaiKhoan = tk.MaTaiKhoan;
                }

                _context.SinhViens.Add(sv);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "🎉 Thêm sinh viên thành công!";
                return RedirectToAction(nameof(Index));
            }

            LoadTaiKhoanDropdown(sv.MaTaiKhoan);
            return View(sv);
        }

        // ================= EDIT =================
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int id)
        {
            var sv = await _context.SinhViens.FindAsync(id);
            if (sv == null) return NotFound();

            LoadTaiKhoanDropdown(sv.MaTaiKhoan, sv.MaSv);
            return View(sv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int id, SinhVien sv)
        {
            if (id != sv.MaSv) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(sv);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "🎉 Cập nhật sinh viên thành công!";
                return RedirectToAction(nameof(Index));
            }

            LoadTaiKhoanDropdown(sv.MaTaiKhoan, sv.MaSv);
            return View(sv);
        }

        // ================= DELETE =================
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Delete(int id)
        {
            var sv = await _context.SinhViens
                .Include(s => s.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(s => s.MaSv == id);

            if (sv == null) return NotFound();
            return View(sv);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sv = await _context.SinhViens
                .Include(s => s.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(s => s.MaSv == id);

            if (sv != null)
            {
                if (sv.MaTaiKhoanNavigation != null)
                    _context.TaiKhoans.Remove(sv.MaTaiKhoanNavigation);

                _context.SinhViens.Remove(sv);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "🎉 Xóa sinh viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ================= DETAILS (Cả 3 Role) =================
        [Authorize(Roles = "Admin,NhanVien,SinhVien")]
        public async Task<IActionResult> Details(int? id)
        {
            if (User.IsInRole("SinhVien"))
            {
                var currentUsername = User.Identity.Name;
                var myProfile = await _context.SinhViens
                    .Include(s => s.MaTaiKhoanNavigation)
                    .FirstOrDefaultAsync(s => s.MaTaiKhoanNavigation.Username == currentUsername);

                if (myProfile == null) return NotFound("Không tìm thấy hồ sơ của bạn.");
                if (id.HasValue && id.Value != myProfile.MaSv) return Forbid();

                return View(myProfile);
            }

            if (id == null) return NotFound();
            var sv = await _context.SinhViens.FirstOrDefaultAsync(s => s.MaSv == id);
            if (sv == null) return NotFound();

            return View(sv);
        }

        // ================= XEM THÔNG TIN CÁ NHÂN KÈM PHÒNG (Dành riêng Sinh Viên) =================
        [Authorize(Roles = "SinhVien")]
        public async Task<IActionResult> MyProfile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdClaim);

            var currentUsername = User.Identity.Name;

            var myProfile = await _context.SinhViens
                .Include(s => s.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(s => s.MaTaiKhoanNavigation.Username == currentUsername);

            if (myProfile == null)
            {
                return NotFound("Không tìm thấy hồ sơ của bạn trong hệ thống.");
            }

            var hopDongHienTai = await _context.HopDongs
                .Include(h => h.MaPhongNavigation)
                    .ThenInclude(p => p.MaToaNavigation)
                        .ThenInclude(t => t.MaKhuNavigation)
                .FirstOrDefaultAsync(h => h.MaSvNavigation.MaTaiKhoan == userId && h.TrangThai == "Đang ở");

            ViewBag.PhongHienTai = hopDongHienTai;

            return View(myProfile);
        }
    }
}