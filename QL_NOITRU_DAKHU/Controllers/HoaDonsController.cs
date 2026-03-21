using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;
using System.Security.Claims;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize]
    public class HoaDonsController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public HoaDonsController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH HÓA ĐƠN
        public async Task<IActionResult> Index(string search, string loaiHoaDon, string trangThai)
        {
            var query = _context.HoaDons
                .Include(h => h.MaPhongNavigation)
                .Include(h => h.MaSvNavigation)
                .AsQueryable();

            if (User.IsInRole("SinhVien"))
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var sv = await _context.SinhViens.FirstOrDefaultAsync(s => s.MaTaiKhoan == userId);

                if (sv != null)
                {
                    var hopDong = await _context.HopDongs.FirstOrDefaultAsync(h => h.MaSv == sv.MaSv && h.TrangThai == "Đang ở");
                    int maPhong = hopDong != null ? (hopDong.MaPhong ?? 0) : 0;
                    query = query.Where(h => h.MaSv == sv.MaSv || (h.MaPhong == maPhong && h.MaSv == null));
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(h => h.MaSvNavigation.Mssv.Contains(search) || h.MaPhongNavigation.TenPhong.Contains(search));
            }
            if (!string.IsNullOrEmpty(loaiHoaDon))
            {
                query = query.Where(h => h.LoaiHoaDon == loaiHoaDon);
            }
            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(h => h.TrangThai == trangThai);
            }

            ViewBag.CurrentSearch = search;
            return View(await query.OrderByDescending(h => h.ThangNam).ThenByDescending(h => h.MaHoaDon).ToListAsync());
        }

        // 2. LẬP HÓA ĐƠN MỚI
        [Authorize(Roles = "Admin,NhanVien")]
        public IActionResult Create()
        {
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong");
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "HoTen");

            var newBill = new HoaDon
            {
                ThangNam = DateOnly.FromDateTime(DateTime.Now),
                TrangThai = "Chưa thanh toán"
            };
            return View(newBill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Create(HoaDon hoaDon)
        {
            ModelState.Remove("MaPhongNavigation");
            ModelState.Remove("MaSvNavigation");

            if (ModelState.IsValid)
            {
                hoaDon.TrangThai = "Chưa thanh toán";
                decimal tile = hoaDon.TyLeGiam ?? 0;
                hoaDon.TongTienSauGiam = hoaDon.TongTien * (1 - (tile / 100));

                _context.Add(hoaDon);
                await _context.SaveChangesAsync();
                TempData["success"] = "Đã lập hóa đơn thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", hoaDon.MaPhong);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "HoTen", hoaDon.MaSv);
            return View(hoaDon);
        }

        // 3. XÁC NHẬN THANH TOÁN
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> ThanhToan(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null && hoaDon.TrangThai == "Chưa thanh toán")
            {
                hoaDon.TrangThai = "Đã thanh toán";
                await _context.SaveChangesAsync();
                TempData["success"] = $"Đã thu tiền hóa đơn #{hoaDon.MaHoaDon} thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        // 4. XÓA HÓA ĐƠN
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var hoaDon = await _context.HoaDons.Include(h => h.MaSvNavigation).Include(h => h.MaPhongNavigation).FirstOrDefaultAsync(m => m.MaHoaDon == id);
            if (hoaDon == null) return NotFound();
            return View(hoaDon);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null)
            {
                _context.HoaDons.Remove(hoaDon);
                await _context.SaveChangesAsync();
                TempData["success"] = "Đã hủy hóa đơn!";
            }
            return RedirectToAction(nameof(Index));
        }

        // 5. API CHI TIẾT
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var hoaDon = await _context.HoaDons
                .Include(h => h.MaPhongNavigation)
                .Include(h => h.MaSvNavigation)
                .FirstOrDefaultAsync(m => m.MaHoaDon == id);
            if (hoaDon == null) return NotFound();
            return View(hoaDon);
        }

        // ================= CÁC API HỖ TRỢ ĐIỀN TỰ ĐỘNG =================

        // Lấy % Giảm giá học lực
        [HttpGet]
        public async Task<IActionResult> GetGiamGia(int maSv, string hocKy)
        {
            if (string.IsNullOrEmpty(hocKy)) return Json(0);
            var hocLuc = await _context.HocLucs.FirstOrDefaultAsync(h => h.MaSv == maSv && h.HocKy == hocKy);
            return Json(hocLuc != null ? (hocLuc.TyLeGiam ?? 0) : 0);
        }

        // Lấy phòng đang ở của sinh viên (cho Tiền phòng)
        [HttpGet]
        // Lấy phòng đang ở của sinh viên (cho Tiền phòng)
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetThongTinPhong(int maSv)
        {
            var hopDong = await _context.HopDongs
                .Include(h => h.MaPhongNavigation)
                .FirstOrDefaultAsync(h => h.MaSv == maSv && h.TrangThai == "Đang ở");

            if (hopDong != null && hopDong.MaPhongNavigation != null)
            {
                // SỬA LẠI THÀNH DÒNG NÀY ĐỂ FIX LỖI (Gán tạm giá = 0, bạn sẽ tự gõ tay số tiền)
                decimal gia = 0;

                return Json(new { success = true, maPhong = hopDong.MaPhong, giaTien = gia });
            }
            return Json(new { success = false });
        }

        // Lấy tiền điện nước của phòng trong tháng
        [HttpGet]
        public async Task<IActionResult> GetTienDienNuoc(int maPhong, int thang, int nam)
        {
            var dienNuoc = await _context.DienNuocs
                .FirstOrDefaultAsync(d => d.MaPhong == maPhong && d.Thang == thang && d.Nam == nam);

            if (dienNuoc != null)
            {
                var tieuThu = (dienNuoc.ChiSoCuoi ?? 0) - (dienNuoc.ChiSoDau ?? 0);
                var thanhTien = tieuThu * (dienNuoc.DonGia ?? 0);
                return Json(new { success = true, tien = thanhTien });
            }
            return Json(new { success = false, message = "Phòng này chưa được chốt số điện nước trong tháng này!" });
        }
    }
}