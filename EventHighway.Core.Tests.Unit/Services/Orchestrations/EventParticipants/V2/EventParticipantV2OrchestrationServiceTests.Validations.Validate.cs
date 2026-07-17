// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        [Fact]
        public async Task ShouldThrowValidationExceptionOnValidateIfParticipantIdIsNotProvidedAndLogItAsync()
        {
            // given
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = Guid.Empty;
            inputEventV2.EventParticipantV2Secret = null;

            var invalidEventParticipantV2OrchestrationException =
                new InvalidEventParticipantV2OrchestrationException(
                    message: "Event participant id is required.");

            var expectedEventParticipantV2OrchestrationValidationException =
                new EventParticipantV2OrchestrationValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2OrchestrationException);

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

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2OrchestrationValidationException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnValidateIfParticipantIsNotFoundAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventParticipantV2 nullEventParticipantV2 = null;

            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = GetRandomId();
            inputEventV2.EventParticipantV2Secret = null;

            var invalidEventParticipantV2OrchestrationException =
                new InvalidEventParticipantV2OrchestrationException(
                    message: "Event participant not found.");

            var expectedEventParticipantV2OrchestrationValidationException =
                new EventParticipantV2OrchestrationValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2OrchestrationException);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventV2.EventParticipantV2Id,
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(nullEventParticipantV2);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

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
                    inputEventV2.EventParticipantV2Id,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2OrchestrationValidationException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnValidateIfParticipantIsInactiveAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventParticipantV2 randomEventParticipantV2 = CreateRandomEventParticipantV2();
            EventParticipantV2 inactiveEventParticipantV2 = randomEventParticipantV2;
            inactiveEventParticipantV2.IsActive = false;
            inactiveEventParticipantV2.ActiveFrom = null;
            inactiveEventParticipantV2.ActiveTo = null;

            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = inactiveEventParticipantV2.Id;
            inputEventV2.EventParticipantV2Secret = null;

            var invalidEventParticipantV2OrchestrationException =
                new InvalidEventParticipantV2OrchestrationException(
                    message: "Event participant is not active.");

            var expectedEventParticipantV2OrchestrationValidationException =
                new EventParticipantV2OrchestrationValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2OrchestrationException);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventV2.EventParticipantV2Id,
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(inactiveEventParticipantV2);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

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
                    inputEventV2.EventParticipantV2Id,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2OrchestrationValidationException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnValidateIfParticipantActiveFromIsInFutureAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventParticipantV2 randomEventParticipantV2 = CreateRandomEventParticipantV2();
            EventParticipantV2 notYetActiveEventParticipantV2 = randomEventParticipantV2;
            notYetActiveEventParticipantV2.IsActive = true;
            notYetActiveEventParticipantV2.ActiveFrom = randomDateTimeOffset.AddMinutes(1);
            notYetActiveEventParticipantV2.ActiveTo = null;

            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = notYetActiveEventParticipantV2.Id;
            inputEventV2.EventParticipantV2Secret = null;

            var invalidEventParticipantV2OrchestrationException =
                new InvalidEventParticipantV2OrchestrationException(
                    message: "Event participant is outside its active window.");

            var expectedEventParticipantV2OrchestrationValidationException =
                new EventParticipantV2OrchestrationValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2OrchestrationException);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventV2.EventParticipantV2Id,
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(notYetActiveEventParticipantV2);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

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
                    inputEventV2.EventParticipantV2Id,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2OrchestrationValidationException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnValidateIfParticipantActiveToHasExpiredAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventParticipantV2 randomEventParticipantV2 = CreateRandomEventParticipantV2();
            EventParticipantV2 expiredEventParticipantV2 = randomEventParticipantV2;
            expiredEventParticipantV2.IsActive = true;
            expiredEventParticipantV2.ActiveFrom = null;
            expiredEventParticipantV2.ActiveTo = randomDateTimeOffset.AddMinutes(-1);

            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = expiredEventParticipantV2.Id;
            inputEventV2.EventParticipantV2Secret = null;

            var invalidEventParticipantV2OrchestrationException =
                new InvalidEventParticipantV2OrchestrationException(
                    message: "Event participant is outside its active window.");

            var expectedEventParticipantV2OrchestrationValidationException =
                new EventParticipantV2OrchestrationValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2OrchestrationException);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventV2.EventParticipantV2Id,
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expiredEventParticipantV2);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

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
                    inputEventV2.EventParticipantV2Id,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2OrchestrationValidationException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnValidateIfSecretIsRequiredButNotProvidedAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventParticipantV2 randomEventParticipantV2 = CreateRandomEventParticipantV2();
            EventParticipantV2 activeEventParticipantV2 = randomEventParticipantV2;
            activeEventParticipantV2.IsActive = true;
            activeEventParticipantV2.ActiveFrom = null;
            activeEventParticipantV2.ActiveTo = null;
            activeEventParticipantV2.IsSecretRequired = true;

            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = activeEventParticipantV2.Id;
            inputEventV2.EventParticipantV2Secret = null;

            var invalidEventParticipantV2OrchestrationException =
                new InvalidEventParticipantV2OrchestrationException(
                    message: "Event participant secret is required.");

            var expectedEventParticipantV2OrchestrationValidationException =
                new EventParticipantV2OrchestrationValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2OrchestrationException);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventV2.EventParticipantV2Id,
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(activeEventParticipantV2);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

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
                    inputEventV2.EventParticipantV2Id,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2OrchestrationValidationException))),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantSecretV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
