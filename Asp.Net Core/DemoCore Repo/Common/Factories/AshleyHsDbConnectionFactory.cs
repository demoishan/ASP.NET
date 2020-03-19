using Common.Abstractions.Configuration;
using Common.Abstractions.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Common.Factories
{
    [ExcludeFromCodeCoverage]
    public class AshleyHsDbConnectionFactory : IAshleyHsDbConnectionFactory
    {
        private readonly IAppSettings _configuration;

        public AshleyHsDbConnectionFactory(IAppSettings configuration)
        {
            _configuration = configuration;
        }

        public DbConnection GetConnection()
        {
            var connectionString = _configuration.AshleyHsDbConnection;

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception($"Could not create a connection string");

            return new SqlConnection(connectionString);
        }
    }
}
