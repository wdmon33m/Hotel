using Hotel.Application.Common.Dto;
using Hotel.Application.Common.Interfaces;
using Hotel.Application.Services.Interface;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;

namespace Hotel.Application.Services.Implementation
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ResponseDto _response;

        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new();
        }
        public ResponseDto CreateBooking(Booking entity)
        {
            try
            {
                entity.BookingDate = DateTime.Now.ToDateOnly();
                _unitOfWork.Booking.Add(entity);
                _unitOfWork.Save();

                return _response;
            }
            catch (Exception ex)
            {
                return _response.Exception(ex.Message);
            }
        }

        public IEnumerable<Booking> GetAllBookings(string userId = "", string? statusFillterList = "")
        {
            IEnumerable<string>? statusList = statusFillterList?.ToLower().Split(",");
            if (!string.IsNullOrEmpty(statusFillterList) && !string.IsNullOrEmpty(userId))
            {
                return _unitOfWork.Booking.GetAll(u => statusList.Contains(u.Status.ToLower()) &&
                                                   u.UserId == userId, includeProperties: "User,Villa");
            }
            else
            {
                if (!string.IsNullOrEmpty(statusFillterList))
                {
                    return _unitOfWork.Booking.GetAll(u => statusList.Contains(u.Status.ToLower()), includeProperties: "User,Villa");
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    return _unitOfWork.Booking.GetAll(u => u.UserId == userId, includeProperties: "User,Villa");
                }
            }
            return _unitOfWork.Booking.GetAll(includeProperties: "User,Villa");
        }

        public Booking GetBookingById(int bookingId)
        {
            return _unitOfWork.Booking.Get(u => u.Id == bookingId , includeProperties: "User,Villa");
        }

        public IEnumerable<int> GetCheckedInVillaNumbers(int villaId)
        {
            return  _unitOfWork.Booking.GetAll(u => u.VillaId == villaId
                                && u.Status == SD.Status_CheckIn)
                                .Select(u => u.VillaNumber);
        }

        public ResponseDto UpdateStatus(int bookingId, string bookingStatus, int villaNumber = 0)
        {
            try
            {
                var bookingFromDb = _unitOfWork.Booking.Get(m => m.Id == bookingId, tracked: true);

                if (bookingFromDb == null)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessage = "Booking Id is not found";
                    return _response;
                }

                bookingFromDb.VillaNumber = villaNumber;
                bookingFromDb.Status = bookingStatus;

                if (bookingStatus == SD.Status_CheckIn)
                {
                    bookingFromDb.ActualCheckInDate = DateTime.Now;
                }
                if (bookingStatus == SD.Status_Completed)
                {
                    bookingFromDb.ActualCheckOutDate = DateTime.Now;
                }
                _unitOfWork.Save();
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
                var bookingFromDb = _unitOfWork.Booking.Get(m => m.Id == bookingId, tracked:true);
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
                _unitOfWork.Save();
                return _response;
            }
            catch (Exception ex)
            {
                return _response.Exception(ex.Message);
            }
        }
    }
}
