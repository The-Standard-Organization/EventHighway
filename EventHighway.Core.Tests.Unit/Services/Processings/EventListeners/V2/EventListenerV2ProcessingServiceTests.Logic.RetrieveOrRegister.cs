// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventListeners.V2
{
    public partial class EventListenerV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEventListenerV2IfItAlreadyExistsAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventListenerV2 randomEventListenerV2 =
                CreateRandomEventListenerV2();

            EventListenerV2 inputEventListenerV2 =
                randomEventListenerV2;

            IQueryable<EventListenerV2> retrievedEventListenerV2s =
                new[] { inputEventListenerV2 }.AsQueryable();

            EventListenerV2 expectedEventListenerV2 =
                inputEventListenerV2.DeepClone();

            this.eventListenerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync())
                    .ReturnsAsync(retrievedEventListenerV2s);

            // when
            EventListenerV2 actualEventListenerV2 =
                await this.eventListenerV2ProcessingService
                    .RetrieveOrRegisterEventListenerV2Async(
                        inputEventListenerV2,
                        randomCancellationToken);

            // then
            actualEventListenerV2.Should().BeEquivalentTo(
                expectedEventListenerV2);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(),
                    Times.Once);

            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRegisterEventListenerV2IfItDoesNotExistAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventListenerV2 randomEventListenerV2 =
                CreateRandomEventListenerV2();

            EventListenerV2 inputEventListenerV2 =
                randomEventListenerV2;

            EventListenerV2 registeredEventListenerV2 =
                inputEventListenerV2;

            EventListenerV2 expectedEventListenerV2 =
                registeredEventListenerV2.DeepClone();

            IQueryable<EventListenerV2> emptyEventListenerV2s =
                Enumerable.Empty<EventListenerV2>().AsQueryable();

            this.eventListenerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync())
                    .ReturnsAsync(emptyEventListenerV2s);

            this.eventListenerV2ServiceMock.Setup(service =>
                service.AddEventListenerV2Async(
                    inputEventListenerV2,
                    randomCancellationToken))
                        .ReturnsAsync(registeredEventListenerV2);

            // when
            EventListenerV2 actualEventListenerV2 =
                await this.eventListenerV2ProcessingService
                    .RetrieveOrRegisterEventListenerV2Async(
                        inputEventListenerV2,
                        randomCancellationToken);

            // then
            actualEventListenerV2.Should().BeEquivalentTo(
                expectedEventListenerV2);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(),
                    Times.Once);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.AddEventListenerV2Async(
                    inputEventListenerV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
