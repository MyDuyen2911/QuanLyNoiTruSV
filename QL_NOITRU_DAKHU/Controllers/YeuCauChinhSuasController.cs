using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;
using System.Security.Claims;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize]
    public class YeuCauChinhSuasController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public YeuCauChinhSuasController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH YÊU CẦU
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.YeuCauChinhSuas
                .Include(y => y.MaSvNavigation)
                .AsQueryable();

            // Phân quyền: Sinh viên chỉ thấy đơn của mình
            if (User.IsInRole("SinhVien"))
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                query = query.Where(y => y.MaSvNavigation.MaTaiKhoan == userId);
            }

            // Tìm kiếm theo MSSV hoặc Tên
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(y => y.MaSvNavigation.HoTen.Contains(search) ||
                                         y.MaSvNavigation.Mssv.Contains(search));
            }

            ViewBag.CurrentSearch = search;
            return View(await query.OrderByDescending(y => y.MaYeuCau).ToListAsync());
        }

        // 2. SINH VIÊN GỬI YÊU CẦU
        [Authorize(Roles = "SinhVien")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SinhVien")]
        public async Task<IActionResult> Create(YeuCauChinhSua yeuCau)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.MaTaiKhoan == userId);

            ModelState.Remove("MaSvNavigation");

            if (ModelState.IsValid && sinhVien != null)
            {
                yeuCau.MaSv = sinhVien.MaSv;
                yeuCau.TrangThai = "Chờ duyệt"; // Đặt mặc định

                _context.Add(yeuCau);
                await _context.SaveChangesAsync();

                TempData["success"] = "Đã gửi yêu cầu chỉnh sửa hồ sơ. Vui lòng chờ Ban quản lý phản hồi!";
                return RedirectToAction(nameof(Index));
            }

            return View(yeuCau);
        }

        // 3. ADMIN/NHÂN VIÊN DUYỆT ĐƠN
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Approve(int id)
        {
            var yeuCau = await _context.YeuCauChinhSuas
                .Include(y => y.MaSvNavigation)
                .FirstOrDefaultAsync(y => y.MaYeuCau == id);

            if (yeuCau == null || yeuCau.TrangThai != "Chờ duyệt") return NotFound();

            yeuCau.TrangThai = "Đã duyệt";
            _context.Update(yeuCau);

            try
            {
                await _context.SaveChangesAsync();
                TempData["success"] = $"Đã duyệt yêu cầu của sinh viên {yeuCau.MaSvNavigation?.HoTen}. Đừng quên vào mục Sinh Viên để cập nhật thông tin thực tế cho bạn ấy nhé!";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi Database (Kiểm tra lại CHECK Constraint cột TrangThai): " + (ex.InnerException?.Message ?? ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        // 4. ADMIN/NHÂN VIÊN TỪ CHỐI
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Reject(int id)
        {
            var yeuCau = await _context.YeuCauChinhSuas.FindAsync(id);
            if (yeuCau != null && yeuCau.TrangThai == "Chờ duyệt")
            {
                yeuCau.TrangThai = "Từ chối";
                await _context.SaveChangesAsync();
                TempData["error"] = "Đã từ chối yêu cầu chỉnh sửa thông tin.";
            }
            return RedirectToAction(nameof(Index));
        }

        // 5. XÓA ĐƠN (Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var yeuCau = await _context.YeuCauChinhSuas
                .Include(y => y.MaSvNavigation)
                .FirstOrDefaultAsync(m => m.MaYeuCau == id);

            if (yeuCau == null) return NotFound();
            return View(yeuCau);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var yeuCau = await _context.YeuCauChinhSuas.FindAsync(id);
            if (yeuCau != null)
            {
                _context.YeuCauChinhSuas.Remove(yeuCau);
                await _context.SaveChangesAsync();
                TempData["success"] = "Đã xóa lịch sử yêu cầu!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}