// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.Users;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Users;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Users.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.Users
{
    public partial class UsersViewServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveUserByIdWhenUserNotFoundAsync()
        {
            // given
            Guid inputUserId = Guid.NewGuid();

            var notFoundUsersViewException =
                new NotFoundUsersViewException(inputUserId);

            var expectedUsersViewValidationException =
                new UsersViewValidationException(
                    innerException: notFoundUsersViewException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync((AppUser)null);

            // when
            ValueTask<UserView> retrieveTask =
                this.usersViewService.RetrieveUserByIdAsync(
                    inputUserId, TestContext.Current.CancellationToken);

            UsersViewValidationException actualException =
                await Assert.ThrowsAsync<UsersViewValidationException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedUsersViewValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUsersViewValidationException))), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveUserByIdWhenUserNotFoundAsync()
        {
            // given
            Guid inputUserId = Guid.NewGuid();

            var notFoundUsersViewException =
                new NotFoundUsersViewException(inputUserId);

            var expectedUsersViewValidationException =
                new UsersViewValidationException(
                    innerException: notFoundUsersViewException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync((AppUser)null);

            // when
            ValueTask removeTask =
                this.usersViewService.RemoveUserByIdAsync(
                    inputUserId, TestContext.Current.CancellationToken);

            UsersViewValidationException actualException =
                await Assert.ThrowsAsync<UsersViewValidationException>(
                    removeTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedUsersViewValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUsersViewValidationException))), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddUserToRoleWhenUserNotFoundAsync()
        {
            // given
            Guid inputUserId = Guid.NewGuid();
            string inputRoleName = GetRandomString();

            var notFoundUsersViewException =
                new NotFoundUsersViewException(inputUserId);

            var expectedUsersViewValidationException =
                new UsersViewValidationException(
                    innerException: notFoundUsersViewException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync((AppUser)null);

            // when
            ValueTask addToRoleTask =
                this.usersViewService.AddUserToRoleAsync(
                    inputUserId, inputRoleName, TestContext.Current.CancellationToken);

            UsersViewValidationException actualException =
                await Assert.ThrowsAsync<UsersViewValidationException>(
                    addToRoleTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedUsersViewValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUsersViewValidationException))), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveUserFromRoleWhenUserNotFoundAsync()
        {
            // given
            Guid inputUserId = Guid.NewGuid();
            string inputRoleName = GetRandomString();

            var notFoundUsersViewException =
                new NotFoundUsersViewException(inputUserId);

            var expectedUsersViewValidationException =
                new UsersViewValidationException(
                    innerException: notFoundUsersViewException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync((AppUser)null);

            // when
            ValueTask removeFromRoleTask =
                this.usersViewService.RemoveUserFromRoleAsync(
                    inputUserId, inputRoleName, TestContext.Current.CancellationToken);

            UsersViewValidationException actualException =
                await Assert.ThrowsAsync<UsersViewValidationException>(
                    removeFromRoleTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedUsersViewValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUsersViewValidationException))), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyUserWhenUserNotFoundAsync()
        {
            // given
            var inputUser = new UserView { Id = Guid.NewGuid() };
            Guid inputUserId = inputUser.Id;

            var notFoundUsersViewException =
                new NotFoundUsersViewException(inputUserId);

            var expectedUsersViewValidationException =
                new UsersViewValidationException(
                    innerException: notFoundUsersViewException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync((AppUser)null);

            // when
            ValueTask<UserView> modifyTask =
                this.usersViewService.ModifyUserAsync(
                    inputUser, TestContext.Current.CancellationToken);

            UsersViewValidationException actualException =
                await Assert.ThrowsAsync<UsersViewValidationException>(
                    modifyTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedUsersViewValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUsersViewValidationException))), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnConfirmUserEmailWhenUserNotFoundAsync()
        {
            // given
            Guid inputUserId = Guid.NewGuid();

            var notFoundUsersViewException =
                new NotFoundUsersViewException(inputUserId);

            var expectedUsersViewValidationException =
                new UsersViewValidationException(
                    innerException: notFoundUsersViewException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync((AppUser)null);

            // when
            ValueTask confirmTask =
                this.usersViewService.ConfirmUserEmailAsync(
                    inputUserId, TestContext.Current.CancellationToken);

            UsersViewValidationException actualException =
                await Assert.ThrowsAsync<UsersViewValidationException>(
                    confirmTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedUsersViewValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUsersViewValidationException))), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnGenerateEmailConfirmationTokenWhenUserNotFoundAsync()
        {
            // given
            Guid inputUserId = Guid.NewGuid();

            var notFoundUsersViewException =
                new NotFoundUsersViewException(inputUserId);

            var expectedUsersViewValidationException =
                new UsersViewValidationException(
                    innerException: notFoundUsersViewException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync((AppUser)null);

            // when
            ValueTask<string> generateTask =
                this.usersViewService.GenerateEmailConfirmationTokenAsync(
                    inputUserId, TestContext.Current.CancellationToken);

            UsersViewValidationException actualException =
                await Assert.ThrowsAsync<UsersViewValidationException>(
                    generateTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedUsersViewValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUsersViewValidationException))), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnGeneratePasswordResetTokenWhenUserNotFoundAsync()
        {
            // given
            Guid inputUserId = Guid.NewGuid();

            var notFoundUsersViewException =
                new NotFoundUsersViewException(inputUserId);

            var expectedUsersViewValidationException =
                new UsersViewValidationException(
                    innerException: notFoundUsersViewException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync((AppUser)null);

            // when
            ValueTask<string> generateTask =
                this.usersViewService.GeneratePasswordResetTokenAsync(
                    inputUserId, TestContext.Current.CancellationToken);

            UsersViewValidationException actualException =
                await Assert.ThrowsAsync<UsersViewValidationException>(
                    generateTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedUsersViewValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUsersViewValidationException))), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnLockUserWhenUserNotFoundAsync()
        {
            // given
            Guid inputUserId = Guid.NewGuid();

            var notFoundUsersViewException =
                new NotFoundUsersViewException(inputUserId);

            var expectedUsersViewValidationException =
                new UsersViewValidationException(
                    innerException: notFoundUsersViewException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync((AppUser)null);

            // when
            ValueTask lockTask =
                this.usersViewService.LockUserAsync(
                    inputUserId, TestContext.Current.CancellationToken);

            UsersViewValidationException actualException =
                await Assert.ThrowsAsync<UsersViewValidationException>(
                    lockTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedUsersViewValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUsersViewValidationException))), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnUnlockUserWhenUserNotFoundAsync()
        {
            // given
            Guid inputUserId = Guid.NewGuid();

            var notFoundUsersViewException =
                new NotFoundUsersViewException(inputUserId);

            var expectedUsersViewValidationException =
                new UsersViewValidationException(
                    innerException: notFoundUsersViewException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync((AppUser)null);

            // when
            ValueTask unlockTask =
                this.usersViewService.UnlockUserAsync(
                    inputUserId, TestContext.Current.CancellationToken);

            UsersViewValidationException actualException =
                await Assert.ThrowsAsync<UsersViewValidationException>(
                    unlockTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedUsersViewValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUsersViewValidationException))), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnResetAccessFailedCountWhenUserNotFoundAsync()
        {
            // given
            Guid inputUserId = Guid.NewGuid();

            var notFoundUsersViewException =
                new NotFoundUsersViewException(inputUserId);

            var expectedUsersViewValidationException =
                new UsersViewValidationException(
                    innerException: notFoundUsersViewException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync((AppUser)null);

            // when
            ValueTask resetTask =
                this.usersViewService.ResetAccessFailedCountAsync(
                    inputUserId, TestContext.Current.CancellationToken);

            UsersViewValidationException actualException =
                await Assert.ThrowsAsync<UsersViewValidationException>(
                    resetTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedUsersViewValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUsersViewValidationException))), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnSetTwoFactorEnabledWhenUserNotFoundAsync()
        {
            // given
            Guid inputUserId = Guid.NewGuid();
            bool inputEnabled = true;

            var notFoundUsersViewException =
                new NotFoundUsersViewException(inputUserId);

            var expectedUsersViewValidationException =
                new UsersViewValidationException(
                    innerException: notFoundUsersViewException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync((AppUser)null);

            // when
            ValueTask setTwoFactorTask =
                this.usersViewService.SetTwoFactorEnabledAsync(
                    inputUserId, inputEnabled, TestContext.Current.CancellationToken);

            UsersViewValidationException actualException =
                await Assert.ThrowsAsync<UsersViewValidationException>(
                    setTwoFactorTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedUsersViewValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUsersViewValidationException))), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnDisableUserWhenUserNotFoundAsync()
        {
            // given
            Guid inputUserId = Guid.NewGuid();

            var notFoundUsersViewException =
                new NotFoundUsersViewException(inputUserId);

            var expectedUsersViewValidationException =
                new UsersViewValidationException(
                    innerException: notFoundUsersViewException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync((AppUser)null);

            // when
            ValueTask disableTask =
                this.usersViewService.DisableUserAsync(
                    inputUserId, TestContext.Current.CancellationToken);

            UsersViewValidationException actualException =
                await Assert.ThrowsAsync<UsersViewValidationException>(
                    disableTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedUsersViewValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUsersViewValidationException))), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnEnableUserWhenUserNotFoundAsync()
        {
            // given
            Guid inputUserId = Guid.NewGuid();

            var notFoundUsersViewException =
                new NotFoundUsersViewException(inputUserId);

            var expectedUsersViewValidationException =
                new UsersViewValidationException(
                    innerException: notFoundUsersViewException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync((AppUser)null);

            // when
            ValueTask enableTask =
                this.usersViewService.EnableUserAsync(
                    inputUserId, TestContext.Current.CancellationToken);

            UsersViewValidationException actualException =
                await Assert.ThrowsAsync<UsersViewValidationException>(
                    enableTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedUsersViewValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUsersViewValidationException))), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
