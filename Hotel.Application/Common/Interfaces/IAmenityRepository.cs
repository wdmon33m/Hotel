using Hotel.Application.Dto;
using Hotel.Domain.Entities;

namespace Hotel.Application.Common.Interfaces
{
    public interface IAmenityRepository : IRepository<Amenity>
    {
        ResponseDto Update(Amenity entity);
    }
}
