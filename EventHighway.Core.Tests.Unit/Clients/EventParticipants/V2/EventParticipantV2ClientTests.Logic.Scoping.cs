// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventParticipants.V2
{
    public partial class EventParticipantV2ClientTests
    {
        [Fact]
        public async Task ShouldResolveProcessingServiceInNewScopePerOperationAsync()
        {
            // given
            EventParticipantV2 randomEventParticipantV2 = CreateRandomEventParticipantV2();
            Guid inputEventParticipantV2Id = randomEventParticipantV2.Id;
            int expectedResolutionCount = 2;

            this.eventParticipantV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventParticipantV2Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(randomEventParticipantV2);

            // when
            await this.eventParticipantV2Client.RetrieveEventParticipantV2ByIdAsync(
                inputEventParticipantV2Id);

            await this.eventParticipantV2Client.RetrieveEventParticipantV2ByIdAsync(
                inputEventParticipantV2Id);

            // then
            this.processingServiceResolutionCount.Should()
                .Be(expectedResolutionCount);

            this.eventParticipantV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventParticipantV2Id, It.IsAny<CancellationToken>()),
                        Times.Exactly(2));

            this.eventParticipantV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
