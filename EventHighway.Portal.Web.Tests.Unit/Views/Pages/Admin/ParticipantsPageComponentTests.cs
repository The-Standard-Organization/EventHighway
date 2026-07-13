// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Bunit;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants;
using EventHighway.Portal.Web.Services.Views.Foundations.EventParticipants;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Pages.Admin
{
    public partial class ParticipantsPageComponentTests : BunitContext
    {
        private readonly Mock<IEventParticipantsViewService> participantsViewServiceMock;

        public ParticipantsPageComponentTests()
        {
            this.participantsViewServiceMock = new Mock<IEventParticipantsViewService>();
            Services.AddSingleton(this.participantsViewServiceMock.Object);
            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static List<EventParticipantView> CreateRandomParticipants(int count) =>
            Enumerable.Range(0, count).Select(_ => new EventParticipantView
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString(),
                Description = GetRandomString(),
                IsActive = true
            }).ToList();
    }
}
