using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;
using System.Security.Claims;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize]
    public class SuaChuasController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public SuaChuasController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH SỬA CHỮA
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.SuaChuas
                .Include(s => s.MaPhongNavigation)
                .Include(s => s.MaTaiSanNavigation)
                .AsQueryable();

            if (User.IsInRole("SinhVien"))
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdClaim, out int userId))
                {
                    var hopDong = await _context.HopDongs
                        .FirstOrDefaultAsync(h => h.MaSvNavigation.MaTaiKhoan == userId && h.TrangThai == "Đang ở");

                    if (hopDong != null)
                    {
                        query = query.Where(s => s.MaPhong == hopDong.MaPhong);
                    }
                    else
                    {
                        query = query.Where(s => s.MaPhong == -1);
                    }
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.MaPhongNavigation.TenPhong.Contains(search) ||
                                         s.MaTaiSanNavigation.TenTaiSan.Contains(search));
            }

            ViewBag.CurrentSearch = search;
            return View(await query.OrderByDescending(s => s.NgayBaoHong).ToListAsync());
        }

        // 2. CHI TIẾT
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var suaChua = await _context.SuaChuas
                .Include(s => s.MaPhongNavigation)
                .Include(s => s.MaTaiSanNavigation)
                .FirstOrDefaultAsync(m => m.MaSuaChua == id);

            if (suaChua == null) return NotFound();
            return View(suaChua);
        }

        // 3. THÊM MỚI (Báo hỏng)
        [Authorize(Roles = "SinhVien")]
        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("SinhVien"))
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdClaim, out int userId))
                {
                    var hopDong = await _context.HopDongs
                        .FirstOrDefaultAsync(h => h.MaSvNavigation.MaTaiKhoan == userId && h.TrangThai == "Đang ở");

                    if (hopDong != null)
                    {
                        ViewData["MaPhong"] = new SelectList(_context.Phongs.Where(p => p.MaPhong == hopDong.MaPhong), "MaPhong", "TenPhong");
                    }
                    else
                    {
                        ViewData["MaPhong"] = new SelectList(new List<Phong>(), "MaPhong", "TenPhong");
                    }
                }
            }
            else
            {
                ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong");
            }

            ViewData["MaTaiSan"] = new SelectList(_context.TaiSans, "MaTaiSan", "TenTaiSan");

            var newSuaChua = new SuaChua
            {
                NgayBaoHong = DateOnly.FromDateTime(DateTime.Now),
                TrangThai = "Chưa sửa" // Mình tạm đoán SQL của bạn đang yêu cầu chữ này
            };
            return View(newSuaChua);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SinhVien")]
        public async Task<IActionResult> Create(SuaChua suaChua)
        {
            ModelState.Remove("MaPhongNavigation");
            ModelState.Remove("MaTaiSanNavigation");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(suaChua);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Gửi báo cáo hỏng hóc thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // IN LỖI DATABASE RA MÀN HÌNH ĐỂ BẠN THẤY
                    TempData["error"] = "Lỗi Database (Thường do sai chữ ở Trạng thái). Chi tiết: " + (ex.InnerException?.Message ?? ex.Message);
                }
            }
            else
            {
                TempData["error"] = "Dữ liệu nhập vào chưa hợp lệ, vui lòng kiểm tra lại ngày tháng.";
            }

            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", suaChua.MaPhong);
            ViewData["MaTaiSan"] = new SelectList(_context.TaiSans, "MaTaiSan", "TenTaiSan", suaChua.MaTaiSan);
            return View(suaChua);
        }

        // 4. SỬA (Cập nhật tiến độ)
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var suaChua = await _context.SuaChuas.FindAsync(id);
            if (suaChua == null) return NotFound();

            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", suaChua.MaPhong);
            ViewData["MaTaiSan"] = new SelectList(_context.TaiSans, "MaTaiSan", "TenTaiSan", suaChua.MaTaiSan);
            return View(suaChua);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Edit(int id, SuaChua suaChua)
        {
            if (id != suaChua.MaSuaChua) return NotFound();

            ModelState.Remove("MaPhongNavigation");
            ModelState.Remove("MaTaiSanNavigation");

            if (suaChua.TrangThai == "Đã hoàn thành" && suaChua.NgayHoanThanh == null)
            {
                suaChua.NgayHoanThanh = DateOnly.FromDateTime(DateTime.Now);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(suaChua);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Cập nhật trạng thái sửa chữa thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["error"] = "Lỗi Database (Thường do sai chữ ở Trạng thái). Chi tiết: " + (ex.InnerException?.Message ?? ex.Message);
                }
            }

            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "TenPhong", suaChua.MaPhong);
            ViewData["MaTaiSan"] = new SelectList(_context.TaiSans, "MaTaiSan", "TenTaiSan", suaChua.MaTaiSan);
            return View(suaChua);
        }

        // 5. XÓA (Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var suaChua = await _context.SuaChuas
                .Include(s => s.MaPhongNavigation)
                .Include(s => s.MaTaiSanNavigation)
                .FirstOrDefaultAsync(m => m.MaSuaChua == id);

            if (suaChua == null) return NotFound();
            return View(suaChua);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var suaChua = await _context.SuaChuas.FindAsync(id);
            if (suaChua != null)
            {
                _context.SuaChuas.Remove(suaChua);
                await _context.SaveChangesAsync();
                TempData["success"] = "Đã xóa phiếu sửa chữa!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}