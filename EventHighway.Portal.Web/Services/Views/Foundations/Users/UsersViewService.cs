// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Brokers.Identities;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.Users;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Users;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Users.Exceptions;

namespace EventHighway.Portal.Web.Services.Views.Foundations.Users
{
    public partial class UsersViewService : IUsersViewService
    {
        private readonly IIdentityBroker identityBroker;
        private readonly ILoggingBroker loggingBroker;

        public UsersViewService(
            IIdentityBroker identityBroker,
            ILoggingBroker loggingBroker)
        {
            this.identityBroker = identityBroker;
            this.loggingBroker = loggingBroker;
        }

        private const string AdministratorsRole = "Administrators";

        public ValueTask<List<UserView>> RetrieveAllUsersAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(() =>
        {
            List<UserView> users = this.identityBroker.SelectAllUsers()
                .Select(AsView)
                .ToList();

            return new ValueTask<List<UserView>>(users);
        });

        public ValueTask<UserView> RetrieveUserByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            AppUser user = await RetrieveExistingUserByIdAsync(userId);

            IList<string> roles = await this.identityBroker.SelectUserRolesAsync(user);

            UserView view = AsView(user);
            view.Roles = roles.ToList();
            view.IsLockedOut = await this.identityBroker.SelectIsLockedOutAsync(user);

            return view;
        });

        public ValueTask<UserView> AddUserAsync(
            UserView user,
            string password,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            var userToAdd = new AppUser
            {
                UserName = user.UserName,
                Email = string.IsNullOrWhiteSpace(user.Email)
                    ? $"{user.UserName}@eventhighway.local"
                    : user.Email,
                EmailConfirmed = true
            };

            await this.identityBroker.InsertUserAsync(userToAdd, password);

            return AsView(userToAdd);
        });

        public ValueTask RemoveUserByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            AppUser user = await RetrieveExistingUserByIdAsync(userId);

            IList<string> roles = await this.identityBroker.SelectUserRolesAsync(user);

            if (roles.Contains(AdministratorsRole))
            {
                await EnsureNotLastAdministratorAsync();
            }

            await this.identityBroker.DeleteUserAsync(user);
        });

        public ValueTask AddUserToRoleAsync(
            Guid userId,
            string roleName,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            AppUser user = await RetrieveExistingUserByIdAsync(userId);

            await this.identityBroker.InsertUserToRoleAsync(user, roleName);
        });

        public ValueTask RemoveUserFromRoleAsync(
            Guid userId,
            string roleName,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            if (roleName == AdministratorsRole)
            {
                await EnsureNotLastAdministratorAsync();
            }

            AppUser user = await RetrieveExistingUserByIdAsync(userId);

            await this.identityBroker.DeleteUserFromRoleAsync(user, roleName);
        });

        public ValueTask<List<string>> RetrieveAllRoleNamesAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(() =>
        {
            List<string> roleNames = this.identityBroker.SelectAllRoles()
                .Select(role => role.Name)
                .Where(name => name != null)
                .Select(name => name!)
                .ToList();

            return new ValueTask<List<string>>(roleNames);
        });

        public ValueTask<UserView> ModifyUserAsync(
            UserView user,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            AppUser existingUser = await RetrieveExistingUserByIdAsync(user.Id);

            await this.identityBroker.SetUserNameAsync(existingUser, user.UserName);
            await this.identityBroker.SetEmailAsync(existingUser, user.Email);
            await this.identityBroker.SetPhoneNumberAsync(existingUser, user.PhoneNumber);

            return AsView(existingUser);
        });

        public ValueTask ConfirmUserEmailAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            AppUser user = await RetrieveExistingUserByIdAsync(userId);

            string token =
                await this.identityBroker.GenerateEmailConfirmationTokenAsync(user);

            await this.identityBroker.ConfirmEmailAsync(user, token);
        });

        public ValueTask<string> GenerateEmailConfirmationTokenAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            AppUser user = await RetrieveExistingUserByIdAsync(userId);

            return await this.identityBroker.GenerateEmailConfirmationTokenAsync(user);
        });

        public ValueTask<string> GeneratePasswordResetTokenAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            AppUser user = await RetrieveExistingUserByIdAsync(userId);

            return await this.identityBroker.GeneratePasswordResetTokenAsync(user);
        });

        public ValueTask LockUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            AppUser user = await RetrieveExistingUserByIdAsync(userId);

            await EnsureNotLastAdministratorWhenInRoleAsync(user);

            await this.identityBroker.SetLockoutEnabledAsync(user, true);
            await this.identityBroker.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        });

        public ValueTask UnlockUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            AppUser user = await RetrieveExistingUserByIdAsync(userId);

            await this.identityBroker.SetLockoutEndDateAsync(user, null);
        });

        public ValueTask ResetAccessFailedCountAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            AppUser user = await RetrieveExistingUserByIdAsync(userId);

            await this.identityBroker.ResetAccessFailedCountAsync(user);
        });

        public ValueTask SetTwoFactorEnabledAsync(
            Guid userId,
            bool enabled,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            AppUser user = await RetrieveExistingUserByIdAsync(userId);

            await this.identityBroker.SetTwoFactorEnabledAsync(user, enabled);

            if (enabled is false)
            {
                await this.identityBroker.ResetAuthenticatorKeyAsync(user);
            }
        });

        public ValueTask DisableUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            AppUser user = await RetrieveExistingUserByIdAsync(userId);

            await EnsureNotLastAdministratorWhenInRoleAsync(user);

            user.IsDisabled = true;
            await this.identityBroker.UpdateUserAsync(user);

            await this.identityBroker.SetLockoutEnabledAsync(user, true);
            await this.identityBroker.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        });

        public ValueTask EnableUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            AppUser user = await RetrieveExistingUserByIdAsync(userId);

            user.IsDisabled = false;
            await this.identityBroker.UpdateUserAsync(user);

            await this.identityBroker.SetLockoutEndDateAsync(user, null);
        });

        // A by-id lookup is expected to hit — the id comes from a rendered list, not a search — so a
        // miss is a not-found to raise, never a null to pass along to the next call.
        private async ValueTask<AppUser> RetrieveExistingUserByIdAsync(Guid userId) =>
            await this.identityBroker.SelectUserByIdAsync(userId)
                ?? throw new NotFoundUsersViewException(userId);

        private async ValueTask EnsureNotLastAdministratorWhenInRoleAsync(AppUser user)
        {
            IList<string> roles = await this.identityBroker.SelectUserRolesAsync(user);

            if (roles.Contains(AdministratorsRole))
            {
                await EnsureNotLastAdministratorAsync();
            }
        }

        private async ValueTask EnsureNotLastAdministratorAsync()
        {
            IList<AppUser> administrators =
                await this.identityBroker.SelectUsersInRoleAsync(AdministratorsRole);

            if (administrators.Count <= 1)
            {
                throw new LastAdministratorUsersViewException();
            }
        }

        private static UserView AsView(AppUser user) =>
            new UserView
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed,
                AccessFailedCount = user.AccessFailedCount,
                TwoFactorEnabled = user.TwoFactorEnabled,
                IsDisabled = user.IsDisabled,
                Roles = new List<string>()
            };
    }
}
