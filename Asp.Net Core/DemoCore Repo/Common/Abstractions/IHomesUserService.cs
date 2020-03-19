using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Abstractions
{
    public interface IHomesUserService : IAbstractService<HomesUser>
    {
        Task<ICollection<HomesUser>> AddUsers(ICollection<HomesUser> users);
    }
}
