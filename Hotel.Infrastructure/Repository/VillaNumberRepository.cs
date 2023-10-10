using Hotel.Application.Common.Interfaces;
using Hotel.Application.Dto;
using Hotel.Domain.Entities;
using Hotel.Infrastructure.Data;

namespace Hotel.Infrastructure.Repository
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ResponseDto _response;
        public VillaNumberRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
            _response = new();
        }

        public ResponseDto Update(VillaNumber entity)
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
