using Hotel.Application.Common.Dto;
using Hotel.Domain.Entities;

namespace Hotel.Application.Common.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        ResponseDto Update(Booking entity);
    }
        
}
