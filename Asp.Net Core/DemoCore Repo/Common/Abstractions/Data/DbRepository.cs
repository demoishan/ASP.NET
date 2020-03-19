using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Common.Abstractions.Data
{
    public abstract class DbRepository<T> : IAbstractRepository<T>
     where T : ITransactionType
    {

        protected readonly IDbConnectionFactory _connectionFactory;

        public DbRepository() { }

        protected DbRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        protected async Task<T> WithConnection<T>(Func<System.Data.Common.DbConnection, Task<T>> execute)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                await connection.OpenAsync();
                connection.EnlistTransaction(Transaction.Current);
                return await execute(connection);
            }
        }

        protected async Task WithConnection(Func<DbConnection, Task> execute)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                await connection.OpenAsync();
                connection.EnlistTransaction(Transaction.Current);
                await execute(connection);
            }
        }

        protected Task<IEnumerable<T>> QueryAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = CommandType.StoredProcedure)
        {
            return WithConnection(async c => await c.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType));
        }

        public virtual Task<T> AddAsync(T hmsTransaction)
        {
            throw new NotSupportedException();
        }

        public virtual Task<bool> DeleteAsync(T hmsTransaction)
        {
            throw new NotSupportedException();
        }

        public virtual Task<ICollection<T>> GetAllAsync()
        {
            throw new NotSupportedException();
        }

        public virtual Task<ICollection<T>> GetAsync(T hmsDomainEntity)
        {
            throw new NotSupportedException();
        }

        public virtual Task<T> GetByIdAsync(string id)
        {
            throw new NotSupportedException();
        }

        public virtual Task<ICollection<T>> GetByIdAsync(int Id)
        {
            throw new NotSupportedException();
        }

        public virtual Task<ICollection<T>> GetByIdAsync(long Id)
        {
            throw new NotSupportedException();
        }

        public virtual Task<T> UpdateAsync(T hmsTransaction)
        {
            throw new NotSupportedException();
        }

        Task<ICollection<T>> IAbstractRepository<T>.GetByIdAsync(int Id)
        {
            throw new NotImplementedException();
        }

        Task<ICollection<T>> IAbstractRepository<T>.GetByIdAsync(long Id)
        {
            throw new NotImplementedException();
        }

        Task<ICollection<T>> IAbstractRepository<T>.GetAsync(T hmsDomainEntity)
        {
            throw new NotImplementedException();
        }

        Task<ICollection<T>> IAbstractRepository<T>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<T> IAbstractRepository<T>.UpdateAsync(T hmsTransaction)
        {
            throw new NotImplementedException();
        }

        Task<T> IAbstractRepository<T>.AddAsync(T hmsTransaction)
        {
            throw new NotImplementedException();
        }

        Task<bool> IAbstractRepository<T>.DeleteAsync(T hmsTransaction)
        {
            throw new NotImplementedException();
        }
    }
}
