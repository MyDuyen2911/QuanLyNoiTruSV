using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml; // Thư viện EPPlus đọc Excel
using QL_NOITRU_DAKHU.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")]
    public class HocLucsController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public HocLucsController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH HỌC LỰC (Đã được áp dụng giảm giá)
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.HocLucs
                .Include(h => h.MaSvNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(h => h.MaSvNavigation.Mssv.Contains(search) ||
                                         h.MaSvNavigation.HoTen.Contains(search) ||
                                         h.HocKy.Contains(search));
            }

            ViewBag.CurrentSearch = search;
            return View(await query.OrderByDescending(h => h.MaHocLuc).ToListAsync());
        }

        // 2. IMPORT EXCEL (Thay thế cho Create gõ tay)
        public IActionResult ImportExcel()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportExcel(IFormFile fileUpload)
        {
            if (fileUpload == null || fileUpload.Length <= 0)
            {
                TempData["error"] = "Vui lòng chọn một file Excel (.xlsx) để tải lên!";
                return View();
            }

            if (!Path.GetExtension(fileUpload.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                TempData["error"] = "Hệ thống chỉ hỗ trợ định dạng file Excel (.xlsx).";
                return View();
            }

            // Bắt buộc khai báo bản quyền sử dụng thư viện EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            int countSuccess = 0;
            int countSkip = 0;

            try
            {
                using (var stream = new MemoryStream())
                {
                    await fileUpload.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null) throw new Exception("File Excel không có dữ liệu.");

                        int rowCount = worksheet.Dimension.Rows;

                        // Bắt đầu đọc từ dòng 2 (Bỏ qua dòng Tiêu đề)
                        for (int row = 2; row <= rowCount; row++)
                        {
                            var mssv = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                            var hocKy = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                            var xepLoai = worksheet.Cells[row, 3].Value?.ToString()?.Trim();

                            if (string.IsNullOrEmpty(mssv) || string.IsNullOrEmpty(xepLoai)) continue;

                            // 1. Tìm Sinh Viên theo MSSV
                            var sv = await _context.SinhViens.FirstOrDefaultAsync(s => s.Mssv == mssv);

                            if (sv != null)
                            {
                                // 2. Kiểm tra xem Sinh viên này có đang ở KTX không?
                                bool isDangO = await _context.HopDongs
                                    .AnyAsync(h => h.MaSv == sv.MaSv && h.TrangThai == "Đang ở");

                                // 3. Nếu ĐANG Ở và có Xếp loại Giỏi/Xuất sắc -> Áp dụng giảm giá
                                if (isDangO && (xepLoai == "Giỏi" || xepLoai == "Xuất sắc"))
                                {
                                    // Kiểm tra xem kỳ này đã add học lực chưa để tránh add trùng
                                    bool isExist = await _context.HocLucs.AnyAsync(h => h.MaSv == sv.MaSv && h.HocKy == hocKy);

                                    if (!isExist)
                                    {
                                        decimal tiLe = (xepLoai == "Xuất sắc") ? 20.00m : 10.00m; // Ví dụ: XS giảm 20%, Giỏi giảm 10%

                                        var hocLucMoi = new HocLuc
                                        {
                                            MaSv = sv.MaSv,
                                            HocKy = hocKy ?? "Kỳ hiện tại",
                                            XepLoai = xepLoai,
                                            TyLeGiam = tiLe
                                        };

                                        _context.HocLucs.Add(hocLucMoi);
                                        countSuccess++;
                                    }
                                    else { countSkip++; }
                                }
                                else { countSkip++; } // Không ở KTX hoặc Không đủ loại Giỏi/XS
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                TempData["success"] = $"Import thành công! Đã thêm {countSuccess} sinh viên đạt chuẩn giảm giá. Bỏ qua {countSkip} sinh viên (không ở KTX, không đủ điểm, hoặc đã có tên).";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = "Có lỗi xảy ra khi đọc file Excel: " + ex.Message;
                return View();
            }
        }

        // 3. XÓA (Thu hồi giảm giá)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var hocLuc = await _context.HocLucs
                .Include(h => h.MaSvNavigation)
                .FirstOrDefaultAsync(m => m.MaHocLuc == id);

            if (hocLuc == null) return NotFound();
            return View(hocLuc);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hocLuc = await _context.HocLucs.FindAsync(id);
            if (hocLuc != null)
            {
                _context.HocLucs.Remove(hocLuc);
                await _context.SaveChangesAsync();
                TempData["success"] = "Đã thu hồi ưu đãi giảm giá!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}