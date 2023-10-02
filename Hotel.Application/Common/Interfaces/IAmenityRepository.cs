using Hotel.Domain.Entities;

namespace Hotel.Application.Common.Interfaces
{
    public interface IAmenityRepository : IRepository<Amenity>
    {
        void Update(Amenity entity);
        void Save();
    }
}
