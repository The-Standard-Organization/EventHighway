// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Replays;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.Replays
{
    public partial class ReplayViewServiceTests
    {
        [Fact]
        public async Task ShouldReplayAsync()
        {
            // given
            Guid addressId = Guid.NewGuid();
            DateTimeOffset startDate = GetRandomDateTimeOffset();
            DateTimeOffset endDate = startDate.AddDays(1);

            ReplayRequestView replayRequest = new ReplayRequestView
            {
                EventAddressV2Id = addressId,
                StartDate = startDate,
                EndDate = endDate
            };

            // when
            await this.replayViewService.ReplayAsync(
                replayRequest, TestContext.Current.CancellationToken);

            // then
            this.eventHighwayBrokerMock.Verify(broker =>
                broker.ReplayEventArchiveV2sAsync(
                    addressId,
                    It.Is<IEnumerable<Guid>>(ids => !ids.Any()),
                    startDate,
                    endDate,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.ProcessReplayedListenerEventV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
