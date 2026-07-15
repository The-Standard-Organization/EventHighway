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
    // ASP.NET Core Identity (UserManager/RoleManager) is an external component, so it is
    // wrapped by this broker. Services depend on the broker, never on the managers directly.
    public interface IIdentityBroker
    {
        IQueryable<AppUser> SelectAllUsers();

        ValueTask<AppUser?> SelectUserByIdAsync(Guid userId);

        ValueTask<IdentityResult> InsertUserAsync(AppUser user, string password);

        ValueTask<IdentityResult> DeleteUserAsync(AppUser user);

        ValueTask<IList<string>> SelectUserRolesAsync(AppUser user);

        ValueTask<IdentityResult> InsertUserToRoleAsync(AppUser user, string roleName);

        ValueTask<IdentityResult> DeleteUserFromRoleAsync(AppUser user, string roleName);

        ValueTask<IList<AppUser>> SelectUsersInRoleAsync(string roleName);

        ValueTask<bool> SelectIsLockedOutAsync(AppUser user);

        ValueTask<IdentityResult> UpdateUserAsync(AppUser user);

        ValueTask<IdentityResult> SetUserNameAsync(AppUser user, string userName);

        ValueTask<IdentityResult> SetEmailAsync(AppUser user, string email);

        ValueTask<IdentityResult> SetPhoneNumberAsync(AppUser user, string phoneNumber);

        ValueTask<string> GenerateEmailConfirmationTokenAsync(AppUser user);

        ValueTask<IdentityResult> ConfirmEmailAsync(AppUser user, string token);

        ValueTask<string> GeneratePasswordResetTokenAsync(AppUser user);

        ValueTask<IdentityResult> SetLockoutEnabledAsync(AppUser user, bool enabled);

        ValueTask<IdentityResult> SetLockoutEndDateAsync(
            AppUser user,
            DateTimeOffset? lockoutEnd);

        ValueTask<IdentityResult> ResetAccessFailedCountAsync(AppUser user);

        ValueTask<IdentityResult> SetTwoFactorEnabledAsync(AppUser user, bool enabled);

        ValueTask<IdentityResult> ResetAuthenticatorKeyAsync(AppUser user);

        IQueryable<AppRole> SelectAllRoles();
    }
}
