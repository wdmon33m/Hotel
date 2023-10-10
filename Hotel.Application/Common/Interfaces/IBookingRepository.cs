using Hotel.Application.Dto;
using Hotel.Domain.Entities;

namespace Hotel.Application.Common.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        ResponseDto Update(Booking entity);
        ResponseDto UpdateStatus(int bookingId, string bookingStatus);
        ResponseDto UpdateStripePaymentId(int bookingId, string sessionId, string paymentIntentId);
    }
}
