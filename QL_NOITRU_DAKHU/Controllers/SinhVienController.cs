using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_NOITRU_DAKHU.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QL_NOITRU_DAKHU.Controllers
{
    // Cấp quyền cho Sinh viên vào đây
    [Authorize]
    public class SinhVienController : Controller
    {
        private readonly QlNoitruDakhuContext _context;

        // Bắt buộc phải có hàm tạo (Constructor) này để bơm Database vào
        public SinhVienController(QlNoitruDakhuContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Các đoạn code đếm số lượng, ViewBag... bạn phải để ở TRONG NÀY nhé
            // Ví dụ:
            // int soLuong = _context.DonDangKies.Count(); 
            // ViewBag.ThongBao = "Xin chào"; 

            return View();
        }
    }
}