// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V2
{
    public partial class ListenerEventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveBatchOfRetryListenerEventV2sWithoutTakeAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int inputTake = 0;

            IEnumerable<ListenerEventV2> retrievedListenerEventV2s =
                CreateRandomListenerEventV2s();

            IEnumerable<ListenerEventV2> expectedListenerEventV2s =
                retrievedListenerEventV2s;

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveRetryBatchListenerEventV2sWithEventWithEventListenerAsync(
                    inputTake,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedListenerEventV2s);

            // when
            IEnumerable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2ProcessingService
                    .RetrieveBatchOfRetryListenerEventV2sAsync(inputTake, randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveRetryBatchListenerEventV2sWithEventWithEventListenerAsync(
                    inputTake,
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveBatchOfRetryListenerEventV2sWithTakeAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();
            int inputTake = randomTake;

            IEnumerable<ListenerEventV2> retrievedListenerEventV2s =
                CreateRandomListenerEventV2s();

            IEnumerable<ListenerEventV2> expectedListenerEventV2s =
                retrievedListenerEventV2s;

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveRetryBatchListenerEventV2sWithEventWithEventListenerAsync(
                    inputTake,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedListenerEventV2s);

            // when
            IEnumerable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2ProcessingService
                    .RetrieveBatchOfRetryListenerEventV2sAsync(inputTake, randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveRetryBatchListenerEventV2sWithEventWithEventListenerAsync(
                    inputTake,
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
