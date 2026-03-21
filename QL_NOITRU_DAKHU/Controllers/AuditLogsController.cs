using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QL_NOITRU_DAKHU.Controllers
{
    // BẢO MẬT: CHỈ DUY NHẤT ADMIN MỚI ĐƯỢC XEM NHẬT KÝ HỆ THỐNG
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        public AuditLogsController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH LỊCH SỬ HOẠT ĐỘNG
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.AuditLogs
                .Include(a => a.MaTaiKhoanNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a =>
                    (a.MaTaiKhoanNavigation != null && a.MaTaiKhoanNavigation.Username.Contains(search)) ||
                    a.HanhDong.Contains(search) ||
                    a.MoTa.Contains(search));
            }

            ViewBag.CurrentSearch = search;

            // Sắp xếp mới nhất lên đầu tiên
            return View(await query.OrderByDescending(a => a.ThoiGian).ToListAsync());
        }

        // 2. CHI TIẾT 1 DÒNG LOG
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var auditLog = await _context.AuditLogs
                .Include(a => a.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.MaLog == id);

            if (auditLog == null) return NotFound();

            return View(auditLog);
        }

        // LƯU Ý: CỐ TÌNH KHÔNG VIẾT HÀM CREATE, EDIT, DELETE VÌ AUDIT LOG KHÔNG ĐƯỢC PHÉP CHỈNH SỬA!
    }
}