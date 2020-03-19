using System.Data.Common;

namespace Ashley.HMS.Common.Abstractions.Data
{
    public interface IAshleyDbConnectionFactory: IDbConnectionFactory
    {
        new DbConnection GetConnection();
    }
}
