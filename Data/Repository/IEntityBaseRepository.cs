using System.Linq.Expressions;

namespace Debt_Calculation_And_Repayment_System.Data.Repository
{
    public interface IEntityBaseRepository<T> where T: class, IEntityBase,new()
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetByIdAsync(string id);
        Task<T> GetByIdAsync(string id, params Expression<Func<T, object>>[] includeProperties);
        Task AddAsync(T entity);
        Task UpdateAsync(string id, T entity);
        Task DeleteAsync(string id);
        Task SaveChangesAsync();
    }
}
