using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize(Roles = "Admin")] // BẢO MẬT: CHỈ ADMIN MỚI ĐƯỢC VÀO QUẢN LÝ TÀI KHOẢN
    public class TaiKhoansController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public TaiKhoansController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH TÀI KHOẢN
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.TaiKhoans.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.Username.Contains(search) || t.VaiTro.Contains(search));
            }

            ViewBag.CurrentSearch = search;
            return View(await query.ToListAsync());
        }

        // 2. CHI TIẾT
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.SinhVien)   // Kéo thông tin SV nếu là tài khoản sinh viên
                .Include(t => t.NhanVien)   // Kéo thông tin NV nếu là tài khoản nhân viên
                .FirstOrDefaultAsync(m => m.MaTaiKhoan == id);

            if (taiKhoan == null) return NotFound();
            return View(taiKhoan);
        }

        // 3. THÊM MỚI
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaiKhoan taiKhoan)
        {
            // Bỏ qua kiểm tra các liên kết (tránh lỗi null!)
            ModelState.Remove("AuditLogs");
            ModelState.Remove("NhanVien");
            ModelState.Remove("SinhVien");

            // Kiểm tra trùng Username
            var isExist = await _context.TaiKhoans.AnyAsync(t => t.Username == taiKhoan.Username);
            if (isExist)
            {
                TempData["error"] = "Tên đăng nhập (Username) này đã tồn tại trong hệ thống!";
                return View(taiKhoan);
            }

            if (ModelState.IsValid)
            {
                // Lưu ý: Ở hệ thống thực tế PasswordHash phải được mã hóa (VD: MD5, BCrypt). 
                // Ở đây lưu chuỗi trực tiếp theo đồ án của bạn.
                _context.Add(taiKhoan);
                await _context.SaveChangesAsync();
                TempData["success"] = "Tạo tài khoản mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(taiKhoan);
        }

        // 4. SỬA (Thường dùng để đổi mật khẩu, cấp lại quyền, khóa tài khoản)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null) return NotFound();
            return View(taiKhoan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.MaTaiKhoan) return NotFound();

            ModelState.Remove("AuditLogs");
            ModelState.Remove("NhanVien");
            ModelState.Remove("SinhVien");

            // Kiểm tra trùng Username với người khác
            var isExist = await _context.TaiKhoans.AnyAsync(t => t.Username == taiKhoan.Username && t.MaTaiKhoan != id);
            if (isExist)
            {
                TempData["error"] = "Tên đăng nhập này đã bị người khác sử dụng!";
                return View(taiKhoan);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taiKhoan);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Cập nhật tài khoản thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["error"] = "Lỗi Database: " + (ex.InnerException?.Message ?? ex.Message);
                }
            }
            return View(taiKhoan);
        }

        // 5. XÓA
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.SinhVien)
                .Include(t => t.NhanVien)
                .FirstOrDefaultAsync(m => m.MaTaiKhoan == id);

            if (taiKhoan == null) return NotFound();
            return View(taiKhoan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan != null)
            {
                try
                {
                    _context.TaiKhoans.Remove(taiKhoan);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Đã xóa vĩnh viễn tài khoản!";
                }
                catch (Exception)
                {
                    TempData["error"] = "Không thể xóa! Tài khoản này đang được liên kết với hồ sơ Sinh Viên hoặc Nhân Viên. Vui lòng xóa hồ sơ liên quan trước.";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}