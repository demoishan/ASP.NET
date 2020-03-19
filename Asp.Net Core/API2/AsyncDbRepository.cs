using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.Transactions;

namespace Ashley.HMS.Common.Abstractions.Data
{
    public abstract class AsyncDbRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        protected AsyncDbRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        protected async Task<T> WithConnection<T>(Func<DbConnection, Task<T>> execute)
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
    }
}
