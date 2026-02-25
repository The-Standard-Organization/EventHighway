// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall;
using EventHighway.Core.Models.Services.Foundations.EventListeners;
using EventHighway.Core.Models.Services.Foundations.Events;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners
{
    public partial class EventListenerOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldSubmitEventToListenersAsync()
        {
            // given
            Event randomEvent = CreateRandomEvent();
            Event inputEvent = randomEvent;
            Event expectedEvent = inputEvent.DeepClone();
            Guid eventAddressId = inputEvent.EventAddressId;

            IQueryable<EventListener> randomEventListeners =
                CreateRandomEventListeners();

            IQueryable<EventListener> retrievedEventListeners =
                randomEventListeners;

            DateTimeOffset randomDate = GetRandomDateTime();
            DateTimeOffset updatedDate = randomDate;

            List<ListenerEvent> expectedListenerEvents =
                retrievedEventListeners.Select(retrievedEventListener =>
                    new ListenerEvent
                    {
                        EventListenerId = retrievedEventListener.Id,
                        EventId = inputEvent.Id,
                        Status = ListenerEventStatus.Pending,
                        EventAddressId = eventAddressId,
                        CreatedDate = inputEvent.CreatedDate,
                        UpdatedDate = inputEvent.UpdatedDate
                    }).ToList();

            List<ListenerEvent> expectedListenerEventOnModify =
                expectedListenerEvents.DeepClone();

            List<EventCall> expectedCallEvents =
                retrievedEventListeners.Select(retrievedEventListener =>
                    new EventCall
                    {
                        Endpoint = retrievedEventListener.Endpoint,
                        Content = inputEvent.Content,
                        Secret = retrievedEventListener.HeaderSecret,
                        Response = null
                    }).ToList();

            var ranEventCalls = new List<EventCall>();

            this.eventListenerProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenersByAddressIdAsync(eventAddressId))
                    .ReturnsAsync(retrievedEventListeners);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(updatedDate);

            foreach (ListenerEvent expectedListenerEvent in expectedListenerEvents)
            {
                this.listenerEventProcessingServiceMock.Setup(service =>
                    service.AddListenerEventAsync(
                        It.Is(SameListenerEventAs(expectedListenerEvent))))
                            .ReturnsAsync(expectedListenerEvent.DeepClone());
            }

            foreach (EventCall expectedCallEvent in expectedCallEvents)
            {
                var ranEventCall = new EventCall
                {
                    Endpoint = expectedCallEvent.Endpoint,
                    Content = expectedCallEvent.Content,
                    Response = CreateRandomEventCallResponse()
                };

                this.eventCallProcessingServiceMock.Setup(service =>
                    service.RunAsync(
                        It.Is(SameEventCallAs(expectedCallEvent))))
                            .ReturnsAsync(ranEventCall);

                ranEventCalls.Add(ranEventCall);
            }

            for (int i = 0; i < retrievedEventListeners.Count(); i++)
            {
                expectedListenerEventOnModify[i].UpdatedDate = updatedDate;
                expectedListenerEventOnModify[i].Status = ListenerEventStatus.Completed;
                expectedListenerEventOnModify[i].Response = ranEventCalls[i].Response;

                this.listenerEventProcessingServiceMock.Setup(service =>
                    service.ModifyListenerEventAsync(
                        It.Is(SameListenerEventAs(expectedListenerEventOnModify[i]))))
                            .ReturnsAsync(expectedListenerEventOnModify[i]);
            }

            // when
            Event actualEvent =
                await this.eventListenerOrchestrationService
                    .SubmitEventToListenersAsync(inputEvent);

            // then
            actualEvent.Should().BeEquivalentTo(expectedEvent);

            this.eventListenerProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenersByAddressIdAsync(eventAddressId),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(retrievedEventListeners.Count()));

            foreach (ListenerEvent expectedListenerEvent in expectedListenerEvents)
            {
                this.listenerEventProcessingServiceMock.Verify(service =>
                    service.AddListenerEventAsync(
                        It.Is(SameListenerEventAs(expectedListenerEvent))),
                            Times.Once);
            }

            foreach (EventCall expectedCallEvent in expectedCallEvents)
            {
                this.eventCallProcessingServiceMock.Verify(service =>
                    service.RunAsync(
                        It.Is(SameEventCallAs(expectedCallEvent))),
                            Times.Once);
            }

            foreach (ListenerEvent expectedListenerEvent in expectedListenerEventOnModify)
            {
                this.listenerEventProcessingServiceMock.Verify(service =>
                    service.ModifyListenerEventAsync(
                        It.Is(SameListenerEventAs(expectedListenerEvent))),
                            Times.Once);
            }

            this.eventListenerProcessingServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.listenerEventProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRecordFailuresOnSubmittalAsync()
        {
            // given
            Event randomEvent = CreateRandomEvent();
            Event inputEvent = randomEvent;
            Event expectedEvent = inputEvent.DeepClone();
            Guid eventAddressId = inputEvent.EventAddressId;

            IQueryable<EventListener> randomEventListeners =
                CreateRandomEventListeners();

            IQueryable<EventListener> retrievedEventListeners =
                randomEventListeners;

            DateTimeOffset randomDate = GetRandomDateTime();
            DateTimeOffset updatedDate = randomDate;

            List<ListenerEvent> expectedListenerEvents =
                retrievedEventListeners.Select(retrievedEventListener =>
                    new ListenerEvent
                    {
                        EventListenerId = retrievedEventListener.Id,
                        EventId = inputEvent.Id,
                        Status = ListenerEventStatus.Pending,
                        EventAddressId = eventAddressId,
                        CreatedDate = inputEvent.CreatedDate,
                        UpdatedDate = inputEvent.UpdatedDate
                    }).ToList();

            List<ListenerEvent> expectedListenerEventOnModify =
                expectedListenerEvents.DeepClone();

            List<EventCall> expectedCallEvents =
                retrievedEventListeners.Select(retrievedEventListener =>
                    new EventCall
                    {
                        Endpoint = retrievedEventListener.Endpoint,
                        Content = inputEvent.Content,
                        Secret = retrievedEventListener.HeaderSecret,
                        Response = null
                    }).ToList();

            List<Exception> eventCallExceptions =
                expectedCallEvents.Select(eventCall =>
                    new Exception(message: CreateRandomExceptionMessage()))
                        .ToList();

            var ranEventCalls = new List<EventCall>();

            this.eventListenerProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenersByAddressIdAsync(eventAddressId))
                    .ReturnsAsync(retrievedEventListeners);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(updatedDate);

            foreach (ListenerEvent expectedListenerEvent in expectedListenerEvents)
            {
                this.listenerEventProcessingServiceMock.Setup(service =>
                    service.AddListenerEventAsync(
                        It.Is(SameListenerEventAs(expectedListenerEvent))))
                            .ReturnsAsync(expectedListenerEvent.DeepClone());
            }

            for (int i = 0; i < expectedCallEvents.Count; i++)
            {
                var ranEventCall = new EventCall
                {
                    Endpoint = expectedCallEvents[i].Endpoint,
                    Content = expectedCallEvents[i].Content,
                    Response = eventCallExceptions[i].Message
                };

                this.eventCallProcessingServiceMock.Setup(service =>
                    service.RunAsync(
                        It.Is(SameEventCallAs(expectedCallEvents[i]))))
                            .ThrowsAsync(eventCallExceptions[i]);

                ranEventCalls.Add(ranEventCall);
            }

            for (int i = 0; i < retrievedEventListeners.Count(); i++)
            {
                expectedListenerEventOnModify[i].UpdatedDate = updatedDate;
                expectedListenerEventOnModify[i].Status = ListenerEventStatus.Completed;
                expectedListenerEventOnModify[i].Response = ranEventCalls[i].Response;

                this.listenerEventProcessingServiceMock.Setup(service =>
                    service.ModifyListenerEventAsync(
                        It.Is(SameListenerEventAs(expectedListenerEventOnModify[i]))))
                            .ReturnsAsync(expectedListenerEventOnModify[i]);
            }

            // when
            Event actualEvent =
                await this.eventListenerOrchestrationService
                    .SubmitEventToListenersAsync(inputEvent);

            // then
            actualEvent.Should().BeEquivalentTo(expectedEvent);

            this.eventListenerProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenersByAddressIdAsync(eventAddressId),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(retrievedEventListeners.Count()));

            foreach (ListenerEvent expectedListenerEvent in expectedListenerEvents)
            {
                this.listenerEventProcessingServiceMock.Verify(service =>
                    service.AddListenerEventAsync(
                        It.Is(SameListenerEventAs(expectedListenerEvent))),
                            Times.Once);
            }

            foreach (EventCall expectedCallEvent in expectedCallEvents)
            {
                this.eventCallProcessingServiceMock.Verify(service =>
                    service.RunAsync(
                        It.Is(SameEventCallAs(expectedCallEvent))),
                            Times.Once);
            }

            foreach (ListenerEvent expectedListenerEvent in expectedListenerEventOnModify)
            {
                this.listenerEventProcessingServiceMock.Verify(service =>
                    service.ModifyListenerEventAsync(
                        It.Is(SameListenerEventAs(expectedListenerEvent))),
                            Times.Once);
            }

            this.eventListenerProcessingServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.listenerEventProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
