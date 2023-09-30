using Hotel.Domain.Entities;

namespace Hotel.Application.Common.Interfaces
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
        void Update(VillaNumber entity);
        void Save();
    }
}
