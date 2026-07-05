// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Configurations.Healths;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Orchestrations.ParticipantSummaries.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ParticipantSummaries.V2
{
    public partial class ParticipantSummaryV2OrchestrationServiceTests
    {
        private readonly Mock<IEventAddressV2Service> eventAddressV2ServiceMock;
        private readonly Mock<IEventV2Service> eventV2ServiceMock;
        private readonly Mock<IConfigurationBroker> configurationBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IParticipantSummaryV2OrchestrationService participantSummaryV2OrchestrationService;

        public ParticipantSummaryV2OrchestrationServiceTests()
        {
            this.eventAddressV2ServiceMock = new Mock<IEventAddressV2Service>();
            this.eventV2ServiceMock = new Mock<IEventV2Service>();
            this.configurationBrokerMock = new Mock<IConfigurationBroker>();

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(new HealthConfiguration());

            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.participantSummaryV2OrchestrationService =
                new ParticipantSummaryV2OrchestrationService(
                    eventAddressV2Service: this.eventAddressV2ServiceMock.Object,
                    eventV2Service: this.eventV2ServiceMock.Object,
                    configurationBroker: this.configurationBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new EventAddressV2DependencyException(someMessage, someInnerException),
                new EventAddressV2ServiceException(someMessage, someInnerException),
                new EventV2DependencyException(someMessage, someInnerException),
                new EventV2ServiceException(someMessage, someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private void SetupParticipantFoundationMocks(
            CancellationToken cancellationToken,
            IQueryable<EventAddressV2> addresses,
            IQueryable<EventV2> events)
        {
            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(cancellationToken))
                    .ReturnsAsync(addresses);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(cancellationToken))
                    .ReturnsAsync(events);
        }

        private void VerifyParticipantFoundationMocksOnce(CancellationToken cancellationToken)
        {
            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(cancellationToken),
                    Times.Once);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(cancellationToken),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(),
                    Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static EventAddressV2 CreateEventAddressV2(
            Guid id,
            string name,
            string description)
        {
            return new EventAddressV2
            {
                Id = id,
                Name = name,
                Description = description,
                CreatedDate = GetRandomDateTimeOffset(),
                UpdatedDate = GetRandomDateTimeOffset()
            };
        }

        private static EventParticipantV2 CreateEventParticipantV2(
            Guid id,
            string name,
            string contactEmail,
            string contactPhone,
            bool isActive)
        {
            return new EventParticipantV2
            {
                Id = id,
                Name = name,
                Description = GetRandomString(),
                ContactEmail = contactEmail,
                ContactPhone = contactPhone,
                IsActive = isActive,
                CreatedDate = GetRandomDateTimeOffset(),
                UpdatedDate = GetRandomDateTimeOffset()
            };
        }

        private static EventV2 CreateEventV2ForParticipant(
            Guid id,
            Guid eventAddressId,
            Guid? participantId,
            EventParticipantV2 participant,
            DateTimeOffset createdDate,
            string contentHash,
            EventStatusV2 status)
        {
            return new EventV2
            {
                Id = id,
                Type = EventTypeV2.Immediate,
                Status = status,
                ContentHash = contentHash,
                Content = GetRandomString(),
                EventName = GetRandomString(),
                CreatedDate = createdDate,
                UpdatedDate = createdDate,
                EventAddressV2Id = eventAddressId,
                EventParticipantV2Id = participantId,
                EventParticipantV2 = participant
            };
        }

        private static EventListenerV2 CreateEventListenerV2ForParticipant(
            Guid id,
            Guid eventAddressId,
            Guid? participantId,
            EventParticipantV2 participant)
        {
            return new EventListenerV2
            {
                Id = id,
                Name = GetRandomString(),
                Description = GetRandomString(),
                CreatedDate = GetRandomDateTimeOffset(),
                UpdatedDate = GetRandomDateTimeOffset(),
                EventAddressV2Id = eventAddressId,
                EventParticipantV2Id = participantId,
                EventParticipantV2 = participant
            };
        }

        private static ListenerEventV2 CreateListenerEventV2ForEventAndListener(
            Guid eventId,
            Guid eventListenerId,
            Guid eventAddressId,
            DateTimeOffset createdDate,
            ListenerEventStatusV2 status)
        {
            return new ListenerEventV2
            {
                Id = Guid.NewGuid(),
                Status = status,
                CreatedDate = createdDate,
                UpdatedDate = createdDate,
                EventV2Id = eventId,
                EventAddressV2Id = eventAddressId,
                EventListenerV2Id = eventListenerId
            };
        }

        private static IQueryable<EventAddressV2> AttachEventListenerV2s(
            IQueryable<EventAddressV2> eventAddressV2s,
            IQueryable<EventListenerV2> eventListenerV2s)
        {
            List<EventAddressV2> addresses = eventAddressV2s.ToList();
            List<EventListenerV2> listeners = eventListenerV2s.ToList();

            for (int i = 0; i < addresses.Count; i++)
            {
                addresses[i].EventListenerV2s =
                    i == 0 ? listeners : new List<EventListenerV2>();
            }

            return addresses.AsQueryable();
        }

        private static IQueryable<EventV2> AttachListenerEventV2s(
            IQueryable<EventV2> eventV2s,
            IQueryable<ListenerEventV2> listenerEventV2s)
        {
            List<EventV2> events = eventV2s.ToList();
            List<ListenerEventV2> listenerEvents = listenerEventV2s.ToList();

            for (int i = 0; i < events.Count; i++)
            {
                events[i].ListenerEventV2s =
                    i == 0 ? listenerEvents : new List<ListenerEventV2>();
            }

            return events.AsQueryable();
        }
    }
}
