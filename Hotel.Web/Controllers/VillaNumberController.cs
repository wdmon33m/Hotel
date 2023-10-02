using Hotel.Application.Common.Interfaces;
using Hotel.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var villaNumbers = _unitOfWork.VillaNumber.GetAll(includeProperties:"Villa");
            return View(villaNumbers);
        }
        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Create(VillaNumberVM obj)
        {
            var villNumber = _unitOfWork.VillaNumber.Get(v => v.Villa_Number == obj.VillaNumber.Villa_Number);

            if (villNumber is not null)
            {
                TempData["error"] = "Villa Number Id is already exist!";

                VillaNumberVM villaNumberVM = new()
                {
                    VillaList = _unitOfWork.Villa.GetAll().Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    })
                };

                return View(villaNumberVM);
            }

            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            _unitOfWork.VillaNumber.Add(obj.VillaNumber);
            _unitOfWork.VillaNumber.Save();
            TempData["success"] = "Villa has been created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                VillaNumber = _unitOfWork.VillaNumber.Get(x => x.Villa_Number == villaNumberId)
            };

            if (villaNumberVM.VillaNumber is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Update(VillaNumberVM obj)
        {
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            _unitOfWork.VillaNumber.Update(obj.VillaNumber);
            _unitOfWork.VillaNumber.Save();
            TempData["success"] = "Villa has been updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                VillaNumber = _unitOfWork.VillaNumber.Get(x => x.Villa_Number == villaNumberId)
            };


            if (villaNumberVM is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Delete(VillaNumberVM obj)
        {
            _unitOfWork.VillaNumber.Remove(obj.VillaNumber);
            _unitOfWork.VillaNumber.Save();
            TempData["success"] = "Villa has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
