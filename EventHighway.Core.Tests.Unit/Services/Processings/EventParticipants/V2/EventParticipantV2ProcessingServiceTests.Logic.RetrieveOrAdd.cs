// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventParticipants.V2
{
    public partial class EventParticipantV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEventParticipantV2IfItAlreadyExistsAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 randomEventParticipantV2 =
                CreateRandomEventParticipantV2();

            EventParticipantV2 inputEventParticipantV2 =
                randomEventParticipantV2;

            IQueryable<EventParticipantV2> retrievedEventParticipantV2s =
                new[] { inputEventParticipantV2 }.AsQueryable();

            EventParticipantV2 expectedEventParticipantV2 =
                inputEventParticipantV2.DeepClone();

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventParticipantV2s);

            // when
            EventParticipantV2 actualEventParticipantV2 =
                await this.eventParticipantV2ProcessingService
                    .RetrieveOrAddEventParticipantV2Async(
                        inputEventParticipantV2,
                        randomCancellationToken);

            // then
            actualEventParticipantV2.Should().BeEquivalentTo(
                expectedEventParticipantV2);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldAddEventParticipantV2IfItDoesNotExistAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 randomEventParticipantV2 =
                CreateRandomEventParticipantV2();

            EventParticipantV2 inputEventParticipantV2 =
                randomEventParticipantV2;

            EventParticipantV2 addedEventParticipantV2 =
                inputEventParticipantV2;

            EventParticipantV2 expectedEventParticipantV2 =
                addedEventParticipantV2.DeepClone();

            IQueryable<EventParticipantV2> emptyEventParticipantV2s =
                Enumerable.Empty<EventParticipantV2>().AsQueryable();

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken))
                    .ReturnsAsync(emptyEventParticipantV2s);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.AddEventParticipantV2Async(
                    inputEventParticipantV2,
                    randomCancellationToken))
                        .ReturnsAsync(addedEventParticipantV2);

            // when
            EventParticipantV2 actualEventParticipantV2 =
                await this.eventParticipantV2ProcessingService
                    .RetrieveOrAddEventParticipantV2Async(
                        inputEventParticipantV2,
                        randomCancellationToken);

            // then
            actualEventParticipantV2.Should().BeEquivalentTo(
                expectedEventParticipantV2);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.AddEventParticipantV2Async(
                    inputEventParticipantV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
