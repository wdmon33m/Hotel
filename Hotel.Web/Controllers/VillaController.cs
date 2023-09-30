using Hotel.Application.Common.Interfaces;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public VillaController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var villas = _unitOfWork.Villa.GetAll();
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

            _unitOfWork.Villa.Add(obj);
            _unitOfWork.Villa.Save();
            TempData["success"] = "Villa has been created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int villaId)
        {
            Villa? obj = _unitOfWork.Villa.Get(x => x.Id == villaId);
            
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

            _unitOfWork.Villa.Update(obj);
            _unitOfWork.Villa.Save();
            TempData["success"] = "Villa has been updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int villaId)
        {
            Villa? obj = _unitOfWork.Villa.Get(x => x.Id == villaId);

            if (obj == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Delete(Villa obj)
        {
            _unitOfWork.Villa.Remove(obj);
            _unitOfWork.Villa.Save();
            TempData["success"] = "Villa has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
