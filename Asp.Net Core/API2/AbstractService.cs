using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DemoDapper.Abstractions
{
    public abstract class AbstractService<T> : IAbstractService<T> where T : ITransactionType
    {
        //TODO: Add code to add messages to the queue
        public abstract Task<T> AddAsync(T hmsTransaction);

        public abstract Task<bool> DeleteAsync(T hmsTransaction);

        public abstract Task<ICollection<T>> ReadAllAsync();

        public abstract Task<ICollection<T>> ReadByIdAsync(int Id);

        public abstract Task<ICollection<T>> ReadByIdAsync(long Id);

        public abstract Task<ICollection<T>> ReadFilteredListAsync(Func<T, bool> condition);

        public abstract Task<ICollection<T>> ReadAsync(T hmsDomainEntity);

        public abstract Task<T> UpdateAsync(T hmsTransaction);

    }
}