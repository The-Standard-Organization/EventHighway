// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using Bunit;
using EventHighway.Portal.Web.Models.Views.EventParticipants;
using EventHighway.Portal.Web.Models.Views.UserEventParticipants;
using EventHighway.Portal.Web.Models.Views.Users;
using EventHighway.Portal.Web.Services.Views.EventParticipants;
using EventHighway.Portal.Web.Services.Views.UserEventParticipants;
using EventHighway.Portal.Web.Services.Views.Users;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Pages.Admin
{
    public partial class UserDetailPageComponentTests : BunitContext
    {
        private readonly Mock<IUsersViewService> usersViewServiceMock;
        private readonly Mock<IUserEventParticipantsViewService>
            userEventParticipantsViewServiceMock;
        private readonly Mock<IEventParticipantsViewService> eventParticipantsViewServiceMock;

        public UserDetailPageComponentTests()
        {
            this.usersViewServiceMock = new Mock<IUsersViewService>();
            this.userEventParticipantsViewServiceMock =
                new Mock<IUserEventParticipantsViewService>();
            this.eventParticipantsViewServiceMock = new Mock<IEventParticipantsViewService>();

            this.userEventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveAssociationsByUserIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<UserEventParticipantView>());

            Services.AddSingleton(this.usersViewServiceMock.Object);
            Services.AddSingleton(this.userEventParticipantsViewServiceMock.Object);
            Services.AddSingleton(this.eventParticipantsViewServiceMock.Object);
            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Guid GetRandomGuid() =>
            Guid.NewGuid();

        private static UserView CreateRandomUser(List<string> roles) =>
            new UserView
            {
                Id = Guid.NewGuid(),
                UserName = GetRandomString(),
                Email = GetRandomString(),
                Roles = roles
            };

        private static UserEventParticipantView CreateRandomAssociationView(Guid userId) =>
            new UserEventParticipantView
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                EventParticipantId = Guid.NewGuid(),
                EventParticipantName = GetRandomString()
            };

        private static EventParticipantView CreateRandomParticipantView() =>
            new EventParticipantView
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString(),
                Description = GetRandomString(),
                IsActive = true
            };
    }
}
