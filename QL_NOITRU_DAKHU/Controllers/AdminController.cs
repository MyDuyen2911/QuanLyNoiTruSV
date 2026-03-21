using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;
using System.Threading.Tasks;

namespace QL_NOITRU_DAKHU.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")] // Đảm bảo chỉ Admin/NV mới vô được trang này
    public class AdminController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        // Bơm Database vào Controller
        public AdminController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. ĐẾM CÁC YÊU CẦU ĐANG CHỜ XỬ LÝ
            int donDangKyCho = await _context.DonDangKies.CountAsync(d => d.TrangThai == "Chờ duyệt");
            int chuyenPhongCho = await _context.YeuCauChuyenPhongs.CountAsync(y => y.TrangThai == "Chờ duyệt");
            int chinhSuaCho = await _context.YeuCauChinhSuas.CountAsync(y => y.TrangThai == "Chờ duyệt");
            int suaChuaCho = await _context.SuaChuas.CountAsync(s => s.TrangThai == "Chưa sửa" || s.TrangThai == "Chờ xử lý");

            int tongChoDuyet = donDangKyCho + chuyenPhongCho + chinhSuaCho + suaChuaCho;

            // 2. GỬI SỐ LIỆU SANG VIEW
            ViewBag.DonDangKyCho = donDangKyCho;
            ViewBag.ChuyenPhongCho = chuyenPhongCho;
            ViewBag.ChinhSuaCho = chinhSuaCho;
            ViewBag.SuaChuaCho = suaChuaCho;
            ViewBag.TongChoDuyet = tongChoDuyet;

            // NẾU TRƯỚC ĐÓ BẠN CÓ CODE ĐẾM TỔNG SINH VIÊN, PHÒNG TRỐNG... 
            // THÌ BẠN CỨ VIẾT TIẾP Ở ĐÂY NHÉ (mình để code mẫu nếu bạn chưa có):
            // ViewBag.TongSinhVien = await _context.SinhViens.CountAsync();
            // ViewBag.PhongTrong = await _context.Phongs.CountAsync(p => p.TrangThai == "Trống");

            return View();
        }
    }
}