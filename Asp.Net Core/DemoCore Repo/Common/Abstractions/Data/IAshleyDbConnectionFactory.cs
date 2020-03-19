using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Common.Abstractions.Data
{
    public interface IAshleyDbConnectionFactory : IDbConnectionFactory
    {
        new DbConnection GetConnection();
    }
}
