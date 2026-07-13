// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Bunit;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Users;
using EventHighway.Portal.Web.Services.Views.Foundations.Users;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Pages.Admin
{
    public partial class UsersPageComponentTests : BunitContext
    {
        private readonly Mock<IUsersViewService> usersViewServiceMock;

        public UsersPageComponentTests()
        {
            this.usersViewServiceMock = new Mock<IUsersViewService>();
            Services.AddSingleton(this.usersViewServiceMock.Object);
            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static List<UserView> CreateRandomUsers(int count) =>
            Enumerable.Range(0, count).Select(_ => new UserView
            {
                Id = Guid.NewGuid(),
                UserName = GetRandomString(),
                Email = GetRandomString()
            }).ToList();
    }
}
