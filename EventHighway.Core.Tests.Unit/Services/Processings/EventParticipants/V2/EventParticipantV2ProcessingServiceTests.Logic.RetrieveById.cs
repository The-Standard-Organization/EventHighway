// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldRetrieveEventParticipantV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventParticipantV2Id = GetRandomId();
            Guid inputEventParticipantV2Id = randomEventParticipantV2Id;

            EventParticipantV2 randomEventParticipantV2 =
                CreateRandomEventParticipantV2();

            EventParticipantV2 retrievedEventParticipantV2 =
                randomEventParticipantV2;

            EventParticipantV2 expectedEventParticipantV2 =
                retrievedEventParticipantV2.DeepClone();

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventParticipantV2Id,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventParticipantV2);

            // when
            EventParticipantV2 actualEventParticipantV2 =
                await this.eventParticipantV2ProcessingService
                    .RetrieveEventParticipantV2ByIdAsync(
                        inputEventParticipantV2Id,
                        randomCancellationToken);

            // then
            actualEventParticipantV2.Should()
                .BeEquivalentTo(expectedEventParticipantV2);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventParticipantV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
