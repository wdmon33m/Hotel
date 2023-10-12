using Hotel.Application.Dto;
using System.Linq.Expressions;

namespace Hotel.Application.Common.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T,bool>>? filter = null, string? includeProperties = null, bool tracked = false); 
        T Get(Expression<Func<T,bool>> filter, string? includeProperties = null, bool tracked = false); 
        ResponseDto Add (T entity);
        ResponseDto Remove (T entity);
    }
}
