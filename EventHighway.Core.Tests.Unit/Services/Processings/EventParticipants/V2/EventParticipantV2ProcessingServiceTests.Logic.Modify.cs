// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldModifyEventParticipantV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 randomEventParticipantV2 =
                CreateRandomEventParticipantV2();

            EventParticipantV2 inputEventParticipantV2 =
                randomEventParticipantV2;

            EventParticipantV2 modifiedEventParticipantV2 =
                inputEventParticipantV2;

            EventParticipantV2 expectedEventParticipantV2 =
                modifiedEventParticipantV2.DeepClone();

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.ModifyEventParticipantV2Async(
                    inputEventParticipantV2,
                    randomCancellationToken))
                        .ReturnsAsync(modifiedEventParticipantV2);

            // when
            EventParticipantV2 actualEventParticipantV2 =
                await this.eventParticipantV2ProcessingService
                    .ModifyEventParticipantV2Async(
                        inputEventParticipantV2,
                        randomCancellationToken);

            // then
            actualEventParticipantV2.Should().BeEquivalentTo(
                expectedEventParticipantV2);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.ModifyEventParticipantV2Async(
                    inputEventParticipantV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
