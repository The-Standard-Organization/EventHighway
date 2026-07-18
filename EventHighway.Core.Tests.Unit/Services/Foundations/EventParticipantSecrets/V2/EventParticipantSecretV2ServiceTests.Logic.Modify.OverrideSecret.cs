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

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ServiceTests
    {
        [Fact]
        public async Task ShouldOverrideRedactedSecretFromStorageOnModifyAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            int randomDaysAgo = GetRandomNegativeNumber();

            EventParticipantSecretV2 randomEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2(randomDateTime);

            EventParticipantSecretV2 inputEventParticipantSecretV2 =
                randomEventParticipantSecretV2;

            inputEventParticipantSecretV2.CreatedDate =
                randomDateTime.AddDays(randomDaysAgo);

            string storedSecret = GetRandomString();

            EventParticipantSecretV2 storageEventParticipantSecretV2 =
                inputEventParticipantSecretV2.DeepClone();

            storageEventParticipantSecretV2.Secret = storedSecret;

            int randomSecondsAgo = GetRandomNegativeNumber();

            storageEventParticipantSecretV2.UpdatedDate =
                randomDateTime.AddSeconds(randomSecondsAgo);

            // the client redacts Secret on reads, so the caller submits a null Secret
            inputEventParticipantSecretV2.Secret = null;

            EventParticipantSecretV2 persistedEventParticipantSecretV2 =
                inputEventParticipantSecretV2.DeepClone();

            persistedEventParticipantSecretV2.Secret = storedSecret;

            EventParticipantSecretV2 expectedEventParticipantSecretV2 =
                persistedEventParticipantSecretV2.DeepClone();

            Guid eventParticipantSecretV2Id = inputEventParticipantSecretV2.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    eventParticipantSecretV2Id, randomCancellationToken))
                        .ReturnsAsync(storageEventParticipantSecretV2);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateEventParticipantSecretV2Async(
                    It.Is<EventParticipantSecretV2>(secret => secret.Secret == storedSecret),
                    randomCancellationToken))
                        .ReturnsAsync(persistedEventParticipantSecretV2);

            // when
            EventParticipantSecretV2 actualEventParticipantSecretV2 =
                await this.eventParticipantSecretV2Service
                    .ModifyEventParticipantSecretV2Async(
                        inputEventParticipantSecretV2, randomCancellationToken);

            // then
            actualEventParticipantSecretV2.Should().BeEquivalentTo(
                expectedEventParticipantSecretV2);

            actualEventParticipantSecretV2.Secret.Should().Be(storedSecret);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    eventParticipantSecretV2Id, randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventParticipantSecretV2Async(
                    It.Is<EventParticipantSecretV2>(secret => secret.Secret == storedSecret),
                    randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
