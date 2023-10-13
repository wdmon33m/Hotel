using Hotel.Application.Common.Dto;
using Hotel.Domain.Entities;

namespace Hotel.Application.Services.Interface
{
    public interface IBookingService
    {
        ResponseDto CreateBooking(Booking entity);
        Booking GetBookingById(int bookingId);
        IEnumerable<Booking> GetAllBookings(string userId = "", string? statusFillterList="");
        ResponseDto UpdateStatus(int bookingId, string bookingStatus, int villaNumber = 0);
        ResponseDto UpdateStripePaymentId(int bookingId, string sessionId, string paymentIntentId);
        public IEnumerable<int> GetCheckedInVillaNumbers(int villaId);
    }
}
