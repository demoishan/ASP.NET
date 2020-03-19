using DemoDapper.Abstractions;
using DemoDapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DemoDapper.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IAbstractRepository<CategoryMaster> _userRepository;

        public CategoryService( IAbstractRepository<CategoryMaster> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ICollection<CategoryMaster>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<ICollection<CategoryMaster>> GetCategoryAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public Task UpdateAsync(CategoryMaster user)
        {
            throw new NotImplementedException();
        }
    }
}