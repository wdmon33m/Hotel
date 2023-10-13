using Hotel.Application.Common.Dto;
using Hotel.Domain.Entities;

namespace Hotel.Application.Services.Interface
{
    public interface IVillaService
    {
        IEnumerable<Villa> GetAllVillas();
        Villa GetVilla(int id);
        ResponseDto CreateVilla(Villa entity, string webRootPath);
        ResponseDto UpdateVilla(Villa entity, string webRootPath);
        ResponseDto DeleteVilla(int id, string webRootPath);
        bool IsVillaAvailble(int villaId, DateOnly checkInDate, int nights);
    }
}
