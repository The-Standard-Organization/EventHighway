// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventAddresses;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventAddresses
{
    public partial class EventAddressesViewServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllAddressesAsync()
        {
            // given
            List<EventAddressV2> randomAddresses = CreateRandomAddresses(count: 3);
            IReadOnlyList<EventAddressV2> returnedAddresses = randomAddresses;
            List<EventAddressView> expectedViews = MapToViews(randomAddresses);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventAddressV2sAsync(
                    It.IsAny<EventAddressV2Query>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(returnedAddresses);

            // when
            List<EventAddressView> actualViews =
                await this.eventAddressesViewService.RetrieveAllAddressesAsync(
                    TestContext.Current.CancellationToken);

            // then
            actualViews.Should().BeEquivalentTo(expectedViews);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventAddressV2sAsync(
                    It.IsAny<EventAddressV2Query>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRegisterAddressAsync()
        {
            // given
            EventAddressView inputView = CreateRandomAddressView();
            DateTimeOffset now = GetRandomDateTimeOffset();

            var returnedAddress = new EventAddressV2
            {
                Id = Guid.NewGuid(),
                Name = inputView.Name,
                Description = inputView.Description,
                CreatedDate = now,
                UpdatedDate = now
            };

            EventAddressView expectedView = MapToViews(new[] { returnedAddress })[0];

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RegisterEventAddressV2Async(
                    It.IsAny<EventAddressV2>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(returnedAddress);

            // when
            EventAddressView actualView =
                await this.eventAddressesViewService.RegisterAddressAsync(
                    inputView, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(), Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RegisterEventAddressV2Async(
                    It.Is<EventAddressV2>(address =>
                        address.Name == inputView.Name
                        && address.Description == inputView.Description
                        && address.CreatedDate == now
                        && address.UpdatedDate == now),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
