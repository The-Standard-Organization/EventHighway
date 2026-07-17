// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventParticipants.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventParticipants.V2
{
    public partial class EventParticipantV2OrchestrationServiceTests
    {
        private async Task RunSecretValidationFailureScenarioAsync(
            EventParticipantSecretV2 storedSecret,
            string inputSecret,
            string hashedSecretValue,
            string expectedMessage)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventParticipantV2 randomEventParticipantV2 = CreateRandomEventParticipantV2();
            EventParticipantV2 activeEventParticipantV2 = randomEventParticipantV2;
            activeEventParticipantV2.IsActive = true;
            activeEventParticipantV2.ActiveFrom = null;
            activeEventParticipantV2.ActiveTo = null;

            storedSecret.EventParticipantV2Id = activeEventParticipantV2.Id;

            IQueryable<EventParticipantSecretV2> eventParticipantSecretV2s =
                new List<EventParticipantSecretV2> { storedSecret }.AsQueryable();

            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = activeEventParticipantV2.Id;
            inputEventV2.EventParticipantV2Secret = inputSecret;

            var invalidEventParticipantV2OrchestrationException =
                new InvalidEventParticipantV2OrchestrationException(
                    message: expectedMessage);

            var expectedEventParticipantV2OrchestrationValidationException =
                new EventParticipantV2OrchestrationValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2OrchestrationException);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventV2.EventParticipantV2Id.Value,
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(activeEventParticipantV2);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.hashBrokerMock.Setup(broker =>
                broker.GenerateSha256Hash(inputSecret))
                    .Returns(hashedSecretValue);

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RetrieveAllEventParticipantSecretV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(eventParticipantSecretV2s);

            // when
            ValueTask validateTask =
                this.eventParticipantV2OrchestrationService
                    .ValidateEventParticipantsAsync(
                        inputEventV2,
                        TestContext.Current.CancellationToken);

            EventParticipantV2OrchestrationValidationException
                actualEventParticipantV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventParticipantV2OrchestrationValidationException>(
                        validateTask.AsTask);

            // then
            actualEventParticipantV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2OrchestrationValidationException);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventV2.EventParticipantV2Id.Value,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.hashBrokerMock.Verify(broker =>
                broker.GenerateSha256Hash(inputSecret),
                    Times.Once);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantSecretV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2OrchestrationValidationException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnValidateIfSecretIsNotFoundAndLogItAsync()
        {
            // given
            EventParticipantSecretV2 nonMatchingSecret = CreateRandomEventParticipantSecretV2();
            nonMatchingSecret.EventParticipantV2Id = GetRandomId();
            nonMatchingSecret.Secret = GetRandomString();
            string inputSecret = GetRandomString();
            string hashedSecretValue = GetRandomString();

            // when then
            await RunSecretValidationFailureScenarioAsync(
                storedSecret: nonMatchingSecret,
                inputSecret: inputSecret,
                hashedSecretValue: hashedSecretValue,
                expectedMessage: "Event participant secret not found.");
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnValidateIfSecretIsInactiveAndLogItAsync()
        {
            // given
            string inputSecret = GetRandomString();
            string hashedSecretValue = GetRandomString();
            EventParticipantSecretV2 inactiveSecret = CreateRandomEventParticipantSecretV2();
            inactiveSecret.Secret = hashedSecretValue;
            inactiveSecret.IsActive = false;
            inactiveSecret.ActiveFrom = null;
            inactiveSecret.ActiveTo = null;

            // when then
            await RunSecretValidationFailureScenarioAsync(
                storedSecret: inactiveSecret,
                inputSecret: inputSecret,
                hashedSecretValue: hashedSecretValue,
                expectedMessage: "Event participant secret is not active.");
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnValidateIfSecretActiveFromIsInFutureAndLogItAsync()
        {
            // given
            string inputSecret = GetRandomString();
            string hashedSecretValue = GetRandomString();
            EventParticipantSecretV2 notYetActiveSecret = CreateRandomEventParticipantSecretV2();
            notYetActiveSecret.Secret = hashedSecretValue;
            notYetActiveSecret.IsActive = true;
            notYetActiveSecret.ActiveFrom = DateTimeOffset.MaxValue;
            notYetActiveSecret.ActiveTo = null;

            // when then
            await RunSecretValidationFailureScenarioAsync(
                storedSecret: notYetActiveSecret,
                inputSecret: inputSecret,
                hashedSecretValue: hashedSecretValue,
                expectedMessage: "Event participant secret is outside its active window.");
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnValidateIfSecretActiveToHasExpiredAndLogItAsync()
        {
            // given
            string inputSecret = GetRandomString();
            string hashedSecretValue = GetRandomString();
            EventParticipantSecretV2 expiredSecret = CreateRandomEventParticipantSecretV2();
            expiredSecret.Secret = hashedSecretValue;
            expiredSecret.IsActive = true;
            expiredSecret.ActiveFrom = null;
            expiredSecret.ActiveTo = DateTimeOffset.MinValue;

            // when then
            await RunSecretValidationFailureScenarioAsync(
                storedSecret: expiredSecret,
                inputSecret: inputSecret,
                hashedSecretValue: hashedSecretValue,
                expectedMessage: "Event participant secret is outside its active window.");
        }
    }
}
