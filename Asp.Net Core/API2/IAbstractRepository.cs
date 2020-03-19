using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoDapper.Abstractions
{
    public interface IAbstractRepository<T> where T : ITransactionType
    {
        Task<ICollection<T>> GetByIdAsync(int Id);
        Task<ICollection<T>> GetByIdAsync(long Id);
        Task<ICollection<T>> GetAsync(T hmsDomainEntity);
        Task<ICollection<T>> GetAllAsync();
        Task<T> UpdateAsync(T hmsTransaction);
        Task<T> AddAsync(T hmsTransaction);
        Task<bool> DeleteAsync(T hmsTransaction);
    }
}