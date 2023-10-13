using Hotel.Application.Services.Interface;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Web.Controllers
{
    [Authorize]
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VillaController(IVillaService villaService, IWebHostEnvironment webHostEnvironment)
        {
            _villaService = villaService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var villas = _villaService.GetAllVillas();
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

            var response = _villaService.CreateVilla(obj, _webHostEnvironment.WebRootPath);

            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Villa has been created successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = response?.ErrorMessage;
            return View(obj);
        }

        public IActionResult Update(int villaId)
        {
            Villa? obj = _villaService.GetVilla(villaId);
            
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

            var response = _villaService.UpdateVilla(obj, _webHostEnvironment.WebRootPath);

            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Villa has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = response?.ErrorMessage;
            return View(obj);
        }

        public IActionResult Delete(int villaId)
        {
            Villa? obj = _villaService.GetVilla(villaId);

            if (obj == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(obj);
        }

        [HttpPost]
        public IActionResult Delete(Villa obj)
        {

            var response = _villaService.DeleteVilla(obj.Id, _webHostEnvironment.WebRootPath);

            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Villa has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = response?.ErrorMessage;
            return View(obj);
        }
    }
}
