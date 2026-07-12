// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Bunit;
using Bunit.TestDoubles;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipantSecrets;
using EventHighway.Portal.Web.Services.Views.Foundations.EventParticipants;
using EventHighway.Portal.Web.Services.Views.Foundations.EventParticipantSecrets;
using EventHighway.Portal.Web.Services.Views.Foundations.UserEventParticipants;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Pages.MyAccount
{
    public partial class MyParticipantDetailPageComponentTests : BunitContext
    {
        private readonly Mock<IUserEventParticipantsViewService>
            userEventParticipantsViewServiceMock;
        private readonly Mock<IEventParticipantsViewService> eventParticipantsViewServiceMock;
        private readonly Mock<IEventParticipantSecretsViewService> secretsViewServiceMock;

        public MyParticipantDetailPageComponentTests()
        {
            this.userEventParticipantsViewServiceMock =
                new Mock<IUserEventParticipantsViewService>();
            this.eventParticipantsViewServiceMock = new Mock<IEventParticipantsViewService>();
            this.secretsViewServiceMock = new Mock<IEventParticipantSecretsViewService>();

            this.secretsViewServiceMock.Setup(service =>
                service.RetrieveSecretsByParticipantAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<EventParticipantSecretView>());

            Services.AddSingleton(this.userEventParticipantsViewServiceMock.Object);
            Services.AddSingleton(this.eventParticipantsViewServiceMock.Object);
            Services.AddSingleton(this.secretsViewServiceMock.Object);
            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        private void AuthorizeUser(Guid userId)
        {
            BunitAuthorizationContext authorizationContext = AddAuthorization();
            authorizationContext.SetAuthorized("user");
            authorizationContext.SetClaims(
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static EventParticipantView CreateRandomParticipant(Guid participantId) =>
            new EventParticipantView
            {
                Id = participantId,
                Name = GetRandomString(),
                Description = GetRandomString(),
                ContactEmail = GetRandomString(),
                IsActive = true
            };
    }
}
