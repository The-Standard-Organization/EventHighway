// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V2
{
    public partial class EventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldSubmitScheduleEventV2WhenScheduledDateIsInFutureAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomDays = GetRandomNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset retrievedDateTimeOffset = randomDateTimeOffset;
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;

            inputEventV2.ScheduledDate =
                retrievedDateTimeOffset.AddDays(randomDays);

            EventV2 inputScheduledEventV2 = inputEventV2;
            inputScheduledEventV2.Type = EventTypeV2.Scheduled;
            EventV2 submittedEventV2 = inputScheduledEventV2;
            EventV2 expectedEventV2 = submittedEventV2.DeepClone();
            expectedEventV2.EventParticipantV2Secret = null;

            SetupValidateEventParticipantsSucceeds();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(retrievedDateTimeOffset);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.StampContentHashAsync(
                    inputScheduledEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(inputScheduledEventV2);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.IsLoopDetectedAsync(
                    inputScheduledEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(false);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.SubmitEventV2Async(
                    inputScheduledEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(submittedEventV2);

            // when
            EventV2 actualEventV2 =
                await this.eventV2CoordinationService
                    .SubmitEventV2Async(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualEventV2.Should().BeEquivalentTo(expectedEventV2);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.StampContentHashAsync(
                    inputScheduledEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.IsLoopDetectedAsync(
                    inputScheduledEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV2Async(
                    inputScheduledEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            VerifyValidateEventParticipantsCalledOnce();

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldClearEventParticipantSecretOnSubmitEventV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomDays = GetRandomNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset retrievedDateTimeOffset = randomDateTimeOffset;
            string randomSecret = GetRandomString();
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = GetRandomId();
            inputEventV2.EventParticipantV2Secret = randomSecret;

            inputEventV2.ScheduledDate =
                retrievedDateTimeOffset.AddDays(randomDays);

            EventV2 inputScheduledEventV2 = inputEventV2;
            inputScheduledEventV2.Type = EventTypeV2.Scheduled;
            EventV2 submittedEventV2 = inputScheduledEventV2;
            string validatedSecretValue = null;
            string stampedSecretValue = null;

            this.eventParticipantV2OrchestrationServiceMock.Setup(service =>
                service.ValidateEventParticipantsAsync(
                    inputEventV2,
                    randomCancellationToken))
                        .Callback<EventV2, CancellationToken>((eventV2, _) =>
                            validatedSecretValue = eventV2.EventParticipantV2Secret)
                        .Returns(ValueTask.CompletedTask);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(retrievedDateTimeOffset);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.StampContentHashAsync(
                    inputScheduledEventV2,
                    randomCancellationToken))
                        .Callback<EventV2, CancellationToken>((eventV2, _) =>
                            stampedSecretValue = eventV2.EventParticipantV2Secret)
                        .ReturnsAsync(inputScheduledEventV2);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.IsLoopDetectedAsync(
                    inputScheduledEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(false);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.SubmitEventV2Async(
                    inputScheduledEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(submittedEventV2);

            // when
            EventV2 actualEventV2 =
                await this.eventV2CoordinationService
                    .SubmitEventV2Async(
                        inputEventV2,
                        randomCancellationToken);

            // then
            validatedSecretValue.Should().Be(randomSecret);
            stampedSecretValue.Should().BeNull();
            actualEventV2.EventParticipantV2Secret.Should().BeNull();

            this.eventParticipantV2OrchestrationServiceMock.Verify(service =>
                service.ValidateEventParticipantsAsync(
                    inputEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.StampContentHashAsync(
                    inputScheduledEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.IsLoopDetectedAsync(
                    inputScheduledEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV2Async(
                    inputScheduledEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ScheduledDates))]
        public async Task ShouldSubmitImmediateEventV2WhenScheduledDateIsNullOrInPastAsync(
            DateTimeOffset randomDateTimeOffset,
            DateTimeOffset? scheduledDate)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var mockSequence = new MockSequence();

            SetupValidateEventParticipantsInSequence(mockSequence);
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.ScheduledDate = scheduledDate;
            EventV2 inputImmediateEventV2 = inputEventV2;
            inputImmediateEventV2.Type = EventTypeV2.Immediate;
            EventV2 submittedEventV2 = inputImmediateEventV2;
            EventV2 firedEventV2 = CreateRandomEventV2();
            EventV2 expectedEventV2 = firedEventV2.DeepClone();

            this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.eventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.StampContentHashAsync(
                        inputImmediateEventV2,
                        randomCancellationToken))
                            .ReturnsAsync(inputImmediateEventV2);

            this.eventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.IsLoopDetectedAsync(
                        inputImmediateEventV2,
                        randomCancellationToken))
                            .ReturnsAsync(false);

            this.eventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.SubmitEventV2Async(
                        inputImmediateEventV2,
                        randomCancellationToken))
                            .ReturnsAsync(submittedEventV2);

            this.eventFiringV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.FireEventV2Async(
                        submittedEventV2,
                        randomCancellationToken))
                            .ReturnsAsync(firedEventV2);

            // when
            EventV2 actualEventV2 =
                await this.eventV2CoordinationService
                    .SubmitEventV2Async(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualEventV2.Should().BeEquivalentTo(expectedEventV2);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.StampContentHashAsync(
                    inputImmediateEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.IsLoopDetectedAsync(
                    inputImmediateEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV2Async(
                    inputImmediateEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(
                    submittedEventV2,
                    randomCancellationToken),
                        Times.Once);

            VerifyValidateEventParticipantsCalledOnce();

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
