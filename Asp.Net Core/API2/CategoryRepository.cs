using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using DemoDapper.Extension;
using DemoDapper.Models;

namespace DemoDapper.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        IConnectionFactory _connectionFactory;
        public CategoryRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public Task<CategoryMaster> AddAsync(CategoryMaster hmsTransaction)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(CategoryMaster hmsTransaction)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<CategoryMaster>> GetAllAsync()
        {
            var query = "GetCategoryMaster";
            var queryParameters = new DynamicParameters();
            queryParameters.Add("@Flag", "True");

            //var list = await SqlMapper.QueryAsync<CategoryMaster>(_connectionFactory.GetConnection, query, queryParameters, commandType: CommandType.StoredProcedure);
            //return list.ToList();

            return (await SqlMapper.QueryAsync<CategoryMaster>(_connectionFactory.GetConnection, query, queryParameters, commandType: CommandType.StoredProcedure)) as ICollection<CategoryMaster>;
        }

        public async Task<ICollection<CategoryMaster>> GetAllAsync1()
        {
            var query = "GetCategoryMaster";
            var queryParameters = new DynamicParameters();
            queryParameters.Add("@Flag", "True");

            //var list = await SqlMapper.QueryAsync<CategoryMaster>(_connectionFactory.GetConnection, query, queryParameters, commandType: CommandType.StoredProcedure);
            //return list.ToList();

            return (await SqlMapper.QueryAsync<CategoryMaster>(_connectionFactory.GetConnection, query, queryParameters, commandType: CommandType.StoredProcedure)) as ICollection<CategoryMaster>;
        }

        public Task<IEnumerable<CategoryMaster>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CategoryMaster>> GetAsync(CategoryMaster hmsDomainEntity)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CategoryMaster>> GetByIdAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CategoryMaster>> GetByIdAsync(long Id)
        {
            throw new NotImplementedException();
        }

        public Task<CategoryMaster> UpdateAsync(CategoryMaster hmsTransaction)
        {
            throw new NotImplementedException();
        }
    }
}