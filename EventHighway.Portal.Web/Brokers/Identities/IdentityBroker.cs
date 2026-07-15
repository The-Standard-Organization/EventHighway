// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.Roles;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.Users;
using Microsoft.AspNetCore.Identity;

namespace EventHighway.Portal.Web.Brokers.Identities
{
    public sealed class IdentityBroker : IIdentityBroker
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;

        public IdentityBroker(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IQueryable<AppUser> SelectAllUsers() =>
            this.userManager.Users;

        public async ValueTask<AppUser?> SelectUserByIdAsync(Guid userId) =>
            await this.userManager.FindByIdAsync(userId.ToString());

        public async ValueTask<IdentityResult> InsertUserAsync(AppUser user, string password) =>
            await this.userManager.CreateAsync(user, password);

        public async ValueTask<IdentityResult> DeleteUserAsync(AppUser user) =>
            await this.userManager.DeleteAsync(user);

        public async ValueTask<IList<string>> SelectUserRolesAsync(AppUser user) =>
            await this.userManager.GetRolesAsync(user);

        public async ValueTask<IdentityResult> InsertUserToRoleAsync(AppUser user, string roleName) =>
            await this.userManager.AddToRoleAsync(user, roleName);

        public async ValueTask<IdentityResult> DeleteUserFromRoleAsync(AppUser user, string roleName) =>
            await this.userManager.RemoveFromRoleAsync(user, roleName);

        public async ValueTask<IList<AppUser>> SelectUsersInRoleAsync(string roleName) =>
            await this.userManager.GetUsersInRoleAsync(roleName);

        public async ValueTask<bool> SelectIsLockedOutAsync(AppUser user) =>
            await this.userManager.IsLockedOutAsync(user);

        public async ValueTask<IdentityResult> UpdateUserAsync(AppUser user) =>
            await this.userManager.UpdateAsync(user);

        public async ValueTask<IdentityResult> SetUserNameAsync(AppUser user, string userName) =>
            await this.userManager.SetUserNameAsync(user, userName);

        public async ValueTask<IdentityResult> SetEmailAsync(AppUser user, string email) =>
            await this.userManager.SetEmailAsync(user, email);

        public async ValueTask<IdentityResult> SetPhoneNumberAsync(
            AppUser user, string phoneNumber) =>
            await this.userManager.SetPhoneNumberAsync(user, phoneNumber);

        public async ValueTask<string> GenerateEmailConfirmationTokenAsync(AppUser user) =>
            await this.userManager.GenerateEmailConfirmationTokenAsync(user);

        public async ValueTask<IdentityResult> ConfirmEmailAsync(AppUser user, string token) =>
            await this.userManager.ConfirmEmailAsync(user, token);

        public async ValueTask<string> GeneratePasswordResetTokenAsync(AppUser user) =>
            await this.userManager.GeneratePasswordResetTokenAsync(user);

        public async ValueTask<IdentityResult> SetLockoutEnabledAsync(AppUser user, bool enabled) =>
            await this.userManager.SetLockoutEnabledAsync(user, enabled);

        public async ValueTask<IdentityResult> SetLockoutEndDateAsync(
            AppUser user, DateTimeOffset? lockoutEnd) =>
            await this.userManager.SetLockoutEndDateAsync(user, lockoutEnd);

        public async ValueTask<IdentityResult> ResetAccessFailedCountAsync(AppUser user) =>
            await this.userManager.ResetAccessFailedCountAsync(user);

        public async ValueTask<IdentityResult> SetTwoFactorEnabledAsync(AppUser user, bool enabled) =>
            await this.userManager.SetTwoFactorEnabledAsync(user, enabled);

        public async ValueTask<IdentityResult> ResetAuthenticatorKeyAsync(AppUser user) =>
            await this.userManager.ResetAuthenticatorKeyAsync(user);

        public IQueryable<AppRole> SelectAllRoles() =>
            this.roleManager.Roles;
    }
}
