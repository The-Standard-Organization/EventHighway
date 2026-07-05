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
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using EventHighway.Core.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Orchestrations.RagStatuses.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RagStatuses.V2
{
    public partial class RagStatusV2OrchestrationServiceTests
    {
        private readonly Mock<IEventAddressV2Service> eventAddressV2ServiceMock;
        private readonly Mock<IEventV2Service> eventV2ServiceMock;
        private readonly Mock<IEventArchiveV2Service> eventArchiveV2ServiceMock;
        private readonly Mock<IConfigurationBroker> configurationBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IRagStatusV2OrchestrationService ragStatusV2OrchestrationService;

        public RagStatusV2OrchestrationServiceTests()
        {
            this.eventAddressV2ServiceMock = new Mock<IEventAddressV2Service>();
            this.eventV2ServiceMock = new Mock<IEventV2Service>();
            this.eventArchiveV2ServiceMock = new Mock<IEventArchiveV2Service>();
            this.configurationBrokerMock = new Mock<IConfigurationBroker>();

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(new HealthConfiguration());

            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.ragStatusV2OrchestrationService =
                new RagStatusV2OrchestrationService(
                    eventAddressV2Service: this.eventAddressV2ServiceMock.Object,
                    eventV2Service: this.eventV2ServiceMock.Object,
                    eventArchiveV2Service: this.eventArchiveV2ServiceMock.Object,
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
                new EventArchiveV2DependencyException(someMessage, someInnerException),
                new EventArchiveV2ServiceException(someMessage, someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private void SetupRagStatusFoundationMocks(
            CancellationToken cancellationToken,
            IQueryable<EventAddressV2> addresses,
            IQueryable<EventV2> events,
            IQueryable<EventArchiveV2> archives)
        {
            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(cancellationToken))
                    .ReturnsAsync(addresses);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(cancellationToken))
                    .ReturnsAsync(events);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync(cancellationToken))
                    .ReturnsAsync(archives);
        }

        private void VerifyRagStatusFoundationMocksOnce(CancellationToken cancellationToken)
        {
            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(cancellationToken),
                    Times.Once);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(cancellationToken),
                    Times.Once);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync(cancellationToken),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(),
                    Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        private static IQueryable<EventAddressV2> CreateAddressesWithListeners(
            int listenerCount,
            int handlerCount)
        {
            return AttachEventListenerV2s(
                CreateRandomEventAddressV2s(),
                CreateRandomEventListenerV2s(listenerCount, handlerCount));
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomPositiveNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static IQueryable<EventAddressV2> CreateRandomEventAddressV2s(int count = 0)
        {
            int actualCount = count > 0 ? count : GetRandomPositiveNumber();

            return Enumerable.Range(0, actualCount)
                .Select(_ => new EventAddressV2
                {
                    Id = Guid.NewGuid(),
                    Name = GetRandomString(),
                    Description = GetRandomString(),
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset()
                })
                .AsQueryable();
        }

        private static IQueryable<EventListenerV2> CreateRandomEventListenerV2s(
            int listenerCount,
            int handlerCount)
        {
            if (listenerCount == 0)
            {
                return Enumerable.Empty<EventListenerV2>().AsQueryable();
            }

            List<Guid> handlerIds = Enumerable.Range(0, handlerCount)
                .Select(_ => Guid.NewGuid())
                .ToList();

            return Enumerable.Range(0, listenerCount)
                .Select(i => new EventListenerV2
                {
                    Id = Guid.NewGuid(),
                    Name = GetRandomString(),
                    Description = GetRandomString(),
                    HandlerId = handlerIds[i % handlerCount],
                    HandlerName = GetRandomString(),
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    EventAddressV2Id = Guid.NewGuid()
                })
                .AsQueryable();
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

        private static IQueryable<EventV2> CreateRandomEventV2s(
            int immediateCount = 0,
            int scheduledCount = 0,
            int deadCount = 0,
            int quarantinedCount = 0)
        {
            var events = new List<EventV2>();

            int immCount = immediateCount > 0 ? immediateCount : GetRandomPositiveNumber();
            int schCount = scheduledCount > 0 ? scheduledCount : GetRandomPositiveNumber();

            events.AddRange(CreateEventV2s(immCount, EventTypeV2.Immediate, EventStatusV2.Active, positiveRetries: true));
            events.AddRange(CreateEventV2s(schCount, EventTypeV2.Scheduled, EventStatusV2.Active, positiveRetries: true));
            events.AddRange(CreateEventV2s(deadCount, EventTypeV2.Immediate, EventStatusV2.Active, positiveRetries: false));
            events.AddRange(CreateEventV2s(quarantinedCount, EventTypeV2.Immediate, EventStatusV2.Quarantined, positiveRetries: true));

            return events.AsQueryable();
        }

        private static IEnumerable<EventV2> CreateEventV2s(
            int count,
            EventTypeV2 type,
            EventStatusV2 status,
            bool positiveRetries)
        {
            return Enumerable.Range(0, count)
                .Select(_ => new EventV2
                {
                    Id = Guid.NewGuid(),
                    Type = type,
                    Status = status,
                    Content = GetRandomString(),
                    EventName = GetRandomString(),
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    EventAddressV2Id = Guid.NewGuid()
                });
        }

        private static IQueryable<ListenerEventV2> CreateRandomListenerEventV2s(
            int successCount = 0,
            int pendingCount = 0,
            int errorCount = 0,
            int replayCount = 0)
        {
            var listenerEvents = new List<ListenerEventV2>();

            int sucCount = successCount > 0 ? successCount : GetRandomPositiveNumber();

            listenerEvents.AddRange(CreateListenerEventV2s(sucCount, ListenerEventStatusV2.Success));
            listenerEvents.AddRange(CreateListenerEventV2s(pendingCount, ListenerEventStatusV2.Pending));
            listenerEvents.AddRange(CreateListenerEventV2s(errorCount, ListenerEventStatusV2.Error));
            listenerEvents.AddRange(CreateListenerEventV2s(replayCount, ListenerEventStatusV2.Replay));

            return listenerEvents.AsQueryable();
        }

        private static IEnumerable<ListenerEventV2> CreateListenerEventV2s(
            int count,
            ListenerEventStatusV2 status)
        {
            return Enumerable.Range(0, count)
                .Select(_ => new ListenerEventV2
                {
                    Id = Guid.NewGuid(),
                    Status = status,
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    EventV2Id = Guid.NewGuid(),
                    EventAddressV2Id = Guid.NewGuid(),
                    EventListenerV2Id = Guid.NewGuid()
                });
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

        private static IQueryable<EventArchiveV2> CreateRandomEventArchiveV2s(
            int count = 0,
            int quarantinedCount = 0,
            int deadCount = 0)
        {
            var archives = new List<EventArchiveV2>();
            int activeCount = count > 0 ? count : GetRandomPositiveNumber();

            archives.AddRange(CreateEventArchiveV2s(activeCount, EventArchiveStatusV2.Active, positiveRetries: true));
            archives.AddRange(CreateEventArchiveV2s(quarantinedCount, EventArchiveStatusV2.Quarantined, positiveRetries: true));
            archives.AddRange(CreateEventArchiveV2s(deadCount, EventArchiveStatusV2.Active, positiveRetries: false));

            return archives.AsQueryable();
        }

        private static IEnumerable<EventArchiveV2> CreateEventArchiveV2s(
            int count,
            EventArchiveStatusV2 status,
            bool positiveRetries)
        {
            return Enumerable.Range(0, count)
                .Select(_ => new EventArchiveV2
                {
                    Id = Guid.NewGuid(),
                    Status = status,
                    Content = GetRandomString(),
                    EventName = GetRandomString(),
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    ArchivedDate = GetRandomDateTimeOffset(),
                    EventAddressV2Id = Guid.NewGuid()
                });
        }

        private static IQueryable<ListenerEventArchiveV2> CreateRandomListenerEventArchiveV2s(
            int successCount = 0,
            int errorCount = 0)
        {
            var archives = new List<ListenerEventArchiveV2>();
            int sucCount = successCount > 0 ? successCount : GetRandomPositiveNumber();

            archives.AddRange(CreateListenerEventArchiveV2s(sucCount, ListenerEventArchiveStatusV2.Success));
            archives.AddRange(CreateListenerEventArchiveV2s(errorCount, ListenerEventArchiveStatusV2.Error));

            return archives.AsQueryable();
        }

        private static IEnumerable<ListenerEventArchiveV2> CreateListenerEventArchiveV2s(
            int count,
            ListenerEventArchiveStatusV2 status)
        {
            return Enumerable.Range(0, count)
                .Select(_ => new ListenerEventArchiveV2
                {
                    Id = Guid.NewGuid(),
                    Status = status,
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    ArchivedDate = GetRandomDateTimeOffset(),
                    EventV2Id = Guid.NewGuid(),
                    EventAddressV2Id = Guid.NewGuid(),
                    EventListenerV2Id = Guid.NewGuid()
                });
        }

        private static IQueryable<EventArchiveV2> AttachListenerEventArchiveV2s(
            IQueryable<EventArchiveV2> eventArchiveV2s,
            IQueryable<ListenerEventArchiveV2> listenerEventArchiveV2s)
        {
            List<EventArchiveV2> eventArchives = eventArchiveV2s.ToList();
            List<ListenerEventArchiveV2> listenerArchives = listenerEventArchiveV2s.ToList();

            for (int i = 0; i < eventArchives.Count; i++)
            {
                eventArchives[i].ListenerEventArchiveV2s =
                    i == 0 ? listenerArchives : new List<ListenerEventArchiveV2>();
            }

            return eventArchives.AsQueryable();
        }
    }
}
