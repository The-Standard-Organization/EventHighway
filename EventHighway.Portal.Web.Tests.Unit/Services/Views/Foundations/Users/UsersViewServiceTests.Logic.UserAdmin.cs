// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.Users;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.Users
{
    public partial class UsersViewServiceTests
    {
        [Fact]
        public async Task ShouldModifyUserAsync()
        {
            // given
            AppUser randomUser = CreateRandomUsers(count: 1)[0];

            var inputView = new UserView
            {
                Id = randomUser.Id,
                UserName = GetRandomString(),
                Email = GetRandomString(),
                PhoneNumber = GetRandomString()
            };

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(randomUser.Id))
                    .ReturnsAsync(randomUser);

            this.identityBrokerMock.Setup(broker =>
                broker.SetUserNameAsync(randomUser, inputView.UserName))
                    .ReturnsAsync(IdentityResult.Success);

            this.identityBrokerMock.Setup(broker =>
                broker.SetEmailAsync(randomUser, inputView.Email))
                    .ReturnsAsync(IdentityResult.Success);

            this.identityBrokerMock.Setup(broker =>
                broker.SetPhoneNumberAsync(randomUser, inputView.PhoneNumber))
                    .ReturnsAsync(IdentityResult.Success);

            // when
            await this.usersViewService.ModifyUserAsync(
                inputView, TestContext.Current.CancellationToken);

            // then
            this.identityBrokerMock.Verify(broker =>
                broker.SetUserNameAsync(randomUser, inputView.UserName), Times.Once);

            this.identityBrokerMock.Verify(broker =>
                broker.SetEmailAsync(randomUser, inputView.Email), Times.Once);

            this.identityBrokerMock.Verify(broker =>
                broker.SetPhoneNumberAsync(randomUser, inputView.PhoneNumber), Times.Once);
        }

        [Fact]
        public async Task ShouldConfirmUserEmailAsync()
        {
            // given
            AppUser randomUser = CreateRandomUsers(count: 1)[0];
            Guid inputUserId = randomUser.Id;
            string randomToken = GetRandomString();

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync(randomUser);

            this.identityBrokerMock.Setup(broker =>
                broker.GenerateEmailConfirmationTokenAsync(randomUser))
                    .ReturnsAsync(randomToken);

            this.identityBrokerMock.Setup(broker =>
                broker.ConfirmEmailAsync(randomUser, randomToken))
                    .ReturnsAsync(IdentityResult.Success);

            // when
            await this.usersViewService.ConfirmUserEmailAsync(
                inputUserId, TestContext.Current.CancellationToken);

            // then
            this.identityBrokerMock.Verify(broker =>
                broker.ConfirmEmailAsync(randomUser, randomToken), Times.Once);
        }

        [Fact]
        public async Task ShouldGeneratePasswordResetTokenAsync()
        {
            // given
            AppUser randomUser = CreateRandomUsers(count: 1)[0];
            Guid inputUserId = randomUser.Id;
            string expectedToken = GetRandomString();

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync(randomUser);

            this.identityBrokerMock.Setup(broker =>
                broker.GeneratePasswordResetTokenAsync(randomUser))
                    .ReturnsAsync(expectedToken);

            // when
            string actualToken = await this.usersViewService.GeneratePasswordResetTokenAsync(
                inputUserId, TestContext.Current.CancellationToken);

            // then
            actualToken.Should().Be(expectedToken);
        }

        [Fact]
        public async Task ShouldLockNonAdministratorUserAsync()
        {
            // given
            AppUser randomUser = CreateRandomUsers(count: 1)[0];
            Guid inputUserId = randomUser.Id;

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync(randomUser);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserRolesAsync(randomUser))
                    .ReturnsAsync(new List<string> { "Users" });

            this.identityBrokerMock.Setup(broker =>
                broker.SetLockoutEnabledAsync(randomUser, true))
                    .ReturnsAsync(IdentityResult.Success);

            this.identityBrokerMock.Setup(broker =>
                broker.SetLockoutEndDateAsync(randomUser, It.IsAny<DateTimeOffset?>()))
                    .ReturnsAsync(IdentityResult.Success);

            // when
            await this.usersViewService.LockUserAsync(
                inputUserId, TestContext.Current.CancellationToken);

            // then
            this.identityBrokerMock.Verify(broker =>
                broker.SetLockoutEnabledAsync(randomUser, true), Times.Once);

            this.identityBrokerMock.Verify(broker =>
                broker.SetLockoutEndDateAsync(randomUser, It.IsAny<DateTimeOffset?>()),
                    Times.Once);
        }

        [Fact]
        public async Task ShouldUnlockUserAsync()
        {
            // given
            AppUser randomUser = CreateRandomUsers(count: 1)[0];
            Guid inputUserId = randomUser.Id;

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync(randomUser);

            this.identityBrokerMock.Setup(broker =>
                broker.SetLockoutEndDateAsync(randomUser, null))
                    .ReturnsAsync(IdentityResult.Success);

            // when
            await this.usersViewService.UnlockUserAsync(
                inputUserId, TestContext.Current.CancellationToken);

            // then
            this.identityBrokerMock.Verify(broker =>
                broker.SetLockoutEndDateAsync(randomUser, null), Times.Once);
        }

        [Fact]
        public async Task ShouldResetAccessFailedCountAsync()
        {
            // given
            AppUser randomUser = CreateRandomUsers(count: 1)[0];
            Guid inputUserId = randomUser.Id;

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync(randomUser);

            this.identityBrokerMock.Setup(broker =>
                broker.ResetAccessFailedCountAsync(randomUser))
                    .ReturnsAsync(IdentityResult.Success);

            // when
            await this.usersViewService.ResetAccessFailedCountAsync(
                inputUserId, TestContext.Current.CancellationToken);

            // then
            this.identityBrokerMock.Verify(broker =>
                broker.ResetAccessFailedCountAsync(randomUser), Times.Once);
        }

        [Fact]
        public async Task ShouldDisableTwoFactorAndResetAuthenticatorKeyAsync()
        {
            // given
            AppUser randomUser = CreateRandomUsers(count: 1)[0];
            Guid inputUserId = randomUser.Id;

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync(randomUser);

            this.identityBrokerMock.Setup(broker =>
                broker.SetTwoFactorEnabledAsync(randomUser, false))
                    .ReturnsAsync(IdentityResult.Success);

            this.identityBrokerMock.Setup(broker =>
                broker.ResetAuthenticatorKeyAsync(randomUser))
                    .ReturnsAsync(IdentityResult.Success);

            // when
            await this.usersViewService.SetTwoFactorEnabledAsync(
                inputUserId, enabled: false, TestContext.Current.CancellationToken);

            // then
            this.identityBrokerMock.Verify(broker =>
                broker.SetTwoFactorEnabledAsync(randomUser, false), Times.Once);

            this.identityBrokerMock.Verify(broker =>
                broker.ResetAuthenticatorKeyAsync(randomUser), Times.Once);
        }

        [Fact]
        public async Task ShouldDisableNonAdministratorUserAsync()
        {
            // given
            AppUser randomUser = CreateRandomUsers(count: 1)[0];
            Guid inputUserId = randomUser.Id;

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync(randomUser);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserRolesAsync(randomUser))
                    .ReturnsAsync(new List<string> { "Users" });

            this.identityBrokerMock.Setup(broker =>
                broker.UpdateUserAsync(randomUser))
                    .ReturnsAsync(IdentityResult.Success);

            this.identityBrokerMock.Setup(broker =>
                broker.SetLockoutEnabledAsync(randomUser, true))
                    .ReturnsAsync(IdentityResult.Success);

            this.identityBrokerMock.Setup(broker =>
                broker.SetLockoutEndDateAsync(randomUser, It.IsAny<DateTimeOffset?>()))
                    .ReturnsAsync(IdentityResult.Success);

            // when
            await this.usersViewService.DisableUserAsync(
                inputUserId, TestContext.Current.CancellationToken);

            // then
            randomUser.IsDisabled.Should().BeTrue();

            this.identityBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(randomUser), Times.Once);

            this.identityBrokerMock.Verify(broker =>
                broker.SetLockoutEndDateAsync(randomUser, It.IsAny<DateTimeOffset?>()),
                    Times.Once);
        }
    }
}
