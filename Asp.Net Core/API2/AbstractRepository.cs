using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DemoDapper.Abstractions
{
    public abstract class AbstractRepository<T> : IAbstractRepository<T> where T : ITransactionType
    {

        public abstract Task<ICollection<T>> GetAllAsync();


        public abstract Task<ICollection<T>> GetAsync(T hmsDomainEntity);


        public abstract Task<ICollection<T>> GetByIdAsync(int Id);


        public abstract Task<ICollection<T>> GetByIdAsync(long Id);

        public abstract Task<T> UpdateAsync(T hmsTransaction);
        public abstract Task<T> AddAsync(T hmsTransaction);
        public abstract Task<bool> DeleteAsync(T hmsTransaction);

    }
}