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
        public async Task ShouldRetrieveEventParticipantSecretV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventParticipantSecretV2Id = GetRandomId();
            Guid inputEventParticipantSecretV2Id = randomEventParticipantSecretV2Id;

            EventParticipantSecretV2 randomEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2();

            EventParticipantSecretV2 retrievedEventParticipantSecretV2 =
                randomEventParticipantSecretV2;

            EventParticipantSecretV2 expectedEventParticipantSecretV2 =
                retrievedEventParticipantSecretV2.DeepClone();

            expectedEventParticipantSecretV2.Secret = null;

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantSecretV2ByIdAsync(
                    inputEventParticipantSecretV2Id,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventParticipantSecretV2);

            // when
            EventParticipantSecretV2 actualEventParticipantSecretV2 =
                await this.eventParticipantSecretV2Client
                    .RetrieveEventParticipantSecretV2ByIdAsync(
                        inputEventParticipantSecretV2Id,
                        randomCancellationToken);

            // then
            actualEventParticipantSecretV2.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantSecretV2ByIdAsync(
                    inputEventParticipantSecretV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
