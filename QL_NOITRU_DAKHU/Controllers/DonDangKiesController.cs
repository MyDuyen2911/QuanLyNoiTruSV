using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;
using System.Security.Claims;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize]
    public class DonDangKiesController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public DonDangKiesController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH ĐƠN
        public async Task<IActionResult> Index(string search)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim);

            var query = _context.DonDangKies
                .Include(d => d.MaPhongDangKyNavigation)
                .Include(d => d.MaSvNavigation)
                .AsQueryable();

            if (User.IsInRole("SinhVien"))
            {
                query = query.Where(d => d.MaSvNavigation.MaTaiKhoan == userId);

                // THÊM ĐOẠN NÀY ĐỂ TÌM PHÒNG ĐANG Ở HIỆN RA BANNER TRANG CHỦ
                var phongDangO = await _context.HopDongs
                    .Include(h => h.MaPhongNavigation)
                        .ThenInclude(p => p.MaToaNavigation)
                            .ThenInclude(t => t.MaKhuNavigation)
                    .FirstOrDefaultAsync(h => h.MaSvNavigation.MaTaiKhoan == userId && h.TrangThai == "Đang ở");

                ViewBag.PhongHienTai = phongDangO;
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.MaSvNavigation.HoTen.Contains(search) || d.MaPhongDangKyNavigation.TenPhong.Contains(search));
            }

            ViewBag.CurrentSearch = search;
            return View(await query.OrderByDescending(d => d.NgayDangKy).ToListAsync());
        }

        // 2. SINH VIÊN GỬI ĐƠN
        [Authorize(Roles = "SinhVien")]
        public async Task<IActionResult> Create()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdClaim);

            var hopDongHienTai = await _context.HopDongs
                .FirstOrDefaultAsync(h => h.MaSvNavigation.MaTaiKhoan == userId && h.TrangThai == "Đang ở");

            if (hopDongHienTai != null)
            {
                TempData["error"] = "Bạn hiện đang có phòng lưu trú (Hợp đồng đang có hiệu lực). Bạn không thể gửi thêm đơn đăng ký phòng mới. Nếu muốn đổi phòng, vui lòng sử dụng tính năng Chuyển phòng.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaPhongDangKy"] = new SelectList(_context.Phongs.Where(p => p.TrangThai != "Đầy"), "MaPhong", "TenPhong");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SinhVien")]
        public async Task<IActionResult> Create(DonDangKy don)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");
            var userId = int.Parse(userIdClaim);

            var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.MaTaiKhoan == userId);

            if (sinhVien == null)
            {
                TempData["error"] = "Không tìm thấy hồ sơ sinh viên cho tài khoản này!";
                return RedirectToAction(nameof(Index));
            }

            don.MaSv = sinhVien.MaSv;
            don.NgayDangKy = DateTime.Now;
            don.TrangThai = "Chờ duyệt";

            _context.Add(don);
            await _context.SaveChangesAsync();

            TempData["success"] = "Gửi đơn thành công!";
            return RedirectToAction(nameof(Index));
        }

        // 3. ADMIN/NHÂN VIÊN DUYỆT ĐƠN (TỰ ĐỘNG TẠO HỢP ĐỒNG & CẬP NHẬT TRẠNG THÁI PHÒNG)
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Approve(int id)
        {
            var don = await _context.DonDangKies
                .Include(d => d.MaPhongDangKyNavigation)
                .FirstOrDefaultAsync(m => m.MaDon == id);

            if (don == null || don.TrangThai != "Chờ duyệt") return NotFound();

            // 1. KIỂM TRA PHÒNG ĐÃ ĐẦY CHƯA (Dựa vào Trạng thái của phòng)
            if (don.MaPhongDangKyNavigation?.TrangThai == "Đầy")
            {
                // Trả về lỗi, không cho duyệt
                TempData["error"] = $"Không thể duyệt! Phòng {don.MaPhongDangKyNavigation.TenPhong} đã đầy.";
                return RedirectToAction(nameof(Index));
            }

            // 2. CẬP NHẬT TRẠNG THÁI ĐƠN VÀ TẠO HỢP ĐỒNG
            don.TrangThai = "Đã duyệt";

            var hopDongMoi = new HopDong
            {
                MaSv = don.MaSv,
                MaPhong = don.MaPhongDangKy,
                NgayBatDau = DateOnly.FromDateTime(DateTime.Now),
                NgayKetThuc = DateOnly.FromDateTime(DateTime.Now.AddMonths(6)),
                TrangThai = "Đang ở"
            };

            _context.Add(hopDongMoi);

            // 3. TỰ ĐỘNG ĐỔI TRẠNG THÁI PHÒNG 
            // Nếu phòng đang "Trống" -> Đổi thành "Đang ở"
            if (don.MaPhongDangKyNavigation.TrangThai == "Trống")
            {
                don.MaPhongDangKyNavigation.TrangThai = "Đang ở";
                _context.Update(don.MaPhongDangKyNavigation);
            }

            // Lưu tất cả thay đổi vào Database
            await _context.SaveChangesAsync();

            TempData["success"] = $"Duyệt đơn thành công! Đã tự động xếp sinh viên vào phòng {don.MaPhongDangKyNavigation.TenPhong}.";
            return RedirectToAction(nameof(Index));
        }

        // 4. TỪ CHỐI ĐƠN
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Reject(int id)
        {
            var don = await _context.DonDangKies.FindAsync(id);
            if (don != null)
            {
                don.TrangThai = "Từ chối";
                await _context.SaveChangesAsync();
                TempData["error"] = "Đã từ chối đơn đăng ký.";
            }
            return RedirectToAction(nameof(Index));
        }

        // 5. CHI TIẾT
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var donDangKy = await _context.DonDangKies
                .Include(d => d.MaPhongDangKyNavigation)
                .Include(d => d.MaSvNavigation)
                .FirstOrDefaultAsync(m => m.MaDon == id);

            if (donDangKy == null) return NotFound();

            return View(donDangKy);
        }
    }
}