// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ClientTests
    {
        [Fact]
        public async Task ShouldModifyEventParticipantSecretV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantSecretV2 randomEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2();

            EventParticipantSecretV2 inputEventParticipantSecretV2 =
                randomEventParticipantSecretV2;

            EventParticipantSecretV2 modifiedEventParticipantSecretV2 =
                inputEventParticipantSecretV2;

            EventParticipantSecretV2 expectedEventParticipantSecretV2 =
                modifiedEventParticipantSecretV2.DeepClone();

            expectedEventParticipantSecretV2.Secret = null;

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.ModifyEventParticipantSecretV2Async(
                    inputEventParticipantSecretV2,
                    randomCancellationToken))
                        .ReturnsAsync(modifiedEventParticipantSecretV2);

            // when
            EventParticipantSecretV2 actualEventParticipantSecretV2 =
                await this.eventParticipantSecretV2Client
                    .ModifyEventParticipantSecretV2Async(
                        inputEventParticipantSecretV2,
                        randomCancellationToken);

            // then
            actualEventParticipantSecretV2.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.ModifyEventParticipantSecretV2Async(
                    inputEventParticipantSecretV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
