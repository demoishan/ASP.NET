using DemoDapper.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoDapper.Services
{
    public interface ICategoryService
    {
        Task<ICollection<CategoryMaster>> GetAllAsync();
        Task UpdateAsync(CategoryMaster user);
    }
}