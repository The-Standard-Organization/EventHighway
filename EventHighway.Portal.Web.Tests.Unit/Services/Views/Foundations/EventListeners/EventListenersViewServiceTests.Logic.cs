// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventListeners;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventListeners
{
    public partial class EventListenersViewServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveListenersByAddressAsync()
        {
            // given
            Guid addressId = Guid.NewGuid();
            List<EventListenerV2> randomListeners = CreateRandomListeners(addressId, count: 3);
            IQueryable<EventListenerV2> returnedListeners = randomListeners.AsQueryable();
            List<EventListenerView> expectedViews = MapToViews(randomListeners);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveEventListenerV2sByEventAddressIdAsync(
                    addressId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(returnedListeners);

            // when
            List<EventListenerView> actualViews =
                await this.eventListenersViewService.RetrieveListenersByAddressAsync(
                    addressId, TestContext.Current.CancellationToken);

            // then
            actualViews.Should().BeEquivalentTo(expectedViews);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveEventListenerV2sByEventAddressIdAsync(
                    addressId, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
