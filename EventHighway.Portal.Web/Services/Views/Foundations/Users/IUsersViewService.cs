// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Users;

namespace EventHighway.Portal.Web.Services.Views.Foundations.Users
{
    public interface IUsersViewService
    {
        ValueTask<List<UserView>> RetrieveAllUsersAsync(
            CancellationToken cancellationToken = default);

        ValueTask<UserView> RetrieveUserByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        ValueTask<UserView> AddUserAsync(
            UserView user,
            string password,
            CancellationToken cancellationToken = default);

        ValueTask<UserView> ModifyUserAsync(
            UserView user,
            CancellationToken cancellationToken = default);

        ValueTask ConfirmUserEmailAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        ValueTask<string> GenerateEmailConfirmationTokenAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        ValueTask<string> GeneratePasswordResetTokenAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        ValueTask LockUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        ValueTask UnlockUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        ValueTask ResetAccessFailedCountAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        ValueTask SetTwoFactorEnabledAsync(
            Guid userId,
            bool enabled,
            CancellationToken cancellationToken = default);

        ValueTask DisableUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        ValueTask EnableUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        ValueTask RemoveUserByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        ValueTask AddUserToRoleAsync(
            Guid userId,
            string roleName,
            CancellationToken cancellationToken = default);

        ValueTask RemoveUserFromRoleAsync(
            Guid userId,
            string roleName,
            CancellationToken cancellationToken = default);

        ValueTask<List<string>> RetrieveAllRoleNamesAsync(
            CancellationToken cancellationToken = default);
    }
}
