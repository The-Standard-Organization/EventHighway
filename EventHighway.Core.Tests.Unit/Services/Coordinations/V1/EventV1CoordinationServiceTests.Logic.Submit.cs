// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V1
{
    public partial class EventV1CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldSubmitScheduleEventV1WhenScheduledDateIsInFutureAsync()
        {
            // given
            int randomDays = GetRandomNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset retrievedDateTimeOffset = randomDateTimeOffset;
            EventV1 randomEventV1 = CreateRandomEventV1();
            EventV1 inputEventV1 = randomEventV1;

            inputEventV1.ScheduledDate =
                retrievedDateTimeOffset.AddDays(randomDays);

            EventV1 inputScheduledEventV1 = inputEventV1;
            inputScheduledEventV1.Type = EventV1Type.Scheduled;
            EventV1 submittedEventV1 = inputScheduledEventV1;
            EventV1 expectedEventV1 = submittedEventV1.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(retrievedDateTimeOffset);

            this.eventV1OrchestrationServiceMock.Setup(service =>
                service.SubmitEventV1Async(inputScheduledEventV1))
                    .ReturnsAsync(submittedEventV1);

            // when
            EventV1 actualEventV1 =
                await this.eventV1CoordinationService
                    .SubmitEventV1Async(inputEventV1);

            // then
            actualEventV1.Should().BeEquivalentTo(expectedEventV1);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV1Async(inputScheduledEventV1),
                    Times.Once);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.AddListenerEventV1Async(
                    It.IsAny<ListenerEventV1>()),
                        Times.Never);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.RunEventCallV1Async(
                    It.IsAny<EventCallV1>()),
                        Times.Never);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.ModifyListenerEventV1Async(
                    It.IsAny<ListenerEventV1>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(PlainException))]
        [MemberData(nameof(EventV1DependencyExceptions))]
        [MemberData(nameof(EventListenerV1DependencyExceptions))]
        public async Task ShouldRetryAndReturnEventV1OnSubmitWhenTransientErrorOccursAsync(
            Exception exception)
        {
            // given
            int randomDays = GetRandomNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset retrievedDateTimeOffset = randomDateTimeOffset;
            int randomRetryAttempts = GetRandomNumber();
            int inputRetryAttempts = randomRetryAttempts;
            EventV1 randomEventV1 = CreateRandomEventV1(inputRetryAttempts);
            EventV1 inputEventV1 = randomEventV1;

            inputEventV1.ScheduledDate =
                retrievedDateTimeOffset.AddDays(randomDays);

            EventV1 inputScheduledEventV1 = inputEventV1;
            inputScheduledEventV1.Type = EventV1Type.Scheduled;
            EventV1 submittedEventV1 = inputScheduledEventV1;
            EventV1 expectedEventV1 = submittedEventV1.DeepClone();
            expectedEventV1.RetryAttempts = randomRetryAttempts - 1;

            this.dateTimeBrokerMock.SetupSequence(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(exception)
                    .ReturnsAsync(retrievedDateTimeOffset);

            this.eventV1OrchestrationServiceMock.Setup(service =>
                service.SubmitEventV1Async(inputScheduledEventV1))
                    .ReturnsAsync(submittedEventV1);

            // when
            EventV1 actualEventV1 =
                await this.eventV1CoordinationService
                    .SubmitEventV1Async(inputEventV1);

            // then
            actualEventV1.Should().BeEquivalentTo(expectedEventV1);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(callCount: 2));

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV1Async(inputScheduledEventV1),
                    Times.Once);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.AddListenerEventV1Async(
                    It.IsAny<ListenerEventV1>()),
                        Times.Never);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.RunEventCallV1Async(
                    It.IsAny<EventCallV1>()),
                        Times.Never);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.ModifyListenerEventV1Async(
                    It.IsAny<ListenerEventV1>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ScheduledDates))]
        public async Task ShouldSubmitImmediateEventV1WhenScheduledDateIsNullOrInPastAsync(
            DateTimeOffset randomDateTimeOffset,
            DateTimeOffset? scheduledDate)
        {
            // given
            var mockSequence = new MockSequence();
            EventV1 randomEventV1 = CreateRandomEventV1();
            EventV1 inputEventV1 = randomEventV1;
            inputEventV1.ScheduledDate = scheduledDate;
            EventV1 inputImmediateEventV1 = inputEventV1;
            inputImmediateEventV1.Type = EventV1Type.Immediate;
            EventV1 submittedEventV1 = inputImmediateEventV1;
            EventV1 expectedEventV1 = submittedEventV1.DeepClone();

            IQueryable<EventListenerV1> randomEventListenerV1s =
                CreateRandomEventListenerV1s();

            IQueryable<EventListenerV1> retrievedEventListenerV1s =
                randomEventListenerV1s;

            List<ListenerEventV1> inputListenerEventV1s =
                retrievedEventListenerV1s.Select(eventListenerV1 =>
                    new ListenerEventV1
                    {
                        EventListenerId = eventListenerV1.Id,
                        EventId = inputImmediateEventV1.Id,
                        Status = ListenerEventV1Status.Pending,
                        EventAddressId = inputImmediateEventV1.EventAddressId,
                        CreatedDate = randomDateTimeOffset,
                        UpdatedDate = randomDateTimeOffset
                    }).ToList();

            List<ListenerEventV1> addedListenerEventV1s =
                inputListenerEventV1s.DeepClone();

            List<ListenerEventV1> modifiedListenerEventV1s =
                addedListenerEventV1s;

            expectedEventV1.ListenerEvents = modifiedListenerEventV1s;

            List<ListenerEventV1> expectedListenerEventV1s =
                modifiedListenerEventV1s.DeepClone();

            List<EventCallV1> expectedInputCallEventV1s =
                retrievedEventListenerV1s.Select(
                    retrievedEventListenerV1 =>
                        new EventCallV1
                        {
                            Endpoint = retrievedEventListenerV1.Endpoint,
                            Content = inputImmediateEventV1.Content,
                            Secret = retrievedEventListenerV1.HeaderSecret,
                        }).ToList();

            int expectedDateTimeBrokerCalls =
                inputListenerEventV1s.Count +
                    modifiedListenerEventV1s.Count + 1;

            var ranEventCallV1s = new List<EventCallV1>();

            this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.eventV1OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.SubmitEventV1Async(inputImmediateEventV1))
                        .ReturnsAsync(submittedEventV1);

            this.eventListenerV1OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveEventListenerV1sByEventAddressIdAsync(
                        inputImmediateEventV1.EventAddressId))
                            .ReturnsAsync(retrievedEventListenerV1s);

            for (int index = 0; index < inputListenerEventV1s.Count; index++)
            {
                this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                    broker.GetDateTimeOffsetAsync())
                        .ReturnsAsync(randomDateTimeOffset);

                this.eventListenerV1OrchestrationServiceMock
                    .InSequence(mockSequence).Setup(service =>
                        service.AddListenerEventV1Async(
                            It.Is(SameListenerEventAs(inputListenerEventV1s[index]))))
                                .ReturnsAsync(addedListenerEventV1s[index]);

                var ranEventCall = new EventCallV1
                {
                    Endpoint = expectedInputCallEventV1s[index].Endpoint,
                    Content = expectedInputCallEventV1s[index].Content,
                    Response = GetRandomString()
                };

                this.eventV1OrchestrationServiceMock
                    .InSequence(mockSequence).Setup(service =>
                        service.RunEventCallV1Async(
                            It.Is(SameEventCallAs(expectedInputCallEventV1s[index]))))
                                .ReturnsAsync(ranEventCall);

                ranEventCallV1s.Add(item: ranEventCall);

                this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                    broker.GetDateTimeOffsetAsync())
                        .ReturnsAsync(randomDateTimeOffset);

                addedListenerEventV1s[index].UpdatedDate = randomDateTimeOffset;
                addedListenerEventV1s[index].Status = ListenerEventV1Status.Success;
                addedListenerEventV1s[index].Response = ranEventCallV1s[index].Response;

                this.eventListenerV1OrchestrationServiceMock
                    .InSequence(mockSequence).Setup(service =>
                        service.ModifyListenerEventV1Async(
                            It.Is(SameListenerEventAs(addedListenerEventV1s[index]))))
                                .ReturnsAsync(modifiedListenerEventV1s[index]);
            }

            // when
            EventV1 actualEventV1 =
                await this.eventV1CoordinationService
                    .SubmitEventV1Async(inputEventV1);

            // then
            actualEventV1.Should().BeEquivalentTo(expectedEventV1);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(callCount: expectedDateTimeBrokerCalls));

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV1Async(inputImmediateEventV1),
                    Times.Once);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(
                    inputImmediateEventV1.EventAddressId),
                        Times.Once);

            for (int index = 0; index < inputListenerEventV1s.Count; index++)
            {
                this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                    service.AddListenerEventV1Async(
                        It.Is(SameListenerEventAs(inputListenerEventV1s[index]))),
                            Times.Once);

                this.eventV1OrchestrationServiceMock.Verify(service =>
                    service.RunEventCallV1Async(
                        It.Is(SameEventCallAs(expectedInputCallEventV1s[index]))),
                            Times.Once);

                this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                    service.ModifyListenerEventV1Async(
                        It.Is(SameListenerEventAs(addedListenerEventV1s[index]))),
                            Times.Once);
            }

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
