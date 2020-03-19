using System;
using System.Data;

namespace DemoDapper.Extension
{
    public interface IConnectionFactory : IDisposable
    {
        IDbConnection GetConnection { get; }
    }
}