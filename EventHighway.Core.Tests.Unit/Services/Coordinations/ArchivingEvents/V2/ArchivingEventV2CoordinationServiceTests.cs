// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.ArchivingEvents.V2;
using EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2;
using EventHighway.Core.Services.Orchestrations.EventArchives.V2;
using EventHighway.Core.Services.Orchestrations.ListenerEvents.V2;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2CoordinationServiceTests
    {
        private readonly Mock<IArchivingEventV2OrchestrationService> archivingEventV2OrchestrationServiceMock;
        private readonly Mock<IEventArchiveV2OrchestrationService> eventArchiveV2OrchestrationServiceMock;
        private readonly Mock<IListenerEventV2OrchestrationService> listenerEventV2OrchestrationServiceMock;
        private readonly Mock<IConfigurationBroker> configurationBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ICompareLogic compareLogic;
        private readonly IArchivingEventV2CoordinationService archivingEventV2CoordinationService;

        public ArchivingEventV2CoordinationServiceTests()
        {
            this.archivingEventV2OrchestrationServiceMock =
                new Mock<IArchivingEventV2OrchestrationService>(
                    behavior: MockBehavior.Strict);

            this.eventArchiveV2OrchestrationServiceMock =
                new Mock<IEventArchiveV2OrchestrationService>(
                    behavior: MockBehavior.Strict);

            this.listenerEventV2OrchestrationServiceMock =
                new Mock<IListenerEventV2OrchestrationService>(
                    behavior: MockBehavior.Strict);

            this.configurationBrokerMock = new Mock<IConfigurationBroker>(
                behavior: MockBehavior.Strict);

            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>(
                behavior: MockBehavior.Strict);

            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            var compareConfiguration = new ComparisonConfig();
            this.compareLogic = new CompareLogic(compareConfiguration);

            this.archivingEventV2CoordinationService =
                new ArchivingEventV2CoordinationService(
                    archivingEventV2OrchestrationService:
                        this.archivingEventV2OrchestrationServiceMock.Object,
                    eventArchiveV2OrchestrationService:
                        this.eventArchiveV2OrchestrationServiceMock.Object,
                    listenerEventV2OrchestrationService:
                        this.listenerEventV2OrchestrationServiceMock.Object,
                    configurationBroker: this.configurationBrokerMock.Object,
                    dateTimeBroker: this.dateTimeBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> ArchivingEventV2ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new ArchivingEventV2OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new ArchivingEventV2OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> ArchivingEventV2DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new ArchivingEventV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new ArchivingEventV2OrchestrationServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> EventArchiveV2ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new EventArchiveV2OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new EventArchiveV2OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> EventArchiveV2DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new EventArchiveV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new EventArchiveV2OrchestrationServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new ArchivingEventV2OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new ArchivingEventV2OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException),

                new EventArchiveV2OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new EventArchiveV2OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new ArchivingEventV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new ArchivingEventV2OrchestrationServiceException(
                    someMessage,
                    someInnerException),

                new EventArchiveV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new EventArchiveV2OrchestrationServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static T GetRandomEnum<T>()
        {
            int randomNumber =
                new IntRange(
                    min: 0,

                    max: Enum.GetValues(
                        enumType: typeof(T)).Length)
                            .GetValue();

            return (T)(object)randomNumber;
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static BatchConfiguration CreateRandomBatchConfiguration() =>
            new BatchConfiguration
            {
                BatchSizeForBulkProcessing = GetRandomNumber()
            };

        private Expression<Func<EventV2, bool>> SameEventV2As(EventV2 expectedEventV2)
        {
            return actualEventV2 =>
                this.compareLogic.Compare(
                    expectedEventV2,
                    actualEventV2)
                        .AreEqual;
        }

        private Expression<Func<EventArchiveV2, bool>> SameEventArchiveV2As(
            EventArchiveV2 expectedEventArchiveV2)
        {
            return actualEventArchiveV2 =>
                this.compareLogic.Compare(
                    expectedEventArchiveV2,
                    actualEventArchiveV2)
                        .AreEqual;
        }

        private Expression<Func<IEnumerable<EventArchiveV2>, bool>> SameEventArchiveV2sAs(
            IEnumerable<EventArchiveV2> expectedEventArchiveV2s)
        {
            return actualEventArchiveV2s =>
                this.compareLogic.Compare(
                    expectedEventArchiveV2s,
                    actualEventArchiveV2s)
                        .AreEqual;
        }

        private Expression<Func<IEnumerable<EventV2>, bool>> SameEventV2sAs(
            IEnumerable<EventV2> expectedEventV2s)
        {
            return actualEventV2s =>
                this.compareLogic.Compare(
                    expectedEventV2s,
                    actualEventV2s)
                        .AreEqual;
        }

        private Expression<Func<IEnumerable<Guid>, bool>> SameEventV2IdsAs(
            IEnumerable<Guid> expectedEventV2Ids)
        {
            return actualEventV2Ids =>
                this.compareLogic.Compare(
                    expectedEventV2Ids,
                    actualEventV2Ids)
                        .AreEqual;
        }

        private Expression<Func<IEnumerable<ListenerEventV2>, bool>> SameListenerEventV2sAs(
            IEnumerable<ListenerEventV2> expectedListenerEventV2s)
        {
            return actualListenerEventV2s =>
                this.compareLogic.Compare(
                    expectedListenerEventV2s,
                    actualListenerEventV2s)
                        .AreEqual;
        }

        private Expression<Func<IEnumerable<ListenerEventArchiveV2>, bool>> SameListenerEventArchiveV2sAs(
            IEnumerable<ListenerEventArchiveV2> expectedListenerEventArchiveV2s)
        {
            return actualListenerEventArchiveV2s =>
                this.compareLogic.Compare(
                    expectedListenerEventArchiveV2s,
                    actualListenerEventArchiveV2s)
                        .AreEqual;
        }

        private static IEnumerable<EventArchiveV2> MapToEventArchiveV2sWithListenerEventArchiveV2s(
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s)
        {
            return listenerEventArchiveV2s
                .GroupBy(listenerEventArchiveV2 => listenerEventArchiveV2.EventArchiveV2Id)
                .Select(listenerEventArchiveV2Group => new EventArchiveV2
                {
                    Id = listenerEventArchiveV2Group.Key,
                    ListenerEventArchiveV2s = listenerEventArchiveV2Group.ToList()
                }).ToList();
        }

        private static List<dynamic> CreateRandomEventV2sProperties()
        {
            int randomCount = GetRandomNumber();

            return Enumerable.Range(start: 0, count: randomCount)
                .Select(item => CreateRandomEventV2Properties())
                    .ToList();
        }

        private static List<dynamic> CreateRandomListenerEventV2sProperties()
        {
            int randomCount = GetRandomNumber();

            return Enumerable.Range(start: 0, count: randomCount)
                .Select(item => CreateRandomListenerEventV2Properties())
                    .ToList();
        }

        private static dynamic CreateRandomEventV2Properties()
        {
            Guid randomId = GetRandomId();
            string randomContent = GetRandomString();
            string randomEventName = GetRandomString();

            EventTypeV2 randomType =
                GetRandomEnum<EventTypeV2>();

            DateTimeOffset randomCreatedDate =
                GetRandomDateTimeOffset();

            DateTimeOffset randomUpdatedDate =
                GetRandomDateTimeOffset();

            DateTimeOffset randomScheduledDate =
                GetRandomDateTimeOffset();

            Guid randomEventAddressId = GetRandomId();
            int randomRemainingRetryAttempts = GetRandomNumber();

            return new
            {
                Id = randomId,
                Content = randomContent,
                EventName = randomEventName,
                Type = randomType,
                CreatedDate = randomCreatedDate,
                UpdatedDate = randomUpdatedDate,
                ScheduledDate = randomScheduledDate,
                RemainingRetryAttempts = randomRemainingRetryAttempts,
                EventAddressId = randomEventAddressId
            };
        }

        private static dynamic CreateRandomListenerEventV2Properties()
        {
            Guid randomId = GetRandomId();

            ListenerEventStatusV2 randomStatus =
                GetRandomEnum<ListenerEventStatusV2>();

            string randomResponse = GetRandomString();
            string randomResponseCode = GetRandomString();
            string randomResponseMessage = GetRandomString();

            DateTimeOffset randomCreatedDate =
                GetRandomDateTimeOffset();

            DateTimeOffset randomUpdatedDate =
                GetRandomDateTimeOffset();

            Guid randomEventId = GetRandomId();
            Guid randomEventAddressId = GetRandomId();
            Guid randomEventListenerId = GetRandomId();
            Guid randomEventParticipantId = GetRandomId();

            return new
            {
                Id = randomId,
                Status = randomStatus,
                Response = randomResponse,
                ResponseCode = randomResponseCode,
                ResponseMessage = randomResponseMessage,
                CreatedDate = randomCreatedDate,
                UpdatedDate = randomUpdatedDate,
                EventId = randomEventId,
                EventAddressId = randomEventAddressId,
                EventListenerId = randomEventListenerId,
                EventParticipantId = randomEventParticipantId
            };
        }

        private static IQueryable<EventV2> CreateRandomEventV2s()
        {
            return CreateEventV2Filler()
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static IEnumerable<EventArchiveV2> CreateRandomEventArchiveV2s()
        {
            return CreateEventArchiveV2Filler()
                .Create(count: GetRandomNumber())
                    .ToList();
        }

        private static Filler<EventArchiveV2> CreateEventArchiveV2Filler()
        {
            var filler = new Filler<EventArchiveV2>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset())

                .OnProperty(eventArchiveV2 => eventArchiveV2.ListenerEventArchiveV2s)
                    .IgnoreIt()

                .OnProperty(eventArchiveV2 => eventArchiveV2.EventAddressV2)
                    .IgnoreIt()

                .OnType<EventParticipantV2>().IgnoreIt();

            return filler;
        }

        private static async IAsyncEnumerable<EventV2> CreateAsyncEnumerable(
            IEnumerable<EventV2> eventV2s)
        {
            foreach (EventV2 eventV2 in eventV2s)
                yield return eventV2;

            await Task.CompletedTask;
        }

        private static async IAsyncEnumerable<EventV2> CreateThrowingAsyncEnumerable(Exception exception)
        {
            await Task.CompletedTask;
            throw exception;
#pragma warning disable CS0162
            yield break;
#pragma warning restore CS0162
        }

        private static Filler<EventV2> CreateEventV2Filler()
        {
            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset())

                .OnType<EventAddressV2>()
                    .IgnoreIt()

                .OnType<EventListenerV2>()
                    .IgnoreIt()

                .OnProperty(eventV2 =>
                    eventV2.EventAddressV2).IgnoreIt()

                .OnProperty(eventV2 =>
                    eventV2.ListenerEventV2s).IgnoreIt()

                .OnType<EventParticipantV2>().IgnoreIt();

            return filler;
        }
    }
}
