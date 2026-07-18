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
        public async Task ShouldAddEventParticipantSecretV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            EventParticipantSecretV2 randomEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2(randomDateTimeOffset);

            EventParticipantSecretV2 inputEventParticipantSecretV2 = randomEventParticipantSecretV2;
            string inputSecretValue = inputEventParticipantSecretV2.Secret;
            string hashedSecretValue = GetRandomString();
            EventParticipantSecretV2 insertedEventParticipantSecretV2 = inputEventParticipantSecretV2;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.hashBrokerMock.Setup(broker =>
                broker.GenerateSha256Hash(inputSecretValue))
                    .Returns(hashedSecretValue);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertEventParticipantSecretV2Async(
                    inputEventParticipantSecretV2, randomCancellationToken))
                        .ReturnsAsync(insertedEventParticipantSecretV2);

            // when
            EventParticipantSecretV2 actualEventParticipantSecretV2 =
                await this.eventParticipantSecretV2Service
                    .AddEventParticipantSecretV2Async(
                        inputEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2 expectedEventParticipantSecretV2 =
                insertedEventParticipantSecretV2.DeepClone();

            // then
            actualEventParticipantSecretV2.Secret.Should().Be(hashedSecretValue);

            actualEventParticipantSecretV2.Should().BeEquivalentTo(
                expectedEventParticipantSecretV2);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.hashBrokerMock.Verify(broker =>
                broker.GenerateSha256Hash(inputSecretValue),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventParticipantSecretV2Async(
                    inputEventParticipantSecretV2, randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
