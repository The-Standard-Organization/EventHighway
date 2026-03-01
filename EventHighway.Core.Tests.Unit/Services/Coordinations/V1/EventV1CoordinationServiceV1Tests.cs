// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;
using EventHighway.Core.Services.Coordinations.Events.V1;
using EventHighway.Core.Services.Orchestrations.EventArchives.V1;
using EventHighway.Core.Services.Orchestrations.Events.V1;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V1
{
    public partial class EventV1CoordinationServiceV1Tests
    {
        private readonly Mock<IEventV1OrchestrationServiceV1> eventV1OrchestrationServiceV1Mock;
        private readonly Mock<IEventV1ArchiveOrchestrationService> eventV1ArchiveOrchestrationServiceMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ICompareLogic compareLogic;
        private readonly IEventV1CoordinationServiceV1 eventV1CoordinationServiceV1;

        public EventV1CoordinationServiceV1Tests()
        {
            this.eventV1OrchestrationServiceV1Mock =
                new Mock<IEventV1OrchestrationServiceV1>(
                    behavior: MockBehavior.Strict);

            this.eventV1ArchiveOrchestrationServiceMock =
                new Mock<IEventV1ArchiveOrchestrationService>(
                    behavior: MockBehavior.Strict);

            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>(
                behavior: MockBehavior.Strict);

            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            var compareConfiguration = new ComparisonConfig();
            this.compareLogic = new CompareLogic(compareConfiguration);

            this.eventV1CoordinationServiceV1 =
                new EventV1CoordinationServiceV1(
                    eventV1OrchestrationServiceV1: this.eventV1OrchestrationServiceV1Mock.Object,
                    eventV1ArchiveOrchestrationService: this.eventV1ArchiveOrchestrationServiceMock.Object,
                    dateTimeBroker: this.dateTimeBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> EventV1ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventV1OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new EventV1OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException)
            };
        }

        public static TheoryData<Xeption> EventV1DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventV1OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new EventV1OrchestrationServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> EventV1ArchiveValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventV1ArchiveOrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new EventV1ArchiveOrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException)
            };
        }

        public static TheoryData<Xeption> EventV1ArchiveDependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventV1ArchiveOrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new EventV1ArchiveOrchestrationServiceException(
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

        private Expression<Func<EventV1, bool>> SameEventV1As(
           EventV1 expectedEventV1)
        {
            return actualEventV1 =>
                this.compareLogic.Compare(
                    expectedEventV1,
                    actualEventV1)
                        .AreEqual;
        }

        private Expression<Func<EventV1Archive, bool>> SameEventV1ArchiveAs(
           EventV1Archive expectedEventV1Archive)
        {
            return actualEventV1Archive =>
                this.compareLogic.Compare(
                    expectedEventV1Archive,
                    actualEventV1Archive)
                        .AreEqual;
        }

        private static List<dynamic> CreateRandomEventV1sProperties()
        {
            int randomCount = GetRandomNumber();

            return Enumerable.Range(start: 0, count: randomCount)
                .Select(item => CreateRandomEventV1Properties())
                    .ToList();
        }

        private static List<dynamic> CreateRandomListenerEventV1sProperties()
        {
            int randomCount = GetRandomNumber();

            return Enumerable.Range(start: 0, count: randomCount)
                .Select(item => CreateRandomListenerEventV1Properties())
                    .ToList();
        }

        private static dynamic CreateRandomEventV1Properties()
        {
            Guid randomId = GetRandomId();
            string randomContent = GetRandomString();

            EventV1Type randomType =
                GetRandomEnum<EventV1Type>();

            DateTimeOffset randomCreatedDate =
                GetRandomDateTimeOffset();

            DateTimeOffset randomUpdatedDate =
                GetRandomDateTimeOffset();

            DateTimeOffset randomScheduledDate =
                GetRandomDateTimeOffset();

            Guid randomEventAddressId = GetRandomId();

            return new
            {
                Id = randomId,
                Content = randomContent,
                Type = randomType,
                CreatedDate = randomCreatedDate,
                UpdatedDate = randomUpdatedDate,
                ScheduledDate = randomScheduledDate,
                EventAddressId = randomEventAddressId
            };
        }

        private static dynamic CreateRandomListenerEventV1Properties()
        {
            Guid randomId = GetRandomId();

            ListenerEventV1Status randomStatus =
                GetRandomEnum<ListenerEventV1Status>();

            string randomResponse = GetRandomString();

            DateTimeOffset randomCreatedDate =
                GetRandomDateTimeOffset();

            DateTimeOffset randomUpdatedDate =
                GetRandomDateTimeOffset();

            DateTimeOffset randomScheduledDate =
                GetRandomDateTimeOffset();

            Guid randomEventId = GetRandomId();
            Guid randomEventAddressId = GetRandomId();
            Guid randomEventListenerId = GetRandomId();

            return new
            {
                Id = randomId,
                Status = randomStatus,
                Response = randomResponse,
                CreatedDate = randomCreatedDate,
                UpdatedDate = randomUpdatedDate,
                EventId = randomEventId,
                EventAddressId = randomEventAddressId,
                EventListenerId = randomEventListenerId
            };
        }

        private static IQueryable<EventV1> CreateRandomEventV1s()
        {
            return CreateEventV1Filler()
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Filler<EventV1> CreateEventV1Filler()
        {
            var filler = new Filler<EventV1>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset())

                .OnProperty(eventV1 =>
                    eventV1.EventAddress).IgnoreIt()

                .OnProperty(eventV1 =>
                    eventV1.ListenerEvents).IgnoreIt();

            return filler;
        }
    }
}
