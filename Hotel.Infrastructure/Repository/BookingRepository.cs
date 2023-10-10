using Hotel.Application.Common.Interfaces;
using Hotel.Application.Dto;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Hotel.Infrastructure.Data;

namespace Hotel.Infrastructure.Repository
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ResponseDto _response;
        public BookingRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
            _response = new();
        }

        public ResponseDto Update(Booking entity)
        {
            try
            {
                _response.Result = _db.Update(entity);
                return _response;
            }
            catch (Exception ex)
            {
                return _response.Exception(ex.Message);
            }
        }

        public ResponseDto UpdateStatus(int bookingId, string bookingStatus)
        {
            try
            {
                var bookingFromDb = _db.Bookings.FirstOrDefault(m => m.Id == bookingId);

                if (bookingFromDb == null)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessage = "Booking Id is not found";
                    return _response;
                }

                bookingFromDb.Status = bookingStatus;
                if (bookingStatus == SD.Status_CheckIn)
                {
                    bookingFromDb.ActualCheckInDate = DateTime.Now;
                }
                if (bookingStatus == SD.Status_Completed)
                {
                    bookingFromDb.ActualCheckOutDate = DateTime.Now;
                }

                return _response;
            }
            catch (Exception ex)
            {
                return _response.Exception(ex.Message);
            }
        }

        public ResponseDto UpdateStripePaymentId(int bookingId, string sessionId, string paymentIntentId)
        {
            try
            {
                var bookingFromDb = _db.Bookings.FirstOrDefault(m => m.Id == bookingId);
                if (bookingFromDb == null)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessage = "Booking Id is not found";
                    return _response;
                }

                if (!string.IsNullOrEmpty(sessionId))
                {
                    bookingFromDb.StripeSessionId = sessionId;
                }

                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    bookingFromDb.StripePaymentIntendId = paymentIntentId;
                    bookingFromDb.PaymentDate = DateTime.Now;
                    bookingFromDb.IsPaymentSuccessfull = true;
                }

                return _response;
            }
            catch (Exception ex)
            {
                return _response.Exception(ex.Message);
            }
        }
    }
}
