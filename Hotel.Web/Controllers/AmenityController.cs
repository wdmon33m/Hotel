using Hotel.Application.Common.Interfaces;
using Hotel.Application.Utility;
using Hotel.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel.Web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AmenityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var obj = _unitOfWork.Amenity.GetAll(includeProperties: "Villa");
            return View(obj);
        }

        public IActionResult Create()
        {
            AmenityVM amenityVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Create(AmenityVM obj)
        {
            AmenityVM amenityVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                Amenity = _unitOfWork.Amenity.Get(v => v.Id == obj.Amenity.Id)
            };

            if (amenityVM.Amenity is not null)
            {
                TempData["error"] = "Amenity Id is already exist!";
                return View(amenityVM);
            }

            if (!ModelState.IsValid)
            {
                return View(amenityVM);
            }

            _unitOfWork.Amenity.Add(obj.Amenity);
            _unitOfWork.Save();
            TempData["success"] = "Amenity has been created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int Id)
        {
            AmenityVM obj = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                Amenity = _unitOfWork.Amenity.Get(v => v.Id == Id)
            };

            if (obj.Amenity is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Update(AmenityVM obj)
        {
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            _unitOfWork.Amenity.Update(obj.Amenity);
            _unitOfWork.Save();
            TempData["success"] = "Amenity has been updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int Id)
        {
            AmenityVM obj = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                Amenity = _unitOfWork.Amenity.Get(v => v.Id == Id)
            };


            if (obj.Amenity is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Delete(AmenityVM obj)
        {
            _unitOfWork.Amenity.Remove(obj.Amenity);
            _unitOfWork.Save();
            TempData["success"] = "Amenity has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
