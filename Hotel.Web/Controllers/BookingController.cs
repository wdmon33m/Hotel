using Hotel.Application.Common.Dto;
using Hotel.Application.Common.Interfaces;
using Hotel.Application.Services.Interface;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace Hotel.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookingService _bookingService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BookingController(IVillaNumberService villaNumberService, 
            IVillaService villaService, UserManager<ApplicationUser> userManager, 
            IBookingService bookingService, IWebHostEnvironment webHostEnvironment)
        {
            _villaNumberService = villaNumberService;
            _villaService = villaService;
            _userManager = userManager;
            _bookingService = bookingService;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult FinalizeBooking(int villaId, DateOnly checkInDate, int nights)
        {
            var cliamIdentity = (ClaimsIdentity)User.Identity;
            var userId = cliamIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser user = _userManager.FindByIdAsync(userId).GetAwaiter().GetResult();

            Booking booking = new()
            {
                Villa = _villaService.GetVilla(villaId),
                VillaId = villaId,
                Nights = nights,
                CheckInDate = checkInDate,
                CheckOutDate = checkInDate.AddDays(nights),
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BookingDate = DateTime.Now.ToDateOnly(),
                UserId = userId,
            };

            booking.TotalCost = booking.Villa.Price * nights;
            return View(booking);
        }

        [Authorize]
        [HttpPost]
        public IActionResult FinalizeBooking(Booking booking)
        {
            var villa = _villaService.GetVilla(booking.VillaId);

            booking.TotalCost = villa.Price * booking.Nights;
            booking.Status = SD.Status_Pending;

            villa.IsAvailable = _villaService.IsVillaAvailble(villa.Id, booking.CheckInDate, booking.Nights);

            if (!villa.IsAvailable)
            {
                TempData["error"] = "Room has been sold out";
                //No room available
                return RedirectToAction(nameof(FinalizeBooking), new
                {
                    villaId = booking.VillaId,
                    checkInDate = booking.CheckInDate,
                    nights = booking.Nights
                });
            }

            var response = _bookingService.CreateBooking(booking);
            if (response is null || !response.IsSuccess)
            {
                TempData["error"] = response?.ErrorMessage;
                return RedirectToAction(nameof(FinalizeBooking), new { villaId = villa.Id, checkInDate = booking.CheckInDate, nights = booking .Nights});
            }


            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"booking/BookingConfirmation?bookingId={booking.Id}",
                CancelUrl = domain + $"booking/FinalizeBooking?villaId={booking.VillaId}&checkInDate={booking.CheckInDate}&nights={booking.Nights}",
            };

            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions()
                {
                    UnitAmountDecimal = booking.TotalCost * 100,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = villa.Name,
                        Description = villa.Description,
                    }
                },
                Quantity = 1,
            });

            var service = new SessionService();
            Session session = service.Create(options);

            response = _bookingService.UpdateStripePaymentId(booking.Id, session.Id, session.PaymentIntentId);
            if (response is null || !response.IsSuccess)
            {
                TempData["error"] = response?.ErrorMessage;
            }

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {
            Booking bookingFromDb = _bookingService.GetBookingById(bookingId);

            if (bookingFromDb.Status == SD.Status_Pending)
            {
                // this is a pending order, we need to confirm if payment was ssuccessfull

                var service = new SessionService();
                Session session = service.Get(bookingFromDb.StripeSessionId);

                if (session.PaymentStatus == "paid")
                {
                    _bookingService.UpdateStatus(bookingId, SD.Status_Approved);
                    _bookingService.UpdateStripePaymentId(bookingFromDb.Id, session.Id, session.PaymentIntentId);
                }
            }
            return View(bookingId);
        }

        [Authorize]
        public IActionResult BookingDetails(int bookingId)
        {
            Booking bookingFromDb = _bookingService.GetBookingById(bookingId);

            if (bookingFromDb.VillaNumber == 0 && bookingFromDb.Status == SD.Status_Approved)
            {
                var availableRooms = AvailabeRoomsByVilla(bookingFromDb.VillaId);

                bookingFromDb.VillaNumbers = _villaNumberService.GetAllVillaNumbers().Where(u => u.VillaId == bookingFromDb.VillaId
                && availableRooms.Any(x => x == u.Villa_Number)).ToList();
            }

            return View(bookingFromDb);
        }

        private List<int> AvailabeRoomsByVilla(int villaId)
        {
            List<int> availableVillaNumbers = new();

            var villaNumbers = _villaNumberService.GetAllVillaNumbers().Where(u => u.VillaId == villaId);

            var checkInVilla = _bookingService.GetCheckedInVillaNumbers(villaId);

            foreach (var villaNumber in villaNumbers)
            {
                if (!checkInVilla.Contains(villaNumber.Villa_Number))
                {
                    availableVillaNumbers.Add(villaNumber.Villa_Number);
                }
            }
            return availableVillaNumbers;
        }

        [HttpPost]
        public IActionResult CheckIn(Booking booking)
        {
            booking.ActualCheckInDate = DateTime.Now;
            _bookingService.UpdateStatus(booking.Id, SD.Status_CheckIn, booking.VillaNumber);

            TempData["success"] = "Booking updated successfully.";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        public IActionResult CheckOut(Booking booking)
        {
            booking.ActualCheckOutDate = DateTime.Now;
            _bookingService.UpdateStatus(booking.Id, SD.Status_Completed, booking.VillaNumber);

            TempData["success"] = "Booking completed  successfully.";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        public IActionResult CancelBooking(Booking booking)
        {
            _bookingService.UpdateStatus(booking.Id, SD.Status_Cancelled);

            TempData["success"] = "Booking cancelled successfully.";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }
        #region API Calls
        [Authorize]
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Booking> objBookings;
            string userId = "";

            if (string.IsNullOrEmpty(status))
            {
                status = "";
            }

            if (!User.IsInRole(SD.Role_Admin))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            }
            objBookings = _bookingService.GetAllBookings(userId, status);
            
            return Json(new { data = objBookings });
        }
        #endregion
    }
}
