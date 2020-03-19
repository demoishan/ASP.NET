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
    public class AshleyDbConnectionFactory : IAshleyDbConnectionFactory
    {
        private readonly IAppSettings _configuration;

        public AshleyDbConnectionFactory(IAppSettings configuration)
        {
            _configuration = configuration;
        }

        public DbConnection GetConnection()
        {
            var connectionString = _configuration.AshleyDbConnection;

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception($"Could not create a connection string");

            return new SqlConnection(connectionString);
        }
    }
}
