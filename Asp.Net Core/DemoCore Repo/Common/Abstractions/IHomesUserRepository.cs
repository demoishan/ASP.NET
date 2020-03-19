using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Abstractions
{
    public interface IHomesUserRepository : IAbstractRepository<HomesUser>
    {
        Task DeleteFromUser(long userId);
        bool IsPervasiveTrue();
    }
}
