// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Bunit;
using EventHighway.Portal.Web.Models.Views.EventArchives;
using EventHighway.Portal.Web.Services.Views.EventArchives;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Pages.Admin
{
    public partial class EventArchivesPageComponentTests : BunitContext
    {
        private readonly Mock<IEventArchivesViewService> eventArchivesViewServiceMock;

        public EventArchivesPageComponentTests()
        {
            this.eventArchivesViewServiceMock = new Mock<IEventArchivesViewService>();
            Services.AddSingleton(this.eventArchivesViewServiceMock.Object);
            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static EventArchiveView CreateArchive(
            string status,
            int listenerEventCount,
            int succeededListenerEventCount) =>
            new EventArchiveView
            {
                Id = Guid.NewGuid(),
                EventName = GetRandomString(),
                Type = "Immediate",
                Status = status,
                ListenerEventCount = listenerEventCount,
                SucceededListenerEventCount = succeededListenerEventCount,
                EventAddressV2Id = Guid.NewGuid(),
                EventAddressName = GetRandomString(),
                ArchivedDate = DateTimeOffset.UtcNow
            };
    }
}
