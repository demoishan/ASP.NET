using System.Data.Common;

namespace Common.Abstractions.Data
{
    public interface IDbConnectionFactory
    {
        DbConnection GetConnection();
    }
}
