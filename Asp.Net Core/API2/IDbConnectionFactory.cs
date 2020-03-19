using System.Data.Common;

namespace Ashley.HMS.Common.Abstractions.Data
{
    public interface IDbConnectionFactory
    {
        DbConnection GetConnection();
    }
}
