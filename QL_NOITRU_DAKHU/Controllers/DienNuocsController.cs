using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;
using System.Security.Claims;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize]
    public class DienNuocsController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public DienNuocsController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH (Sinh viên chỉ xem phòng mình, Admin xem hết)
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.DienNuocs
                .Include(d => d.MaPhongNavigation)
                .AsQueryable();

            // Nếu là Sinh viên -> Lọc theo phòng đang ở
            if (User.IsInRole("SinhVien"))
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var hopDong = await _context.HopDongs
                    .FirstOrDefaultAsync(h => h.MaSvNavigation.MaTaiKhoan == userId && h.TrangThai == "Đang ở");

                if (hopDong != null)
                {
                    query = query.Where(d => d.MaPhong == hopDong.MaPhong);
                }
                else
                {
                    query = query.Where(d => d.MaPhong == -1); // Không có phòng thì không thấy gì
                }
            }

            // Tìm kiếm (Tìm theo Tên phòng hoặc Tháng/Năm)
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.MaPhongNavigation.TenPhong.Contains(search) ||
                                         d.Thang.ToString().Contains(search) ||
                                         d.Nam.ToString().Contains(search));
            }

            ViewBag.CurrentSearch = search;
            // Sắp xếp mới nhất lên đầu
            return View(await query.OrderByDescending(d => d.Nam).ThenByDescending(d => d.Thang).ToListAsync());
        }

        // 2. CHI TIẾT
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var dienNuoc = await _context.DienNuocs
                .Include(d => d.MaPhongNavigation)
                .FirstOrDefaultAsync(m => m.MaDienNuoc == id);

            if (dienNuoc == null) return NotFound();
            return View(dienNuoc);
        }

        // 3. THÊM MỚI (Chỉ Admin / NhanVien được ghi chỉ số điện nước)
        [Authorize(Roles = "Admin,NhanVien")]
        public IActionResult Create()
        {
            // Tự động gán Tháng/Năm hiện tại
            var dienNuocMoi = new DienNuoc
            {
                Thang = DateTime.Now.Month,
                Nam = DateTime.Now.Year,
                DonGia = 3500 // Giá điện nước trung bình (bạn có thể tự sửa số này trên view)
            };

            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong");
            return View(dienNuocMoi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Create(DienNuoc dienNuoc)
        {
            ModelState.Remove("MaPhongNavigation");

            // KIỂM TRA LOGIC 1: Chỉ số cuối phải >= Chỉ số đầu
            if (dienNuoc.ChiSoCuoi < dienNuoc.ChiSoDau)
            {
                TempData["error"] = "Lỗi: Chỉ số cuối không được nhỏ hơn chỉ số đầu!";
                ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", dienNuoc.MaPhong);
                return View(dienNuoc);
            }

            // KIỂM TRA LOGIC 2: Tránh ghi trùng 2 lần 1 tháng cho 1 phòng
            var isExist = await _context.DienNuocs.AnyAsync(d => d.MaPhong == dienNuoc.MaPhong && d.Thang == dienNuoc.Thang && d.Nam == dienNuoc.Nam);
            if (isExist)
            {
                TempData["error"] = $"Phòng này đã được chốt chỉ số điện nước trong Tháng {dienNuoc.Thang}/{dienNuoc.Nam} rồi!";
                ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", dienNuoc.MaPhong);
                return View(dienNuoc);
            }

            if (ModelState.IsValid)
            {
                _context.Add(dienNuoc);
                await _context.SaveChangesAsync();
                TempData["success"] = "Đã chốt chỉ số điện nước thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", dienNuoc.MaPhong);
            return View(dienNuoc);
        }

        // 4. SỬA
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var dienNuoc = await _context.DienNuocs.FindAsync(id);
            if (dienNuoc == null) return NotFound();

            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", dienNuoc.MaPhong);
            return View(dienNuoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int id, DienNuoc dienNuoc)
        {
            if (id != dienNuoc.MaDienNuoc) return NotFound();

            ModelState.Remove("MaPhongNavigation");

            if (dienNuoc.ChiSoCuoi < dienNuoc.ChiSoDau)
            {
                TempData["error"] = "Lỗi: Chỉ số cuối không được nhỏ hơn chỉ số đầu!";
                ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", dienNuoc.MaPhong);
                return View(dienNuoc);
            }

            // Tránh trùng tháng (trừ chính nó ra)
            var isExist = await _context.DienNuocs.AnyAsync(d => d.MaPhong == dienNuoc.MaPhong && d.Thang == dienNuoc.Thang && d.Nam == dienNuoc.Nam && d.MaDienNuoc != id);
            if (isExist)
            {
                TempData["error"] = $"Phòng này đã được chốt số trong Tháng {dienNuoc.Thang}/{dienNuoc.Nam} rồi!";
                ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", dienNuoc.MaPhong);
                return View(dienNuoc);
            }

            if (ModelState.IsValid)
            {
                _context.Update(dienNuoc);
                await _context.SaveChangesAsync();
                TempData["success"] = "Cập nhật chỉ số thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", dienNuoc.MaPhong);
            return View(dienNuoc);
        }

        // 5. XÓA (Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var dienNuoc = await _context.DienNuocs
                .Include(d => d.MaPhongNavigation)
                .FirstOrDefaultAsync(m => m.MaDienNuoc == id);

            if (dienNuoc == null) return NotFound();
            return View(dienNuoc);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dienNuoc = await _context.DienNuocs.FindAsync(id);
            if (dienNuoc != null)
            {
                _context.DienNuocs.Remove(dienNuoc);
                await _context.SaveChangesAsync();
                TempData["success"] = "Đã xóa bản ghi điện nước!";
            }
            return RedirectToAction(nameof(Index));
        }
        // TÍNH NĂNG TỰ ĐỘNG LẤY CHỈ SỐ CUỐI THÁNG TRƯỚC
        [HttpGet]
        public async Task<IActionResult> GetChiSoCuoi(int maPhong)
        {
            // Tìm bản ghi gần nhất của phòng này (Sắp xếp theo Năm giảm dần, Tháng giảm dần)
            var banGhiCu = await _context.DienNuocs
                .Where(d => d.MaPhong == maPhong)
                .OrderByDescending(d => d.Nam)
                .ThenByDescending(d => d.Thang)
                .FirstOrDefaultAsync();

            // Nếu tìm thấy thì trả về Chỉ số cuối, nếu phòng mới toanh chưa có thì trả về 0
            int chiSoDauMoi = banGhiCu != null ? (banGhiCu.ChiSoCuoi ?? 0) : 0;

            return Json(chiSoDauMoi);
        }
    }
}