using System.Data.Common;

namespace Ashley.HMS.Common.Abstractions.Data
{
    public interface IAshleyHsDbConnectionFactory: IDbConnectionFactory
    {
        new DbConnection GetConnection();
    }
}
