using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Common.Abstractions.Data
{
    public abstract class AsyncDbRepository
    {
        protected readonly IDbConnectionFactory _connectionFactory;

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
