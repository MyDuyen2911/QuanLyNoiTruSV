using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNoiTruSV.Data;
using QuanLyNoiTruSV.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyNoiTruSV.Controllers
{
    public class KhuNoiTruController : Controller
    {
        private readonly QuanLyNoiTruSVContext _context;

        public KhuNoiTruController(QuanLyNoiTruSVContext context)
        {
            _context = context;
        }

        // LIST + SEARCH
        public async Task<IActionResult> Index(string searchString)
        {
            var khus = from k in _context.KhuNoiTrus
                       select k;

            if (!string.IsNullOrEmpty(searchString))
            {
                khus = khus.Where(k => k.TenKhu.Contains(searchString)
                                    || k.DiaChi.Contains(searchString));
            }

            return View(await khus.ToListAsync());
        }

        // CREATE
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhuNoiTru khu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(khu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(khu);
        }

        // EDIT
        public async Task<IActionResult> Edit(int id)
        {
            var khu = await _context.KhuNoiTrus.FindAsync(id);
            if (khu == null) return NotFound();
            return View(khu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KhuNoiTru khu)
        {
            if (id != khu.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(khu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(khu);
        }

        // DELETE
        public async Task<IActionResult> Delete(int id)
        {
            var khu = await _context.KhuNoiTrus.FindAsync(id);
            if (khu == null) return NotFound();
            return View(khu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khu = await _context.KhuNoiTrus.FindAsync(id);
            _context.KhuNoiTrus.Remove(khu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // PRINT
        public async Task<IActionResult> Print()
        {
            var list = await _context.KhuNoiTrus.ToListAsync();
            return View(list);
        }
        // DETAIL
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0) return NotFound();

            var khu = await _context.KhuNoiTrus
                .FirstOrDefaultAsync(m => m.Id == id);

            if (khu == null) return NotFound();

            return View(khu);
        }
    }
}