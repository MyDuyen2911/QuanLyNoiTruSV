using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using QL_NOITRU_DAKHU.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly QlNoitruDakhuContext _context;

    public AccountController(QlNoitruDakhuContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        // Nếu đã đăng nhập rồi thì không cho vào trang Login nữa, đẩy về trang chủ luôn
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        // Tìm tài khoản khớp Username và Password
        var user = _context.TaiKhoans
            .FirstOrDefault(x => x.Username == username && x.PasswordHash == password);

        if (user == null)
        {
            ViewBag.Error = "❌ Sai tài khoản hoặc mật khẩu!";
            return View();
        }

        // Tạo danh sách "thẻ căn cước" (Claims) cho người dùng
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.MaTaiKhoan.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.VaiTro) // Quan trọng để phân biệt Admin/NhanVien/SinhVien
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // Tiến hành đăng nhập (Ghi Cookie vào trình duyệt)
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true } // Nhớ trạng thái đăng nhập
        );

        // ================= PHÂN LUỒNG ĐIỀU HƯỚNG CỰC CHUẨN =================

        // ĐIỀU HƯỚNG SAU KHI ĐĂNG NHẬP THÀNH CÔNG
        if (user.VaiTro == "Admin")
        {
            // Admin thì vào Dashboard tổng quát để xem Thống kê - Báo cáo
            return RedirectToAction("Index", "Admin");
        }
        else if (user.VaiTro == "NhanVien")
        {
            // Nhân viên thì vào trang cá nhân hoặc trang quản lý nghiệp vụ
            return RedirectToAction("MyProfile", "NhanViens");
        }
        else if (user.VaiTro == "SinhVien")
        {
            // Sinh viên thì vào trang cá nhân của họ
            return RedirectToAction("MyProfile", "QLSinhViens");
        }

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}