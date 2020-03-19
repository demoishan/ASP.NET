using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Abstractions
{
    public interface IApplicationDefaultsRepository : IAbstractRepository<ApplicationDefault>
    {
        Task<ApplicationDefault> GetAsync(string defaultDescription, string defaultPC);
    }
}
