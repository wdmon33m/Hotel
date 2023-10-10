using Azure;
using Hotel.Application.Common.Interfaces;
using Hotel.Application.Dto;
using Hotel.Domain.Entities;
using Hotel.Infrastructure.Data;

namespace Hotel.Infrastructure.Repository
{
    public class VillaRepository : Repository<Villa>, IVillaRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ResponseDto _response;
        public VillaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
            _response = new();
        }

        public ResponseDto Update(Villa entity)
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
