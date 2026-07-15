// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task ShouldRetrieveAllUsersAsync()
        {
            // given
            List<AppUser> randomUsers = CreateRandomUsers(count: 3);
            IQueryable<AppUser> returnedUsers = randomUsers.AsQueryable();
            List<UserView> expectedViews = MapToViews(randomUsers);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectAllUsers())
                    .Returns(returnedUsers);

            // when
            List<UserView> actualViews =
                await this.usersViewService.RetrieveAllUsersAsync(
                    TestContext.Current.CancellationToken);

            // then
            actualViews.Should().BeEquivalentTo(expectedViews);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectAllUsers(), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldAddUserToRoleAsync()
        {
            // given
            AppUser randomUser = CreateRandomUsers(count: 1)[0];
            Guid inputUserId = randomUser.Id;
            string inputRoleName = GetRandomString();

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync(randomUser);

            this.identityBrokerMock.Setup(broker =>
                broker.InsertUserToRoleAsync(randomUser, inputRoleName))
                    .ReturnsAsync(IdentityResult.Success);

            // when
            await this.usersViewService.AddUserToRoleAsync(
                inputUserId, inputRoleName, TestContext.Current.CancellationToken);

            // then
            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.identityBrokerMock.Verify(broker =>
                broker.InsertUserToRoleAsync(randomUser, inputRoleName), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRemoveUserFromNonAdministratorRoleAsync()
        {
            // given
            AppUser randomUser = CreateRandomUsers(count: 1)[0];
            Guid inputUserId = randomUser.Id;
            string inputRoleName = "Users";

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync(randomUser);

            this.identityBrokerMock.Setup(broker =>
                broker.DeleteUserFromRoleAsync(randomUser, inputRoleName))
                    .ReturnsAsync(IdentityResult.Success);

            // when
            await this.usersViewService.RemoveUserFromRoleAsync(
                inputUserId, inputRoleName, TestContext.Current.CancellationToken);

            // then
            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.identityBrokerMock.Verify(broker =>
                broker.DeleteUserFromRoleAsync(randomUser, inputRoleName), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
