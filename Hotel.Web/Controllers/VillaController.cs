using Hotel.Domain.Entities;
using Hotel.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly ApplicationDbContext _db;

        public VillaController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var villas = _db.Villas.ToList();
            return View(villas);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Villa obj)
        {
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            _db.Villas.Add(obj);
            _db.SaveChanges();
            TempData["success"] = "Villa has been created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int villaId)
        {
            Villa? obj = _db.Villas.FirstOrDefault(x => x.Id == villaId);
            
            if (obj == null)
            {
                return RedirectToAction("Error","Home");
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Update(Villa obj)
        {
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            _db.Update(obj);
            _db.SaveChanges();
            TempData["success"] = "Villa has been updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int villaId)
        {
            Villa? obj = _db.Villas.FirstOrDefault(x => x.Id == villaId);

            if (obj == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Delete(Villa obj)
        {
            _db.Remove(obj);
            _db.SaveChanges();
            TempData["error"] = "Villa has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
