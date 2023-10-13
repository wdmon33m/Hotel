using Hotel.Application.Common.Dto;
using Hotel.Application.Common.Interfaces;
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
    }
}
