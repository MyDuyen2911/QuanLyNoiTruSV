using Microsoft.AspNetCore.Mvc;
using QuanLyNoiTruSV.Data;
using System.Linq;

namespace QuanLyNoiTruSV.Controllers
{
    public class AuthController : Controller
    {
        private readonly QuanLyNoiTruSVContext _context;

        public AuthController(QuanLyNoiTruSVContext context)
        {
            _context = context;
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            var user = _context.NguoiDungs
                .FirstOrDefault(u => u.Email.Trim() == email.Trim()
                                  && u.MatKhau.Trim() == password.Trim());

            if (user != null)
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.VaiTro);

                if (user.VaiTro == "Admin")
                    return RedirectToAction("Index", "Admin");

                if (user.VaiTro == "NhanVien")
                    return RedirectToAction("Index", "NhanVien");

                if (user.VaiTro == "SinhVien")
                    return RedirectToAction("Index", "SinhVien");
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}