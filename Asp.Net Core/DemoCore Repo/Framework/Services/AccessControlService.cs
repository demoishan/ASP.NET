using Common.Abstractions;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Services
{
    public class AccessControlService : IAccessControlService
    {
        private IAccessControlRepository _repository;

        public AccessControlService(IAccessControlRepository repository)
        {

            _repository = repository;
        }

        public Task<SecureResource> GetResourceAsync(int Id)
        {
            throw new NotSupportedException();
        }

        public async Task<ICollection<SecureResource>> GetResourceListAsync()
        {
            return await _repository.GetAllAsync();
        }

        public bool ValidateToken(string token)
        {
            throw new NotSupportedException();
        }
    }
}
