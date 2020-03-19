using DemoDapper.Abstractions;
using DemoDapper.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoDapper.Repositories
{
    public interface ICategoryRepository : IAbstractRepository<CategoryMaster>
    {
        Task<ICollection<CategoryMaster>> GetAllAsync1();

        Task<IEnumerable<CategoryMaster>> GetAsync();
    }
}