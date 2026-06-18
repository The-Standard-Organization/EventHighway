// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListenerArchives.V2
{
    public partial class EventListenerArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldBulkRemoveEventListenerArchiveV2sAsync()
        {
            // given
            IQueryable<EventListenerArchiveV2> randomEventListenerArchiveV2s =
                CreateRandomEventListenerArchiveV2s();

            IEnumerable<EventListenerArchiveV2> inputEventListenerArchiveV2s =
                randomEventListenerArchiveV2s;

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteBulkEventListenerArchiveV2sAsync(
                    inputEventListenerArchiveV2s,
                        It.IsAny<CancellationToken>()))
                            .Returns(ValueTask.CompletedTask);

            // when
            await this.eventListenerArchiveV2Service.BulkRemoveEventListenerArchiveV2sAsync(
                inputEventListenerArchiveV2s,
                    TestContext.Current.CancellationToken);

            // then
            this.storageBrokerMock.Verify(broker =>
                broker.DeleteBulkEventListenerArchiveV2sAsync(
                    inputEventListenerArchiveV2s,
                        It.IsAny<CancellationToken>()),
                            Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
