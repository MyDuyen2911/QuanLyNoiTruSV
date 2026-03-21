using Microsoft.AspNetCore.Mvc;
using QL_NOITRU_DAKHU.Models;

public class HomeController : Controller
{
    private readonly QlNoitruDakhuContext _context;

    public HomeController(QlNoitruDakhuContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        ViewBag.TotalSinhVien = _context.SinhViens.Count();
        ViewBag.TotalKhu = _context.Khus.Count();
        ViewBag.TotalPhong = _context.Phongs.Count();

        return View();
    }
}