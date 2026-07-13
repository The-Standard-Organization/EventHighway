// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Bunit;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventArchives;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEventArchives;
using EventHighway.Portal.Web.Services.Views.Foundations.EventArchives;
using EventHighway.Portal.Web.Services.Views.Foundations.ListenerEventArchives;
using EventHighway.Portal.Web.Services.Views.Foundations.Replays;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Pages.Admin
{
    public partial class EventArchiveDetailPageComponentTests : BunitContext
    {
        private readonly Mock<IEventArchivesViewService> eventArchivesViewServiceMock;
        private readonly Mock<IListenerEventArchivesViewService> listenerEventArchivesViewServiceMock;
        private readonly Mock<IReplayViewService> replayViewServiceMock;

        public EventArchiveDetailPageComponentTests()
        {
            this.eventArchivesViewServiceMock = new Mock<IEventArchivesViewService>();
            this.listenerEventArchivesViewServiceMock =
                new Mock<IListenerEventArchivesViewService>();
            this.replayViewServiceMock = new Mock<IReplayViewService>();

            Services.AddSingleton(this.eventArchivesViewServiceMock.Object);
            Services.AddSingleton(this.listenerEventArchivesViewServiceMock.Object);
            Services.AddSingleton(this.replayViewServiceMock.Object);

            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static EventArchiveView CreateRandomArchive() =>
            new EventArchiveView
            {
                Id = Guid.NewGuid(),
                EventName = GetRandomString(),
                Content = GetRandomString(),
                Type = "Immediate",
                Status = "Active",
                EventAddressV2Id = Guid.NewGuid(),
                EventAddressName = GetRandomString()
            };

        private static ListenerEventArchiveView CreateListenerArchive(
            Guid eventArchiveId, string status) =>
            new ListenerEventArchiveView
            {
                Id = Guid.NewGuid(),
                Status = status,
                ResponseCode = status == "Success" ? "200" : "503",
                ResponseMessage = status,
                EventV2Id = Guid.NewGuid(),
                EventAddressV2Id = Guid.NewGuid(),
                EventListenerV2Id = Guid.NewGuid(),
                EventArchiveV2Id = eventArchiveId,
                ListenerName = GetRandomString()
            };
    }
}
