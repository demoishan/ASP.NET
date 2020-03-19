using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Abstractions.Configuration
{
    public interface IDatabaseSettings
    {
        string AshleyDbConnection { get; }
        string AshleyHsDbConnection { get; }
        string AuditDbConnection { get; }
        string HomesDbConnection { get; }
    }
}
