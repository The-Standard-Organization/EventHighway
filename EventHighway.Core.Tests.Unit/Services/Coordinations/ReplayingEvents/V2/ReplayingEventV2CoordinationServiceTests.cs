// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.ReplayingListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.RestoringEvents.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.ReplayingEvents.V2;
using EventHighway.Core.Services.Orchestrations.EventArchives.V2;
using EventHighway.Core.Services.Orchestrations.ReplayingListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.RestoringEvents.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ReplayingEvents.V2
{
    public partial class ReplayingEventV2CoordinationServiceTests
    {
        private readonly Mock<IEventArchiveV2OrchestrationService> eventArchiveV2OrchestrationServiceMock;
        private readonly Mock<IRestoringEventV2OrchestrationService> restoringEventV2OrchestrationServiceMock;
        private readonly Mock<IReplayingListenerEventV2OrchestrationService> replayingListenerEventV2OrchestrationServiceMock;
        private readonly Mock<IConfigurationBroker> configurationBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IReplayingEventV2CoordinationService replayingEventV2CoordinationService;

        public ReplayingEventV2CoordinationServiceTests()
        {
            this.eventArchiveV2OrchestrationServiceMock =
                new Mock<IEventArchiveV2OrchestrationService>();

            this.restoringEventV2OrchestrationServiceMock =
                new Mock<IRestoringEventV2OrchestrationService>();

            this.replayingListenerEventV2OrchestrationServiceMock =
                new Mock<IReplayingListenerEventV2OrchestrationService>();

            this.configurationBrokerMock = new Mock<IConfigurationBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.replayingEventV2CoordinationService = new ReplayingEventV2CoordinationService(
                eventArchiveV2OrchestrationService: this.eventArchiveV2OrchestrationServiceMock.Object,
                restoringEventV2OrchestrationService: this.restoringEventV2OrchestrationServiceMock.Object,
                replayingListenerEventV2OrchestrationService: this.replayingListenerEventV2OrchestrationServiceMock.Object,
                configurationBroker: this.configurationBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);

            return new TheoryData<Xeption>
            {
                new EventArchiveV2OrchestrationValidationException(someMessage, someInnerException),
                new EventArchiveV2OrchestrationDependencyValidationException(someMessage, someInnerException),
                new RestoringEventV2OrchestrationValidationException(someMessage, someInnerException),
                new RestoringEventV2OrchestrationDependencyValidationException(someMessage, someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);

            return new TheoryData<Xeption>
            {
                new EventArchiveV2OrchestrationDependencyException(someMessage, someInnerException),
                new EventArchiveV2OrchestrationServiceException(someMessage, someInnerException),
                new RestoringEventV2OrchestrationDependencyException(someMessage, someInnerException),
                new RestoringEventV2OrchestrationServiceException(someMessage, someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static bool SameGuidSetAs(
            IEnumerable<Guid> expectedGuids,
            IEnumerable<Guid> actualGuids) =>
            expectedGuids.OrderBy(guid => guid).SequenceEqual(actualGuids.OrderBy(guid => guid));

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static BatchConfiguration CreateBatchConfiguration(int batchSize) =>
            new BatchConfiguration { BatchSizeForBulkProcessing = batchSize };

        private static List<EventArchiveV2> CreateRandomEventArchiveV2s() =>
            CreateEventArchiveV2Filler().Create(count: GetRandomNumber()).ToList();

        private static List<ListenerEventArchiveV2> CreateRandomListenerEventArchiveV2s() =>
            CreateListenerEventArchiveV2Filler().Create(count: GetRandomNumber()).ToList();

        private static List<ListenerEventArchiveV2> CreateRandomListenerEventArchiveV2s(int count) =>
            CreateListenerEventArchiveV2Filler().Create(count: count).ToList();

        private static Filler<EventArchiveV2> CreateEventArchiveV2Filler()
        {
            var filler = new Filler<EventArchiveV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnType<DateTimeOffset?>().Use(GetRandomDateTimeOffset())

                .OnProperty(eventArchiveV2 => eventArchiveV2.ListenerEventArchiveV2s)
                    .IgnoreIt()

                .OnProperty(eventArchiveV2 => eventArchiveV2.EventAddressV2)
                    .IgnoreIt()

                .OnProperty(eventArchiveV2 => eventArchiveV2.EventParticipantV2)
                    .IgnoreIt();

            return filler;
        }

        private static Filler<ListenerEventArchiveV2> CreateListenerEventArchiveV2Filler()
        {
            var filler = new Filler<ListenerEventArchiveV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnType<DateTimeOffset?>().Use(GetRandomDateTimeOffset())

                .OnProperty(listenerEventArchiveV2 => listenerEventArchiveV2.EventListenerV2)
                    .IgnoreIt()

                .OnType<EventParticipantV2>().IgnoreIt();

            return filler;
        }

        public static TheoryData<Xeption> DependencyValidationExceptionsForProcessReplayed()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);

            return new TheoryData<Xeption>
            {
                new ReplayingListenerEventV2OrchestrationValidationException(someMessage, someInnerException),
                new ReplayingListenerEventV2OrchestrationDependencyValidationException(someMessage, someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptionsForProcessReplayed()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);

            return new TheoryData<Xeption>
            {
                new ReplayingListenerEventV2OrchestrationDependencyException(someMessage, someInnerException),
                new ReplayingListenerEventV2OrchestrationServiceException(someMessage, someInnerException),
            };
        }

        private static List<ListenerEventV2> CreateRandomListenerEventV2s(int count) =>
            CreateListenerEventV2Filler().Create(count: count).ToList();

        private static Filler<ListenerEventV2> CreateListenerEventV2Filler()
        {
            var filler = new Filler<ListenerEventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnType<DateTimeOffset?>().Use(GetRandomDateTimeOffset())
                .OnType<EventParticipantV2>().IgnoreIt()
                .OnProperty(lev => lev.EventV2).IgnoreIt()
                .OnProperty(lev => lev.EventAddressV2).IgnoreIt()
                .OnProperty(lev => lev.EventListenerV2).IgnoreIt();

            return filler;
        }
    }
}
