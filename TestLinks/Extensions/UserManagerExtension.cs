using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestLinks.Models;

namespace TestLinks.Extensions
{
    public static class UserManagerExtension
    {
        
        public static async Task<IdentityResult> EnsureUserRole(this UserManager<ApplicationUser> userManager,
                                                              string uid, string roleName)
        {
            IdentityResult IR = null;

            var user = await userManager.FindByNameAsync(uid.ToString());
            if (user != null)
            {
                if (!await userManager.IsInRoleAsync(user, roleName))
                { 
                    IR = await userManager.AddToRoleAsync(user, roleName);
                }
            }

            return IR;
        }

        public static async Task<ApplicationUser> EnsureAccount(this UserManager<ApplicationUser> userManager,
            ApplicationUser account, string pass)
        {
            var user = await userManager.FindByEmailAsync(account.Email);
            if (user == null)
            {
                user = new ApplicationUser()
                {
                    UserName = account.Email,
                    Email = account.Email,
                    DepartmentId = account.DepartmentId,
                };
                var result = await userManager.CreateAsync(user);
                var addResult = await userManager.AddPasswordAsync(user, pass);
            }
            return user;
        }

        public static async Task EnsureAccounts(
            this UserManager<ApplicationUser> userManager, 
            List<ApplicationUser> accounts,
            List<string> passwords)
        {
            for (int i = 0; i < accounts.Count; i++)
            {
                var account = accounts[i];
                var pass = passwords[i];
                await userManager.EnsureAccount(account, pass);
            }
        }
    }
}
