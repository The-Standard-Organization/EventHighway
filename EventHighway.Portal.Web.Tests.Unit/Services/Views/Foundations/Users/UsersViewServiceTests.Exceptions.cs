// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.Users;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Users.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.Users
{
    public partial class UsersViewServiceTests
    {
        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllUsersWhenErrorOccurs()
        {
            // given
            var serviceException = new Exception();

            var failedUsersViewServiceException =
                new FailedUsersViewServiceException(innerException: serviceException);

            var expectedUsersViewServiceException =
                new UsersViewServiceException(innerException: failedUsersViewServiceException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectAllUsers())
                    .Throws(serviceException);

            // when
            ValueTask<List<Models.Services.Views.Foundations.Users.UserView>> retrieveTask =
                this.usersViewService.RetrieveAllUsersAsync(
                    TestContext.Current.CancellationToken);

            UsersViewServiceException actualException =
                await Assert.ThrowsAsync<UsersViewServiceException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedUsersViewServiceException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectAllUsers(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUsersViewServiceException))),
                        Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveUserFromRoleWhenLastAdministrator()
        {
            // given
            AppUser randomUser = CreateRandomUsers(count: 1)[0];
            Guid inputUserId = randomUser.Id;
            string administratorsRole = "Administrators";

            var soleAdministrators = new List<AppUser> { randomUser };

            var expectedLastAdministratorException =
                new LastAdministratorUsersViewException();

            var expectedUsersViewValidationException =
                new UsersViewValidationException(
                    innerException: expectedLastAdministratorException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUsersInRoleAsync(administratorsRole))
                    .ReturnsAsync(soleAdministrators);

            // when
            ValueTask removeTask =
                this.usersViewService.RemoveUserFromRoleAsync(
                    inputUserId, administratorsRole, TestContext.Current.CancellationToken);

            UsersViewValidationException actualException =
                await Assert.ThrowsAsync<UsersViewValidationException>(
                    removeTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedUsersViewValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUsersInRoleAsync(administratorsRole), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUsersViewValidationException))),
                        Times.Once);

            this.identityBrokerMock.Verify(broker =>
                broker.DeleteUserFromRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()),
                    Times.Never);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnLockUserWhenLastAdministrator()
        {
            // given
            AppUser randomUser = CreateRandomUsers(count: 1)[0];
            Guid inputUserId = randomUser.Id;

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync(randomUser);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserRolesAsync(randomUser))
                    .ReturnsAsync(new List<string> { "Administrators" });

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUsersInRoleAsync("Administrators"))
                    .ReturnsAsync(new List<AppUser> { randomUser });

            // when
            ValueTask lockTask =
                this.usersViewService.LockUserAsync(
                    inputUserId, TestContext.Current.CancellationToken);

            await Assert.ThrowsAsync<UsersViewValidationException>(lockTask.AsTask);

            // then
            this.identityBrokerMock.Verify(broker =>
                broker.SetLockoutEndDateAsync(
                    It.IsAny<AppUser>(), It.IsAny<DateTimeOffset?>()),
                        Times.Never);
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnDisableUserWhenLastAdministrator()
        {
            // given
            AppUser randomUser = CreateRandomUsers(count: 1)[0];
            Guid inputUserId = randomUser.Id;

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync(randomUser);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserRolesAsync(randomUser))
                    .ReturnsAsync(new List<string> { "Administrators" });

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUsersInRoleAsync("Administrators"))
                    .ReturnsAsync(new List<AppUser> { randomUser });

            // when
            ValueTask disableTask =
                this.usersViewService.DisableUserAsync(
                    inputUserId, TestContext.Current.CancellationToken);

            await Assert.ThrowsAsync<UsersViewValidationException>(disableTask.AsTask);

            // then
            this.identityBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(It.IsAny<AppUser>()), Times.Never);
        }

        private static System.Linq.Expressions.Expression<Func<Xeption, bool>>
            SameExceptionAs(Xeption expectedException) =>
            actualException =>
                actualException.SameExceptionAs(expectedException);
    }
}
