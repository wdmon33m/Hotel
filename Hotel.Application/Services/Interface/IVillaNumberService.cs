using Hotel.Application.Common.Dto;
using Hotel.Domain.Entities;

namespace Hotel.Application.Services.Interface
{
    public interface IVillaNumberService
    {
        IEnumerable<VillaNumber> GetAllVillaNumbers();
        VillaNumber GetVillaNumber(int id);
        ResponseDto CreateVillaNumber(VillaNumber entity);
        ResponseDto UpdateVillaNumber(VillaNumber entity);
        ResponseDto DeleteVillaNumber(int id);
    }
}
