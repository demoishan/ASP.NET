using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authorization
{
    public class RoleListRequirement : IAuthorizationRequirement
    {
        public RoleListRequirement(ICollection<string> roles)
        {
            Roles = roles;
        }

        public ICollection<string> Roles { get; }
    }
}
