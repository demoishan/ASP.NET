using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoDapper.Abstractions
{
    public interface IAbstractService<T> where T : ITransactionType
    {
        Task<T> AddAsync(T hmsTransaction);
        Task<ICollection<T>> ReadByIdAsync(int Id);
        Task<ICollection<T>> ReadByIdAsync(long Id);
        Task<ICollection<T>> ReadAllAsync();
        Task<ICollection<T>> ReadAsync(T hmsDomainEntity);
        Task<ICollection<T>> ReadFilteredListAsync(Func<T, bool> condition);
        Task<T> UpdateAsync(T hmsTransaction);
        Task<bool> DeleteAsync(T hmsTransaction);
    }
}