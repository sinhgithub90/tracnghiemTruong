using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace TestLinks.Extensions
{
    public static class RoleManagerExtension
    {
        public static async Task<IdentityRole> EnsureRole(this RoleManager<IdentityRole> roleManager,
           string roleName)
        {

            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                role = new IdentityRole()
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                };

                await roleManager.CreateAsync(role);
                role = await roleManager.FindByNameAsync(roleName);

            }


            return role;
        }

    }
}
