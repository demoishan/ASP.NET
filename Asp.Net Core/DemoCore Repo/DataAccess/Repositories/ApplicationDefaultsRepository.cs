using Common.Abstractions;
using Common.Abstractions.Configuration;
using Common.Abstractions.Data;
using Common.Models;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class ApplicationDefaultsRepository : AsyncDbRepository, IApplicationDefaultsRepository
    {
        private readonly IAppSettings _configuration;
        private ILogger<ApplicationDefaultsRepository> _logger;
        public ApplicationDefaultsRepository(IAppSettings configuration, IAshleyDbConnectionFactory connectionFactory, ILogger<ApplicationDefaultsRepository> logger) : base(connectionFactory)
        {
            _logger = logger;
            _configuration = configuration;
            if (connectionFactory == null) throw new ArgumentNullException(nameof(connectionFactory));
        }

        public Task<ApplicationDefault> AddAsync(ApplicationDefault hmsTransaction)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(ApplicationDefault hmsTransaction)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ApplicationDefault>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationDefault> GetAsync(string defaultDescription, string defaultPC)
        {
            var queryParameters = new DynamicParameters();
            queryParameters.Add("@defaultdescription", defaultDescription);

            return (await WithConnection(async c => await c.QueryAsync<ApplicationDefault>(sql: _configuration.GetDefaultOption, param: queryParameters, commandType: CommandType.StoredProcedure))).Where(x => x.default_PC == defaultPC).FirstOrDefault();

        }

        public Task<ICollection<ApplicationDefault>> GetAsync(ApplicationDefault hmsDomainEntity)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ApplicationDefault>> GetByIdAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ApplicationDefault>> GetByIdAsync(long Id)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationDefault> UpdateAsync(ApplicationDefault hmsTransaction)
        {
            //throw new NotImplementedException();

            _logger.LogInformation("Start UpdateAsync. {Default Description}=", hmsTransaction.default_Desc);

            var queryParameters = new DynamicParameters();
            queryParameters.Add("@DEFAULTPC", hmsTransaction.default_PC);
            queryParameters.Add("@ProfitCenter", hmsTransaction.default_ProfitCenter);
            queryParameters.Add("@KEY", hmsTransaction.default_Desc);
            if (!int.TryParse(hmsTransaction.default_Value, out int val))
            {
                // Update to random number, log warning and allow app to continue. 
                val = new Random().Next(1000, 2000);
                _logger.LogWarning($"UpdateAsync: Default Description {hmsTransaction.default_Desc} updated with a rendom number !!!");
            }
            // increase current value
            queryParameters.Add("@VALUE", (val + 1).ToString());

            // update required setting
            await WithConnection(async c => await c.ExecuteAsync(_configuration.SetDefaultOption, queryParameters, commandType: CommandType.StoredProcedure));

            _logger.LogInformation("End UpdateAsync. {Default Description}=", hmsTransaction.default_Desc);

            // return updated object
            return (await GetAsync(hmsTransaction.default_Desc, hmsTransaction.default_PC));
        }
    }
}
