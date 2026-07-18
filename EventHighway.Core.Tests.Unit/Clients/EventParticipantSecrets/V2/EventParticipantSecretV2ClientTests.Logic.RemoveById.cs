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

namespace EventHighway.Core.Tests.Unit.Clients.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ClientTests
    {
        [Fact]
        public async Task ShouldRemoveEventParticipantSecretV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventParticipantSecretV2Id = GetRandomId();
            Guid inputEventParticipantSecretV2Id = randomEventParticipantSecretV2Id;

            EventParticipantSecretV2 randomEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2();

            EventParticipantSecretV2 removedEventParticipantSecretV2 =
                randomEventParticipantSecretV2;

            EventParticipantSecretV2 expectedEventParticipantSecretV2 =
                removedEventParticipantSecretV2.DeepClone();

            expectedEventParticipantSecretV2.Secret = null;

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RemoveEventParticipantSecretV2ByIdAsync(
                    inputEventParticipantSecretV2Id,
                    randomCancellationToken))
                        .ReturnsAsync(removedEventParticipantSecretV2);

            // when
            EventParticipantSecretV2 actualEventParticipantSecretV2 =
                await this.eventParticipantSecretV2Client
                    .RemoveEventParticipantSecretV2ByIdAsync(
                        inputEventParticipantSecretV2Id,
                        randomCancellationToken);

            // then
            actualEventParticipantSecretV2.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RemoveEventParticipantSecretV2ByIdAsync(
                    inputEventParticipantSecretV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
