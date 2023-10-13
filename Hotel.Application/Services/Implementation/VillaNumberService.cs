using Hotel.Application.Common.Dto;
using Hotel.Application.Common.Interfaces;
using Hotel.Application.Services.Interface;
using Hotel.Domain.Entities;

namespace Hotel.Application.Services.Implementation
{
    public class VillaNumberService : IVillaNumberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ResponseDto _response;

        public VillaNumberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new();
        }
        public ResponseDto CreateVillaNumber(VillaNumber entity)
        {
            try
            {
                _unitOfWork.VillaNumber.Add(entity);
                _unitOfWork.Save();

                return _response;
            }
            catch (Exception ex)
            {
                return _response.Exception(ex.Message);
            }
        }

        public ResponseDto DeleteVillaNumber(int id)
        {
            try
            {
                VillaNumber? obj = GetVillaNumber(id);

                _unitOfWork.VillaNumber.Remove(obj);
                _unitOfWork.Save();

                return _response;
            }
            catch (Exception ex)
            {
                return _response.Exception(ex.Message);
            }
        }

        public IEnumerable<VillaNumber> GetAllVillaNumbers()
        {
            return _unitOfWork.VillaNumber.GetAll();
        }

        public VillaNumber GetVillaNumber(int id)
        {
            return _unitOfWork.VillaNumber.Get(e => e.Villa_Number == id); ;
        }

        public ResponseDto UpdateVillaNumber(VillaNumber entity)
        {
            try
            {
                _unitOfWork.VillaNumber.Update(entity);
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
