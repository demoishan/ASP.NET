using System.Data.Common;

namespace Ashley.HMS.Common.Abstractions.Data
{
    public interface IHomesDbConnectionFactory : IDbConnectionFactory
    {
        new DbConnection GetConnection();
    }
}
