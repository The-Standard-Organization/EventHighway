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
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2;
using EventHighway.Core.Services.Processings.Events.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2OrchestrationServiceTests
    {
        private readonly Mock<IEventV2ProcessingService> eventV2ProcessingServiceMock;
        private readonly Mock<IListenerEventV2ProcessingService> listenerEventV2ProcessingServiceMock;
        private readonly Mock<IConfigurationBroker> configurationBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IArchivingEventV2OrchestrationService archivingEventV2OrchestrationService;

        public ArchivingEventV2OrchestrationServiceTests()
        {
            this.eventV2ProcessingServiceMock = new Mock<IEventV2ProcessingService>();
            this.listenerEventV2ProcessingServiceMock = new Mock<IListenerEventV2ProcessingService>();
            this.configurationBrokerMock = new Mock<IConfigurationBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.archivingEventV2OrchestrationService =
                new ArchivingEventV2OrchestrationService(
                    eventV2ProcessingService: this.eventV2ProcessingServiceMock.Object,
                    listenerEventV2ProcessingService: this.listenerEventV2ProcessingServiceMock.Object,
                    configurationBroker: this.configurationBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new EventV2ProcessingValidationException(someMessage, someInnerException),
                new EventV2ProcessingDependencyValidationException(someMessage, someInnerException),
                new ListenerEventV2ProcessingValidationException(someMessage, someInnerException),
                new ListenerEventV2ProcessingDependencyValidationException(someMessage, someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new EventV2ProcessingDependencyException(someMessage, someInnerException),
                new EventV2ProcessingServiceException(someMessage, someInnerException),
                new ListenerEventV2ProcessingDependencyException(someMessage, someInnerException),
                new ListenerEventV2ProcessingServiceException(someMessage, someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static EventV2 CreateRandomEventV2() =>
            CreateEventV2Filler().Create();

        private static IQueryable<EventV2> CreateRandomEventV2s() =>
            CreateEventV2Filler()
                .Create(count: GetRandomNumber())
                    .AsQueryable();

        private static List<EventV2> CreateRandomEventV2List() =>
            CreateEventV2Filler()
                .Create(count: GetRandomNumber())
                    .ToList();

        private static BatchConfiguration CreateRandomBatchConfiguration() =>
            new BatchConfiguration
            {
                BatchSizeForBulkProcessing = GetRandomNumber()
            };

        private static IEnumerable<ListenerEventV2> CreateRandomListenerEventV2s() =>
            CreateListenerEventV2Filler()
                .Create(count: GetRandomNumber())
                    .ToList();

        private static Filler<EventV2> CreateEventV2Filler()
        {
            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset())

                .OnProperty(eventV2 => eventV2.EventAddressV2)
                    .IgnoreIt()

                .OnType<EventAddressV2>().IgnoreIt()
                .OnType<EventListenerV2>().IgnoreIt();

            return filler;
        }

        private static Filler<ListenerEventV2> CreateListenerEventV2Filler()
        {
            var filler = new Filler<ListenerEventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnProperty(listenerEventV2 => listenerEventV2.Event)
                    .IgnoreIt()

                .OnProperty(listenerEventV2 => listenerEventV2.EventAddress)
                    .IgnoreIt()

                .OnProperty(listenerEventV2 => listenerEventV2.EventListener)
                    .IgnoreIt();

            return filler;
        }
    }
}
