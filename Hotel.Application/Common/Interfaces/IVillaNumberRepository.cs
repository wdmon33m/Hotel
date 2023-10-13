using Hotel.Application.Common.Dto;
using Hotel.Domain.Entities;

namespace Hotel.Application.Common.Interfaces
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
        ResponseDto Update(VillaNumber entity);
    }
}
