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
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V1
{
    public partial class EventV1CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldFireScheduledPendingEventV1sAsync()
        {
            // given
            IQueryable<EventV1> randomEventV1s = CreateRandomEventV1s();
            IQueryable<EventV1> retrievedEventV1s = randomEventV1s;

            IQueryable<EventListenerV1> randomEventListenerV1s =
                CreateRandomEventListenerV1s();

            IQueryable<EventListenerV1> retrievedEventListenerV1s =
                randomEventListenerV1s;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset retrievedDateTimeOffset = randomDateTimeOffset;

            List<ListenerEventV1> inputListenerEventV1s =
                retrievedEventV1s.SelectMany(eventV1 =>
                    retrievedEventListenerV1s.Select(eventListenerV1 =>
                        new ListenerEventV1
                        {
                            EventListenerId = eventListenerV1.Id,
                            EventId = eventV1.Id,
                            Status = ListenerEventStatusV1.Pending,
                            EventAddressId = eventV1.EventAddressId,
                            CreatedDate = retrievedDateTimeOffset,
                            UpdatedDate = retrievedDateTimeOffset
                        })).ToList();

            List<ListenerEventV1> expectedListenerEventV1s =
                inputListenerEventV1s.DeepClone();

            List<EventCallV1> expectedCallEventV1s =
                retrievedEventV1s.SelectMany(eventV1 =>
                    retrievedEventListenerV1s.Select(
                        retrievedEventListenerV1 =>
                            new EventCallV1
                            {
                                Endpoint = retrievedEventListenerV1.Endpoint,
                                Content = eventV1.Content,
                                Secret = retrievedEventListenerV1.HeaderSecret,
                            })).ToList();

            int expectedDateTimeBrokerCalls =
                inputListenerEventV1s.Count + expectedListenerEventV1s.Count;

            var ranEventCallV1s = new List<EventCallV1>();

            this.eventV1OrchestrationServiceMock.Setup(service =>
                service.RetrieveScheduledPendingEventV1sAsync())
                    .ReturnsAsync(retrievedEventV1s);

            foreach (EventV1 retrievedEventV1 in retrievedEventV1s)
            {
                this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                    service.RetrieveEventListenerV1sByEventAddressIdAsync(
                        retrievedEventV1.EventAddressId))
                            .ReturnsAsync(retrievedEventListenerV1s);
            }

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(retrievedDateTimeOffset);

            foreach (ListenerEventV1 expectedListenerEventV1 in inputListenerEventV1s)
            {
                this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                    service.AddListenerEventV1Async(
                        It.Is(SameListenerEventAs(expectedListenerEventV1))))
                            .ReturnsAsync(expectedListenerEventV1.DeepClone());
            }

            foreach (EventCallV1 expectedCallEventV1 in expectedCallEventV1s)
            {
                var ranEventCall = new EventCallV1
                {
                    Endpoint = expectedCallEventV1.Endpoint,
                    Content = expectedCallEventV1.Content,
                    Response = GetRandomString()
                };

                this.eventV1OrchestrationServiceMock.Setup(service =>
                    service.RunEventCallV1Async(
                        It.Is(SameEventCallAs(expectedCallEventV1))))
                            .ReturnsAsync(ranEventCall);

                ranEventCallV1s.Add(item: ranEventCall);
            }

            for (int index = 0; index < inputListenerEventV1s.Count; index++)
            {
                expectedListenerEventV1s[index].UpdatedDate = retrievedDateTimeOffset;
                expectedListenerEventV1s[index].Status = ListenerEventStatusV1.Success;
                expectedListenerEventV1s[index].Response = ranEventCallV1s[index].Response;

                this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetDateTimeOffsetAsync())
                        .ReturnsAsync(retrievedDateTimeOffset);

                this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                    service.ModifyListenerEventV1Async(
                        It.Is(SameListenerEventAs(expectedListenerEventV1s[index]))))
                            .ReturnsAsync(expectedListenerEventV1s[index]);
            }

            // when
            await this.eventV1CoordinationService
                .FireScheduledPendingEventV1sAsync();

            // then
            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveScheduledPendingEventV1sAsync(),
                    Times.Once);

            foreach (EventV1 retreivedEventV1 in retrievedEventV1s)
            {
                this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                    service.RetrieveEventListenerV1sByEventAddressIdAsync(
                        retreivedEventV1.EventAddressId),
                            Times.Once);
            }

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(callCount: expectedDateTimeBrokerCalls));

            foreach (ListenerEventV1 expectedListenerEventV1 in inputListenerEventV1s)
            {
                this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                    service.AddListenerEventV1Async(
                        It.Is(SameListenerEventAs(expectedListenerEventV1))),
                            Times.Once);
            }

            foreach (EventCallV1 expectedCallEventV1 in expectedCallEventV1s)
            {
                this.eventV1OrchestrationServiceMock.Verify(service =>
                    service.RunEventCallV1Async(
                        It.Is(SameEventCallAs(expectedCallEventV1))),
                            Times.Once);
            }

            foreach (ListenerEventV1 expectedListenerEventV1 in expectedListenerEventV1s)
            {
                this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                    service.ModifyListenerEventV1Async(
                        It.Is(SameListenerEventAs(expectedListenerEventV1))),
                            Times.Once);
            }

            foreach (EventV1 retreivedEventV1 in retrievedEventV1s)
            {
                this.eventV1OrchestrationServiceMock.Verify(service =>
                    service.MarkEventV1AsImmediateAsync(retreivedEventV1),
                        Times.Once);
            }

            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRecordFailuresOnFireScheduledPendingEventV1sAsync()
        {
            // given
            IQueryable<EventV1> randomEventV1s = CreateRandomEventV1s();
            IQueryable<EventV1> retrievedEventV1s = randomEventV1s;

            IQueryable<EventListenerV1> randomEventListenerV1s =
                CreateRandomEventListenerV1s();

            IQueryable<EventListenerV1> retrievedEventListenerV1s =
                randomEventListenerV1s;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset retrievedDateTimeOffset = randomDateTimeOffset;

            List<ListenerEventV1> expectedListenerEventV1s =
                retrievedEventV1s.SelectMany(eventV1 =>
                    retrievedEventListenerV1s.Select(eventListenerV1 =>
                        new ListenerEventV1
                        {
                            EventListenerId = eventListenerV1.Id,
                            EventId = eventV1.Id,
                            Status = ListenerEventStatusV1.Pending,
                            EventAddressId = eventV1.EventAddressId,
                            CreatedDate = retrievedDateTimeOffset,
                            UpdatedDate = retrievedDateTimeOffset
                        })).ToList();


            List<ListenerEventV1> expectedListenerEventV1sOnModify =
                expectedListenerEventV1s.DeepClone();

            List<EventCallV1> expectedCallEventV1s =
                retrievedEventV1s.SelectMany(eventV1 =>
                    retrievedEventListenerV1s.Select(
                        retrievedEventListenerV1 =>
                            new EventCallV1
                            {
                                Endpoint = retrievedEventListenerV1.Endpoint,
                                Content = eventV1.Content,
                                Secret = retrievedEventListenerV1.HeaderSecret,
                            })).ToList();

            List<Exception> eventCallExceptions =
                expectedCallEventV1s.Select(eventCall =>
                    new Exception(message: GetRandomString()))
                        .ToList();

            int expectedDateTimeBrokerCalls =
                expectedListenerEventV1s.Count + expectedListenerEventV1sOnModify.Count;

            var ranEventCallV1s = new List<EventCallV1>();

            this.eventV1OrchestrationServiceMock.Setup(service =>
                service.RetrieveScheduledPendingEventV1sAsync())
                    .ReturnsAsync(retrievedEventV1s);

            foreach (EventV1 eventV1 in retrievedEventV1s)
            {
                this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                    service.RetrieveEventListenerV1sByEventAddressIdAsync(
                        eventV1.EventAddressId))
                            .ReturnsAsync(retrievedEventListenerV1s);
            }

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(retrievedDateTimeOffset);

            foreach (ListenerEventV1 expectedListenerEventV1 in expectedListenerEventV1s)
            {
                this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                    service.AddListenerEventV1Async(
                        It.Is(SameListenerEventAs(expectedListenerEventV1))))
                            .ReturnsAsync(expectedListenerEventV1.DeepClone());
            }

            for (int index = 0; index < expectedCallEventV1s.Count; index++)
            {
                var ranEventCall = new EventCallV1
                {
                    Endpoint = expectedCallEventV1s[index].Endpoint,
                    Content = expectedCallEventV1s[index].Content,
                    Response = eventCallExceptions[index].Message
                };

                this.eventV1OrchestrationServiceMock.Setup(service =>
                    service.RunEventCallV1Async(
                        It.Is(SameEventCallAs(expectedCallEventV1s[index]))))
                            .ThrowsAsync(eventCallExceptions[index]);

                ranEventCallV1s.Add(item: ranEventCall);
            }

            for (int index = 0; index < expectedListenerEventV1s.Count; index++)
            {
                expectedListenerEventV1sOnModify[index].UpdatedDate = retrievedDateTimeOffset;
                expectedListenerEventV1sOnModify[index].Status = ListenerEventStatusV1.Error;
                expectedListenerEventV1sOnModify[index].Response = ranEventCallV1s[index].Response;

                this.eventV1OrchestrationServiceMock.Setup(service =>
                    service.RetrieveScheduledPendingEventV1sAsync())
                        .ReturnsAsync(retrievedEventV1s);

                this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                    service.ModifyListenerEventV1Async(
                        It.Is(SameListenerEventAs(expectedListenerEventV1sOnModify[index]))))
                            .ReturnsAsync(expectedListenerEventV1sOnModify[index]);
            }

            // when
            await this.eventV1CoordinationService
                .FireScheduledPendingEventV1sAsync();

            // then
            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveScheduledPendingEventV1sAsync(),
                    Times.Once);

            foreach (EventV1 eventV1 in retrievedEventV1s)
            {
                this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                    service.RetrieveEventListenerV1sByEventAddressIdAsync(
                        eventV1.EventAddressId),
                            Times.Once);
            }

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(callCount: expectedDateTimeBrokerCalls));

            foreach (ListenerEventV1 expectedListenerEventV1 in expectedListenerEventV1s)
            {
                this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                    service.AddListenerEventV1Async(
                        It.Is(SameListenerEventAs(expectedListenerEventV1))),
                            Times.Once);
            }

            foreach (EventCallV1 expectedCallEventV1 in expectedCallEventV1s)
            {
                this.eventV1OrchestrationServiceMock.Verify(service =>
                    service.RunEventCallV1Async(
                        It.Is(SameEventCallAs(expectedCallEventV1))),
                            Times.Once);
            }

            foreach (ListenerEventV1 expectedListenerEventV1 in expectedListenerEventV1sOnModify)
            {
                this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                    service.ModifyListenerEventV1Async(
                        It.Is(SameListenerEventAs(expectedListenerEventV1))),
                            Times.Once);
            }

            foreach (EventV1 retreivedEventV1 in retrievedEventV1s)
            {
                this.eventV1OrchestrationServiceMock.Verify(service =>
                    service.MarkEventV1AsImmediateAsync(retreivedEventV1),
                        Times.Once);
            }

            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
