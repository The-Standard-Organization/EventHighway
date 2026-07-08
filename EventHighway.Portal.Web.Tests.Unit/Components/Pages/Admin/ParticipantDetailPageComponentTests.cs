// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using Bunit;
using EventHighway.Portal.Web.Models.Views.EventParticipants;
using EventHighway.Portal.Web.Models.Views.EventParticipantSecrets;
using EventHighway.Portal.Web.Models.Views.UserEventParticipants;
using EventHighway.Portal.Web.Models.Views.Users;
using EventHighway.Portal.Web.Services.Views.EventParticipants;
using EventHighway.Portal.Web.Services.Views.EventParticipantSecrets;
using EventHighway.Portal.Web.Services.Views.UserEventParticipants;
using EventHighway.Portal.Web.Services.Views.Users;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Pages.Admin
{
    public partial class ParticipantDetailPageComponentTests : BunitContext
    {
        private readonly Mock<IEventParticipantsViewService> eventParticipantsViewServiceMock;
        private readonly Mock<IEventParticipantSecretsViewService> secretsViewServiceMock;
        private readonly Mock<IUserEventParticipantsViewService>
            userEventParticipantsViewServiceMock;
        private readonly Mock<IUsersViewService> usersViewServiceMock;

        public ParticipantDetailPageComponentTests()
        {
            this.eventParticipantsViewServiceMock = new Mock<IEventParticipantsViewService>();
            this.secretsViewServiceMock = new Mock<IEventParticipantSecretsViewService>();
            this.userEventParticipantsViewServiceMock =
                new Mock<IUserEventParticipantsViewService>();
            this.usersViewServiceMock = new Mock<IUsersViewService>();

            this.secretsViewServiceMock.Setup(service =>
                service.RetrieveSecretsByParticipantAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<EventParticipantSecretView>());

            this.userEventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveAssociationsByParticipantIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<UserEventParticipantView>());

            Services.AddSingleton(this.eventParticipantsViewServiceMock.Object);
            Services.AddSingleton(this.secretsViewServiceMock.Object);
            Services.AddSingleton(this.userEventParticipantsViewServiceMock.Object);
            Services.AddSingleton(this.usersViewServiceMock.Object);
            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static EventParticipantView CreateRandomParticipant(Guid participantId) =>
            new EventParticipantView
            {
                Id = participantId,
                Name = GetRandomString(),
                Description = GetRandomString(),
                IsActive = true
            };

        private static UserEventParticipantView CreateRandomAssociationView(Guid participantId) =>
            new UserEventParticipantView
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = GetRandomString(),
                UserEmail = GetRandomString(),
                EventParticipantId = participantId
            };

        private static UserView CreateRandomUserView() =>
            new UserView
            {
                Id = Guid.NewGuid(),
                UserName = GetRandomString(),
                Email = GetRandomString(),
                Roles = new List<string>()
            };
    }
}
