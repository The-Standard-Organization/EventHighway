// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Portal.Web.Brokers.Identities;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.Users;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Users;
using EventHighway.Portal.Web.Services.Views.Foundations.Users;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.Users
{
    public partial class UsersViewServiceTests
    {
        private readonly Mock<IIdentityBroker> identityBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IUsersViewService usersViewService;

        public UsersViewServiceTests()
        {
            this.identityBrokerMock = new Mock<IIdentityBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.usersViewService = new UsersViewService(
                identityBroker: this.identityBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static List<AppUser> CreateRandomUsers(int count) =>
            Enumerable.Range(0, count).Select(_ => new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = GetRandomString(),
                Email = GetRandomString()
            }).ToList();

        private static List<UserView> MapToViews(IEnumerable<AppUser> users) =>
            users.Select(user => new UserView
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = new List<string>()
            }).ToList();
    }
}
