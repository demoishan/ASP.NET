using Common.Abstractions;
using Common.Abstractions.Configuration;
using Common.Abstractions.Data;
using Common.Models;
using Common.Abstractions;
using Common.Abstractions.Configuration;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class AccessControlRepository : AsyncDbRepository, IAccessControlRepository
    {
        private readonly IAppSettings _configuration;

        public AccessControlRepository(IAppSettings configuration, IAshleyHsDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _configuration = configuration;
            if (connectionFactory == null) throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<ICollection<SecureResource>> GetAllAsync()
        {
            //mapping this the old fassion way for now 
            //TODO: Refactor mapping code
            var resourcerows = (await WithConnection(async c => await c.QueryAsync(sql: _configuration.GetResources, commandType: CommandType.StoredProcedure))).ToList();
            List<SecureResource> resources = new List<SecureResource>();
            SecureResource currentResource = null;
            int currentResourceId = 0;

            foreach (var row in resourcerows)
            {
                Role currentRole = new Role();
                currentRole.Id = (int)row.RoleId;
                currentRole.Name = row.RoleName;

                if (currentResource is null || row.ResourceId != currentResource.Id)
                {
                    List<Role> currentRoles = new List<Role>();
                    currentRoles.Add(currentRole);
                    currentResource = new SecureResource();
                    currentResource.Id = (int)row.ResourceId;
                    currentResource.Name = row.ResourceName;
                    currentResource.AccessControl = new Dictionary<string, ICollection<Role>>();
                    currentResource.AccessControl.Add(row.ActionName, currentRoles);
                    resources.Add(currentResource);
                }
                else
                {
                    ICollection<Role> existingRoles = null;
                    if (currentResource.AccessControl.TryGetValue(row.ActionName, out existingRoles))
                    {
                        existingRoles.Add(currentRole);
                    }
                    else
                    {
                        List<Role> currentRoles = new List<Role>();
                        currentRoles.Add(currentRole);
                        currentResource.AccessControl.Add(row.ActionName, currentRoles);
                    }
                }
            }

            return resources;
        }

        public Task<ICollection<SecureResource>> GetAsync(SecureResource hmsDomainEntity)
        {
            throw new NotSupportedException();
        }

        public Task<ICollection<SecureResource>> GetByIdAsync(long Id)
        {
            throw new NotSupportedException();
        }
    }
}
