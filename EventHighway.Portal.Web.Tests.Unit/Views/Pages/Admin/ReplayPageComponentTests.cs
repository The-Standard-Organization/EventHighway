// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Bunit;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventAddresses;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventListeners;
using EventHighway.Portal.Web.Services.Views.Foundations.EventAddresses;
using EventHighway.Portal.Web.Services.Views.Foundations.EventListeners;
using EventHighway.Portal.Web.Services.Views.Foundations.Replays;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Pages.Admin
{
    public partial class ReplayPageComponentTests : BunitContext
    {
        private readonly Mock<IReplayViewService> replayViewServiceMock;
        private readonly Mock<IEventAddressesViewService> addressesViewServiceMock;
        private readonly Mock<IEventListenersViewService> listenersViewServiceMock;

        public ReplayPageComponentTests()
        {
            this.replayViewServiceMock = new Mock<IReplayViewService>();
            this.addressesViewServiceMock = new Mock<IEventAddressesViewService>();
            this.listenersViewServiceMock = new Mock<IEventListenersViewService>();

            Services.AddSingleton(this.replayViewServiceMock.Object);
            Services.AddSingleton(this.addressesViewServiceMock.Object);
            Services.AddSingleton(this.listenersViewServiceMock.Object);

            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static List<EventAddressView> CreateRandomAddresses(int count) =>
            Enumerable.Range(0, count).Select(_ => new EventAddressView
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString()
            }).ToList();

        private static List<EventListenerView> CreateRandomListeners(Guid addressId, int count) =>
            Enumerable.Range(0, count).Select(_ => new EventListenerView
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString(),
                EventAddressV2Id = addressId
            }).ToList();
    }
}
