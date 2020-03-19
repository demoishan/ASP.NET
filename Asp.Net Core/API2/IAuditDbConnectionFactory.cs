using System.Data.Common;

namespace Ashley.HMS.Common.Abstractions.Data
{
    public interface IAuditDbConnectionFactory: IDbConnectionFactory
    {
        new DbConnection GetConnection();
    }
}
