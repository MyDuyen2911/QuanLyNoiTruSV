using Microsoft.AspNetCore.Mvc;

namespace QuanLyNoiTruSV.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
