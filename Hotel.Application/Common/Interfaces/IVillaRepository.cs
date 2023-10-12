using Hotel.Application.Dto;
using Hotel.Domain.Entities;

namespace Hotel.Application.Common.Interfaces
{
    public interface IVillaRepository : IRepository<Villa>
    {
        ResponseDto Update(Villa entity);
        bool IsVillaAvailble(int villaId,
            List<VillaNumber> villaNumberList, DateOnly checkInDate,
            int nights, List<Booking> bookings);
    }
}
