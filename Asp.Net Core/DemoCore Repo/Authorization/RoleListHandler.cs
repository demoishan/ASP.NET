using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Authorization
{
    public class RoleListHandler : AuthorizationHandler<RoleListRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleListRequirement requirement)
        {
            if (context.User.HasClaim(claim => claim.Type == ClaimTypes.Role) && requirement.Roles.Any(context.User.IsInRole))
            {
                context.Succeed(requirement);
            }

            await Task.CompletedTask;
        }
    }
}
