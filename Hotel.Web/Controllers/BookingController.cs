using Hotel.Application.Common.Interfaces;
using Hotel.Application.Dto;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace Hotel.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ResponseDto _response;

        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new();
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult FinalizeBooking(int villaId,DateOnly checkInDate,int nights)
        {
            var cliamIdentity = (ClaimsIdentity)User.Identity;
            var userId = cliamIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            
            ApplicationUser user = _unitOfWork.User.Get(u => u.Id == userId);
            Booking booking = new()
            {
                Villa = _unitOfWork.Villa.Get(u => u.Id ==  villaId,includeProperties:"VillaAmenity"),
                VillaId = villaId,
                Nights = nights,
                CheckInDate = checkInDate.ToDateTimeOnly(), 
                CheckOutDate = checkInDate.AddDays(nights).ToDateTimeOnly(),
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BookingDate = DateTime.Now,
                UserId = userId,
            };

            booking.CheckInDate = checkInDate.ToDateTimeOnly();
            
            booking.TotalCost = booking.Villa.Price * nights;
            return View(booking);
        }

        [Authorize]
        [HttpPost]
        public IActionResult FinalizeBooking(Booking booking)
        {
            var villa = _unitOfWork.Villa.Get(v => v.Id == booking.VillaId);
            
            booking.TotalCost = villa.Price * booking.Nights;
            booking.Status = SD.Status_Pending;
            booking.BookingDate = DateTime.Now;

            _unitOfWork.Booking.Add(booking);
            _unitOfWork.Save();


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

            _unitOfWork.Booking.UpdateStripePaymentId(booking.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {
            Booking bookingFromDb = _unitOfWork.Booking.Get(b => b.Id == bookingId,includeProperties:"User,Villa");

            if (bookingFromDb.Status == SD.Status_Pending)
            {
                // this is a pending order, we need to confirm if payment was ssuccessfull

                var service = new SessionService();
                Session session = service.Get(bookingFromDb.StripeSessionId);

                if (session.PaymentStatus == "paid")
                {
                    _unitOfWork.Booking.UpdateStatus(bookingId, SD.Status_Approved);
                    _unitOfWork.Booking.UpdateStripePaymentId(bookingFromDb.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                }
            }
            return View(bookingId);
        }
    }
}
