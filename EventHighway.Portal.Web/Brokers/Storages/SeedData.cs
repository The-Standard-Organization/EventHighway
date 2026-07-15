// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Portal.Web.Models.Services.Domains.Foundations.Roles;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Portal.Web.Brokers.Storages
{
    // Idempotent first-run seed: creates the Administrators and Users roles and the default
    // admin/user accounts (Spec Section 6.3). Default credentials are intentionally weak for
    // first-run/demo; production must enforce a strong password policy and force-change.
    public static class SeedData
    {
        private const string AdministratorsRole = "Administrators";
        private const string UsersRole = "Users";

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            IServiceProvider services = scope.ServiceProvider;

            var securityDbContext = services.GetRequiredService<SecurityDbContext>();
            await securityDbContext.Database.MigrateAsync();

            var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();

            await EnsureRoleAsync(roleManager, AdministratorsRole);
            await EnsureRoleAsync(roleManager, UsersRole);
            await EnsureUserAsync(userManager, "admin", "admin", AdministratorsRole);
            await EnsureUserAsync(userManager, "user", "user", UsersRole);
        }

        private static async Task EnsureRoleAsync(
            RoleManager<AppRole> roleManager,
            string roleName)
        {
            if ((await roleManager.RoleExistsAsync(roleName)) is false)
            {
                await roleManager.CreateAsync(new AppRole { Name = roleName });
            }
        }

        private static async Task EnsureUserAsync(
            UserManager<AppUser> userManager,
            string userName,
            string password,
            string roleName)
        {
            AppUser? existingUser = await userManager.FindByNameAsync(userName);

            if (existingUser is null)
            {
                var newUser = new AppUser
                {
                    UserName = userName,
                    Email = $"{userName}@eventhighway.local",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(newUser, password);
                await userManager.AddToRoleAsync(newUser, roleName);
            }
        }
    }
}
