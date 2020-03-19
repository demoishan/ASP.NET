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
    class HomesUserSqlRepository : AsyncDbRepository, IHomesUserSqlRepository
    {
        private readonly IAppSettings _configuration;
        private ILogger<HomesUserSqlRepository> _logger;
        const string contentType = "application/json";

        public HomesUserSqlRepository(IAppSettings configuration, IAshleyHsDbConnectionFactory connectionFactory, ILogger<HomesUserSqlRepository> logger) : base(connectionFactory)
        {
            _logger = logger;
            _configuration = configuration;
            if (connectionFactory == null) throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<HomesUser> AddAsync(HomesUser homesUser)
        {
            var queryParameters = new DynamicParameters();

            queryParameters.Add("@UserName", homesUser.UserName);
            queryParameters.Add("@Password", homesUser.Password);
            queryParameters.Add("@SecurityLevel", homesUser.SecurityLevel);
            queryParameters.Add("@ProfitCenter", homesUser.ProfitCenter);
            queryParameters.Add("@Spins", homesUser.Spins);
            queryParameters.Add("@ActiveRecord", homesUser.ActiveRecord);
            if (string.IsNullOrEmpty(homesUser.SalesPersonId))
                queryParameters.Add("@SalesPersonId", null);
            else
                queryParameters.Add("@SalesPersonId", homesUser.SalesPersonId);

            await WithConnection(async c => await c.ExecuteAsync(_configuration.InsertHomesUser, queryParameters, commandType: CommandType.StoredProcedure));

            return homesUser;
        }

        public Task<bool> DeleteAsync(HomesUser hmsTransaction)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<HomesUser>> GetAllAsync()
        {
            return (await WithConnection(async c => await c.QueryAsync<HomesUser>(_configuration.GetHomesUsers, commandType: CommandType.StoredProcedure))).ToList();
        }

        public Task<ICollection<HomesUser>> GetAsync(HomesUser hmsDomainEntity)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HomesUser>> GetByIdAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<HomesUser>> GetByIdAsync(long Id)
        {
            var queryParameters = new DynamicParameters();
            queryParameters.Add("@UserId", Id);

            return (await WithConnection(async c => await c.QueryAsync<HomesUser>(_configuration.GetHomesUserById, queryParameters, commandType: CommandType.StoredProcedure))).ToList();
        }

        public async Task<HomesUser> UpdateAsync(HomesUser homesUser)
        {
            var queryParameters = new DynamicParameters();

            queryParameters.Add("@UserId", homesUser.UserId);
            queryParameters.Add("@UserName", homesUser.UserName);
            queryParameters.Add("@Password", homesUser.Password);
            queryParameters.Add("@SecurityLevel", homesUser.SecurityLevel);
            queryParameters.Add("@ProfitCenter", homesUser.ProfitCenter);
            queryParameters.Add("@Spins", homesUser.Spins);
            queryParameters.Add("@ActiveRecord", homesUser.ActiveRecord);
            queryParameters.Add("@SalesPersonId", homesUser.SalesPersonId);

            await WithConnection(async c => await c.ExecuteAsync(_configuration.UpdateHomesUser, queryParameters, commandType: CommandType.StoredProcedure));

            return homesUser;
        }
    }
}
