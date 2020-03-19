using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Abstractions
{
    public interface IAccessControlService
    {
        Task<SecureResource> GetResourceAsync(int Id);
        Task<ICollection<SecureResource>> GetResourceListAsync();

        bool ValidateToken(string token);
    }
}
