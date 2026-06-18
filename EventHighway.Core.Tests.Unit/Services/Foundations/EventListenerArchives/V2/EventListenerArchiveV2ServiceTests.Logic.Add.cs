// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListenerArchives.V2
{
    public partial class EventListenerArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldAddEventListenerArchiveV2Async()
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            EventListenerArchiveV2 randomEventListenerArchiveV2 =
                CreateRandomEventListenerArchiveV2();

            EventListenerArchiveV2 inputEventListenerArchiveV2 =
                randomEventListenerArchiveV2;

            EventListenerArchiveV2 insertedEventListenerArchiveV2 =
                inputEventListenerArchiveV2;

            EventListenerArchiveV2 expectedEventListenerArchiveV2 =
                insertedEventListenerArchiveV2.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertEventListenerArchiveV2Async(
                    inputEventListenerArchiveV2,
                    cancellationToken))
                        .ReturnsAsync(insertedEventListenerArchiveV2);

            // when
            EventListenerArchiveV2 actualEventListenerArchiveV2 =
                await this.eventListenerArchiveV2Service
                    .AddEventListenerArchiveV2Async(
                        inputEventListenerArchiveV2,
                        cancellationToken);

            // then
            actualEventListenerArchiveV2.Should().BeEquivalentTo(
                expectedEventListenerArchiveV2);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerArchiveV2Async(
                    inputEventListenerArchiveV2,
                    cancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
