// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Bunit;
using Bunit.TestDoubles;
using EventHighway.Portal.Web.Models.Views.EventParticipants;
using EventHighway.Portal.Web.Models.Views.UserEventParticipants;
using EventHighway.Portal.Web.Services.Views.EventParticipants;
using EventHighway.Portal.Web.Services.Views.UserEventParticipants;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Pages.MyAccount
{
    public partial class MyParticipantsPageComponentTests : BunitContext
    {
        private readonly Mock<IUserEventParticipantsViewService>
            userEventParticipantsViewServiceMock;
        private readonly Mock<IEventParticipantsViewService> eventParticipantsViewServiceMock;

        public MyParticipantsPageComponentTests()
        {
            this.userEventParticipantsViewServiceMock =
                new Mock<IUserEventParticipantsViewService>();
            this.eventParticipantsViewServiceMock = new Mock<IEventParticipantsViewService>();

            Services.AddSingleton(this.userEventParticipantsViewServiceMock.Object);
            Services.AddSingleton(this.eventParticipantsViewServiceMock.Object);
            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        private BunitAuthorizationContext AuthorizeUser(Guid userId, params string[] roles)
        {
            BunitAuthorizationContext authorizationContext = AddAuthorization();
            authorizationContext.SetAuthorized("user");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            authorizationContext.SetClaims(claims.ToArray());

            return authorizationContext;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static UserEventParticipantView CreateRandomAssociationView(
            Guid userId,
            Guid participantId) =>
            new UserEventParticipantView
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                EventParticipantId = participantId,
                EventParticipantName = GetRandomString()
            };

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
