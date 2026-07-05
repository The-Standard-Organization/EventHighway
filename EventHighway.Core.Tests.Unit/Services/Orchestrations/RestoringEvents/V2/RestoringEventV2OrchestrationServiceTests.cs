// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.RestoringEvents.V2;
using EventHighway.Core.Services.Processings.EventListeners.V2;
using EventHighway.Core.Services.Processings.Events.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RestoringEvents.V2
{
    public partial class RestoringEventV2OrchestrationServiceTests
    {
        private readonly Mock<IEventV2ProcessingService> eventV2ProcessingServiceMock;
        private readonly Mock<IListenerEventV2ProcessingService> listenerEventV2ProcessingServiceMock;
        private readonly Mock<IEventListenerV2ProcessingService> eventListenerV2ProcessingServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IRestoringEventV2OrchestrationService restoringEventV2OrchestrationService;

        public RestoringEventV2OrchestrationServiceTests()
        {
            this.eventV2ProcessingServiceMock = new Mock<IEventV2ProcessingService>();
            this.listenerEventV2ProcessingServiceMock = new Mock<IListenerEventV2ProcessingService>();
            this.eventListenerV2ProcessingServiceMock = new Mock<IEventListenerV2ProcessingService>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.restoringEventV2OrchestrationService = new RestoringEventV2OrchestrationService(
                eventV2ProcessingService: this.eventV2ProcessingServiceMock.Object,
                listenerEventV2ProcessingService: this.listenerEventV2ProcessingServiceMock.Object,
                eventListenerV2ProcessingService: this.eventListenerV2ProcessingServiceMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);

            return new TheoryData<Xeption>
            {
                new EventV2ProcessingValidationException(someMessage, someInnerException),
                new EventV2ProcessingDependencyValidationException(someMessage, someInnerException),
                new ListenerEventV2ProcessingValidationException(someMessage, someInnerException),
                new ListenerEventV2ProcessingDependencyValidationException(someMessage, someInnerException),
                new EventListenerV2ProcessingValidationException(someMessage, someInnerException),
                new EventListenerV2ProcessingDependencyValidationException(someMessage, someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);

            return new TheoryData<Xeption>
            {
                new EventV2ProcessingDependencyException(someMessage, someInnerException),
                new EventV2ProcessingServiceException(someMessage, someInnerException),
                new ListenerEventV2ProcessingDependencyException(someMessage, someInnerException),
                new ListenerEventV2ProcessingServiceException(someMessage, someInnerException),
                new EventListenerV2ProcessingDependencyException(someMessage, someInnerException),
                new EventListenerV2ProcessingServiceException(someMessage, someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static bool SameEventV2sAs(
            List<EventV2> expectedEventV2s,
            List<EventV2> actualEventV2s)
        {
            var compareLogic = new CompareLogic();

            return compareLogic.Compare(expectedEventV2s, actualEventV2s).AreEqual;
        }

        private static bool SameListenerEventV2sAs(
            List<ListenerEventV2> expectedListenerEventV2s,
            List<ListenerEventV2> actualListenerEventV2s)
        {
            var compareLogic = new CompareLogic();

            return compareLogic.Compare(expectedListenerEventV2s, actualListenerEventV2s).AreEqual;
        }

        private static bool SameGeneratedListenerEventV2sAs(
            List<ListenerEventV2> expectedListenerEventV2s,
            List<ListenerEventV2> actualListenerEventV2s)
        {
            var compareLogic = new CompareLogic(
                new ComparisonConfig
                {
                    MembersToIgnore = new List<string> { nameof(ListenerEventV2.Id) }
                });

            bool fieldsMatch =
                compareLogic.Compare(expectedListenerEventV2s, actualListenerEventV2s).AreEqual;

            bool idsGenerated =
                actualListenerEventV2s.All(listenerEventV2 => listenerEventV2.Id != Guid.Empty);

            return fieldsMatch && idsGenerated;
        }

        private static EventV2 MapToEventV2(EventArchiveV2 eventArchiveV2) =>
            new EventV2
            {
                Id = eventArchiveV2.Id,
                Content = eventArchiveV2.Content,
                EventName = eventArchiveV2.EventName,
                Type = EventTypeV2.Immediate,
                Status = EventStatusV2.Active,
                CreatedDate = eventArchiveV2.CreatedDate,
                UpdatedDate = eventArchiveV2.UpdatedDate,
                ScheduledDate = null,
                ContentHash = null,
                EventAddressV2Id = eventArchiveV2.EventAddressV2Id
            };

        private static ListenerEventV2 MapToListenerEventV2(ListenerEventArchiveV2 listenerEventArchiveV2) =>
            new ListenerEventV2
            {
                Id = listenerEventArchiveV2.Id,
                CorrelationId = listenerEventArchiveV2.Id,
                Status = ListenerEventStatusV2.Replay,
                Response = null,
                ResponseCode = null,
                ResponseMessage = null,
                CreatedDate = listenerEventArchiveV2.CreatedDate,
                UpdatedDate = listenerEventArchiveV2.UpdatedDate,
                EventV2Id = listenerEventArchiveV2.EventV2Id,
                EventAddressV2Id = listenerEventArchiveV2.EventAddressV2Id,
                EventListenerV2Id = listenerEventArchiveV2.EventListenerV2Id
            };

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static List<EventArchiveV2> CreateRandomEventArchiveV2s() =>
            CreateEventArchiveV2Filler().Create(count: GetRandomNumber()).ToList();

        private static List<ListenerEventArchiveV2> CreateRandomListenerEventArchiveV2s() =>
            CreateListenerEventArchiveV2Filler().Create(count: GetRandomNumber()).ToList();

        private static List<EventListenerV2> CreateRandomEventListenerV2s() =>
            CreateEventListenerV2Filler().Create(count: GetRandomNumber()).ToList();

        private static Filler<EventArchiveV2> CreateEventArchiveV2Filler()
        {
            var filler = new Filler<EventArchiveV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnType<DateTimeOffset?>().Use(GetRandomDateTimeOffset())
                .OnType<EventParticipantV2>().IgnoreIt()

                .OnProperty(eventArchiveV2 => eventArchiveV2.ListenerEventArchiveV2s)
                    .IgnoreIt()

                .OnProperty(eventArchiveV2 => eventArchiveV2.EventAddressV2)
                    .IgnoreIt();

            return filler;
        }

        private static Filler<ListenerEventArchiveV2> CreateListenerEventArchiveV2Filler()
        {
            var filler = new Filler<ListenerEventArchiveV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)

                .OnProperty(listenerEventArchiveV2 => listenerEventArchiveV2.EventListenerV2)
                    .IgnoreIt()

                .OnType<EventParticipantV2>().IgnoreIt();

            return filler;
        }

        private static Filler<EventListenerV2> CreateEventListenerV2Filler()
        {
            var filler = new Filler<EventListenerV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnType<DateTimeOffset?>().Use(GetRandomDateTimeOffset())

                .OnProperty(eventListenerV2 => eventListenerV2.EventAddressV2)
                    .IgnoreIt()

                .OnProperty(eventListenerV2 => eventListenerV2.ListenerEventV2s)
                    .IgnoreIt()

                .OnProperty(eventListenerV2 => eventListenerV2.ListenerEventArchiveV2s)
                    .IgnoreIt()

                .OnProperty(eventListenerV2 => eventListenerV2.EventParticipantV2)
                    .IgnoreIt();

            return filler;
        }
    }
}
