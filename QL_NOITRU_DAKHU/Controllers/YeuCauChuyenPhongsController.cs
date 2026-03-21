using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;
using System.Security.Claims;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize]
    public class YeuCauChuyenPhongsController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public YeuCauChuyenPhongsController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH YÊU CẦU
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.YeuCauChuyenPhongs
                .Include(y => y.MaPhongMuonChuyenNavigation)
                .Include(y => y.MaSvNavigation)
                .AsQueryable();

            if (User.IsInRole("SinhVien"))
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                query = query.Where(y => y.MaSvNavigation.MaTaiKhoan == userId);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(y => y.MaSvNavigation.HoTen.Contains(search) ||
                                         y.MaPhongMuonChuyenNavigation.TenPhong.Contains(search));
            }

            // Gắn thêm thông tin "Phòng hiện tại" vào ViewBag để hiển thị ngoài View
            var danhSachYeuCau = await query.OrderByDescending(y => y.MaYeuCau).ToListAsync();
            var phongHienTaiDict = new Dictionary<int, string>();

            foreach (var yeuCau in danhSachYeuCau)
            {
                var hopDong = await _context.HopDongs
                    .Include(h => h.MaPhongNavigation)
                    .FirstOrDefaultAsync(h => h.MaSv == yeuCau.MaSv && h.TrangThai == "Đang ở");

                phongHienTaiDict[yeuCau.MaYeuCau] = hopDong?.MaPhongNavigation?.TenPhong ?? "Không rõ";
            }

            ViewBag.PhongHienTaiDict = phongHienTaiDict;
            ViewBag.CurrentSearch = search;

            return View(danhSachYeuCau);
        }

        // 2. SINH VIÊN TẠO YÊU CẦU
        [Authorize(Roles = "SinhVien")]
        public async Task<IActionResult> Create()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.MaTaiKhoan == userId);
            if (sinhVien == null) return NotFound();

            // Kiểm tra: Sinh viên phải đang ở 1 phòng nào đó mới được xin chuyển
            var hopDongHienTai = await _context.HopDongs
                .FirstOrDefaultAsync(h => h.MaSv == sinhVien.MaSv && h.TrangThai == "Đang ở");

            if (hopDongHienTai == null)
            {
                TempData["error"] = "Bạn chưa có phòng lưu trú nên không thể gửi yêu cầu chuyển phòng. Vui lòng làm Đơn đăng ký phòng mới!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra: Không cho gửi 2 yêu cầu cùng lúc
            var dangChoDuyet = await _context.YeuCauChuyenPhongs
                .AnyAsync(y => y.MaSv == sinhVien.MaSv && y.TrangThai == "Chờ duyệt");

            if (dangChoDuyet)
            {
                TempData["error"] = "Bạn đang có một yêu cầu chuyển phòng chờ duyệt. Vui lòng đợi kết quả!";
                return RedirectToAction(nameof(Index));
            }

            // Lấy danh sách phòng: Trừ phòng hiện tại và các phòng đã Đầy
            var danhSachPhong = _context.Phongs
                .Where(p => p.MaPhong != hopDongHienTai.MaPhong && p.TrangThai != "Đầy")
                .ToList();

            ViewData["MaPhongMuonChuyen"] = new SelectList(danhSachPhong, "MaPhong", "TenPhong");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SinhVien")]
        public async Task<IActionResult> Create(YeuCauChuyenPhong yeuCau)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.MaTaiKhoan == userId);

            ModelState.Remove("MaPhongMuonChuyenNavigation");
            ModelState.Remove("MaSvNavigation");

            if (ModelState.IsValid && sinhVien != null)
            {
                yeuCau.MaSv = sinhVien.MaSv;
                yeuCau.TrangThai = "Chờ duyệt";

                _context.Add(yeuCau);
                await _context.SaveChangesAsync();
                TempData["success"] = "Đã gửi yêu cầu chuyển phòng thành công. Vui lòng chờ phản hồi!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaPhongMuonChuyen"] = new SelectList(_context.Phongs.Where(p => p.TrangThai != "Đầy"), "MaPhong", "TenPhong", yeuCau.MaPhongMuonChuyen);
            return View(yeuCau);
        }

        // 3. ADMIN/NHÂN VIÊN DUYỆT CHUYỂN PHÒNG
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Approve(int id)
        {
            var yeuCau = await _context.YeuCauChuyenPhongs
                .Include(y => y.MaPhongMuonChuyenNavigation)
                .FirstOrDefaultAsync(y => y.MaYeuCau == id);

            if (yeuCau == null || yeuCau.TrangThai != "Chờ duyệt") return NotFound();

            // 1. Kiểm tra phòng mới có bị đầy chưa (Lỡ ai đó chen chân vào trước)
            if (yeuCau.MaPhongMuonChuyenNavigation?.TrangThai == "Đầy")
            {
                TempData["error"] = $"Phòng {yeuCau.MaPhongMuonChuyenNavigation.TenPhong} đã đầy, không thể duyệt chuyển vào lúc này!";
                return RedirectToAction(nameof(Index));
            }

            // 2. TÌM HỢP ĐỒNG HIỆN TẠI VÀ CHUYỂN PHÒNG
            var hopDongHienTai = await _context.HopDongs
                .FirstOrDefaultAsync(h => h.MaSv == yeuCau.MaSv && h.TrangThai == "Đang ở");

            if (hopDongHienTai != null)
            {
                var maPhongCu = hopDongHienTai.MaPhong;

                // Đổi mã phòng trong hợp đồng sang phòng mới
                hopDongHienTai.MaPhong = yeuCau.MaPhongMuonChuyen;
                _context.Update(hopDongHienTai);

                // Cập nhật trạng thái phòng CŨ (Nếu không còn ai ở thì thành Trống)
                var soNguoiOPhongCu = await _context.HopDongs
                    .CountAsync(h => h.MaPhong == maPhongCu && h.TrangThai == "Đang ở" && h.MaHopDong != hopDongHienTai.MaHopDong);

                if (soNguoiOPhongCu == 0)
                {
                    var phongCu = await _context.Phongs.FindAsync(maPhongCu);
                    if (phongCu != null)
                    {
                        phongCu.TrangThai = "Trống"; // ⚠️ CHÚ Ý: ĐẢM BẢO CHỮ NÀY GIỐNG TRONG SQL CỦA BẠN
                        _context.Update(phongCu);
                    }
                }

                // Cập nhật trạng thái phòng MỚI (Từ Trống -> Đang ở)
                if (yeuCau.MaPhongMuonChuyenNavigation.TrangThai == "Trống") // ⚠️ CHÚ Ý
                {
                    yeuCau.MaPhongMuonChuyenNavigation.TrangThai = "Đang ở"; // ⚠️ CHÚ Ý
                    _context.Update(yeuCau.MaPhongMuonChuyenNavigation);
                }
            }

            // 3. Đổi trạng thái Yêu cầu thành Đã duyệt
            yeuCau.TrangThai = "Đã duyệt";
            _context.Update(yeuCau);

            try
            {
                await _context.SaveChangesAsync();
                TempData["success"] = $"Đã duyệt! Hệ thống đã tự động chuyển sinh viên sang phòng {yeuCau.MaPhongMuonChuyenNavigation?.TenPhong}.";
            }
            catch (Exception ex)
            {
                // Bắt lỗi SQL Check Constraint và hiển thị ra màn hình
                TempData["error"] = "Lỗi Database: SQL không cho phép cập nhật trạng thái phòng. Vui lòng kiểm tra lại Constraint của cột TrangThai trong bảng Phong. Chi tiết: " + (ex.InnerException?.Message ?? ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        // 4. ADMIN/NHÂN VIÊN TỪ CHỐI
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Reject(int id)
        {
            var yeuCau = await _context.YeuCauChuyenPhongs.FindAsync(id);
            if (yeuCau != null && yeuCau.TrangThai == "Chờ duyệt")
            {
                yeuCau.TrangThai = "Từ chối";
                await _context.SaveChangesAsync();
                TempData["error"] = "Đã từ chối yêu cầu chuyển phòng.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}