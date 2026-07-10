// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventParticipants.V2
{
    public partial class EventParticipantV2ClientTests
    {
        [Fact]
        public async Task ShouldAddEventParticipantV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 randomEventParticipantV2 = CreateRandomEventParticipantV2();
            EventParticipantV2 inputEventParticipantV2 = randomEventParticipantV2;
            EventParticipantV2 addedEventParticipantV2 = inputEventParticipantV2;
            EventParticipantV2 expectedEventParticipantV2 = addedEventParticipantV2.DeepClone();

            this.eventParticipantV2ProcessingServiceMock.Setup(service =>
                service.AddEventParticipantV2Async(
                    inputEventParticipantV2,
                    randomCancellationToken))
                        .ReturnsAsync(addedEventParticipantV2);

            // when
            EventParticipantV2 actualEventParticipantV2 =
                await this.eventParticipantV2Client
                    .AddEventParticipantV2Async(
                        inputEventParticipantV2,
                        randomCancellationToken);

            // then
            actualEventParticipantV2.Should()
                .BeEquivalentTo(expectedEventParticipantV2);

            this.eventParticipantV2ProcessingServiceMock.Verify(service =>
                service.AddEventParticipantV2Async(
                    inputEventParticipantV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
