using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Abstractions
{
    public interface IAccessControlRepository
    {
        Task<ICollection<SecureResource>> GetByIdAsync(long Id);
        Task<ICollection<SecureResource>> GetAsync(SecureResource hmsDomainEntity);
        Task<ICollection<SecureResource>> GetAllAsync();
    }
}
