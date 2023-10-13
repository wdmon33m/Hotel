using Hotel.Application.Common.Interfaces;
using Hotel.Application.Services.Interface;
using Hotel.Application.Utility;
using Hotel.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVillaService _villaService;
        public HomeController(IUnitOfWork unitOfWork, IVillaService villaService)
        {
            _unitOfWork = unitOfWork;
            _villaService = villaService;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity"),
                Nights = 1,
                CheckInDate = DateTime.Now.ToDateOnly()
            };
            return View(homeVM);
        }
        [HttpPost]
        public IActionResult Index(HomeVM homeVM)
        {
            homeVM.VillaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity");

            foreach (var villa in homeVM.VillaList)
            {
                if (villa.Id %2 == 0)
                {
                    villa.IsAvailable = false;
                }
            }

            return View(homeVM);
        }

        public IActionResult GetVillasByDate(int nights, DateOnly checkInDate)
        {
            var villaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity");

            foreach (var villa in villaList)
            {
                villa.IsAvailable = _villaService.IsVillaAvailble(villa.Id, checkInDate, nights);
            }

            HomeVM homeVM = new()
            {   
                CheckInDate = checkInDate,
                Nights = nights,
                VillaList = villaList
            };
            return PartialView("_VillaList", homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}