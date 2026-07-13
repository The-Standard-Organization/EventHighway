// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventAddresses;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventAddresses
{
    public partial class EventAddressesViewServiceTests
    {
        [Fact]
        public async Task ShouldRemoveAddressByIdAsync()
        {
            // given
            Guid addressId = Guid.NewGuid();

            EventAddressV2 removedAddress = CreateRandomAddresses(count: 1)[0];
            removedAddress.Id = addressId;

            EventAddressView expectedView =
                MapToViews(new[] { removedAddress })[0];

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RemoveEventAddressV2ByIdAsync(
                    addressId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(removedAddress);

            // when
            EventAddressView actualView =
                await this.eventAddressesViewService.RemoveAddressByIdAsync(
                    addressId, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RemoveEventAddressV2ByIdAsync(
                    addressId, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
