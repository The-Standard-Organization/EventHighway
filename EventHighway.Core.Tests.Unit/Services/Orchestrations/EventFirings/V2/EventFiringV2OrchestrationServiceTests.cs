// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Configurations.Retries;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Processings.EventCalls.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.EventFirings.V2;
using EventHighway.Core.Services.Processings.EventCalls.V2;
using EventHighway.Core.Services.Processings.EventListeners.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventFirings.V2
{
    public partial class EventFiringV2OrchestrationServiceTests
    {
        private readonly Mock<IEventListenerV2ProcessingService> eventListenerV2ProcessingServiceMock;
        private readonly Mock<IListenerEventV2ProcessingService> listenerEventV2ProcessingServiceMock;
        private readonly Mock<IEventCallV2ProcessingService> eventCallV2ProcessingServiceMock;
        private readonly Mock<IConfigurationBroker> configurationBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ICompareLogic compareLogic;
        private readonly RetryConfiguration retryConfiguration;
        private readonly IEventFiringV2OrchestrationService eventFiringV2OrchestrationService;

        public EventFiringV2OrchestrationServiceTests()
        {
            this.eventListenerV2ProcessingServiceMock =
                new Mock<IEventListenerV2ProcessingService>(
                    behavior: MockBehavior.Strict);

            this.listenerEventV2ProcessingServiceMock =
                new Mock<IListenerEventV2ProcessingService>(
                    behavior: MockBehavior.Strict);

            this.eventCallV2ProcessingServiceMock =
                new Mock<IEventCallV2ProcessingService>(
                    behavior: MockBehavior.Strict);

            this.configurationBrokerMock = new Mock<IConfigurationBroker>(
                behavior: MockBehavior.Strict);

            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>(
                behavior: MockBehavior.Strict);

            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            var compareConfiguration = new ComparisonConfig();

            compareConfiguration.IgnoreProperty<ListenerEventV2>(listenerEventV2 =>
                listenerEventV2.Id);

            this.compareLogic = new CompareLogic(compareConfiguration);

            this.retryConfiguration = CreateRandomRetryConfiguration();

            this.configurationBrokerMock.Setup(broker =>
                broker.GetRetryConfiguration())
                    .Returns(this.retryConfiguration);

            this.eventFiringV2OrchestrationService =
                new EventFiringV2OrchestrationService(
                    eventListenerV2ProcessingService: this.eventListenerV2ProcessingServiceMock.Object,
                    listenerEventV2ProcessingService: this.listenerEventV2ProcessingServiceMock.Object,
                    eventCallV2ProcessingService: this.eventCallV2ProcessingServiceMock.Object,
                    configurationBroker: this.configurationBrokerMock.Object,
                    dateTimeBroker: this.dateTimeBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new EventListenerV2ProcessingValidationException(
                    someMessage,
                    someInnerException),

                new EventListenerV2ProcessingDependencyValidationException(
                    someMessage,
                    someInnerException),

                new ListenerEventV2ProcessingValidationException(
                    someMessage,
                    someInnerException),

                new ListenerEventV2ProcessingDependencyValidationException(
                    someMessage,
                    someInnerException),

                new EventCallV2ProcessingValidationException(
                    someMessage,
                    someInnerException),

                new EventCallV2ProcessingDependencyValidationException(
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
                new EventListenerV2ProcessingDependencyException(
                    someMessage,
                    someInnerException),

                new EventListenerV2ProcessingServiceException(
                    someMessage,
                    someInnerException),

                new ListenerEventV2ProcessingDependencyException(
                    someMessage,
                    someInnerException),

                new ListenerEventV2ProcessingServiceException(
                    someMessage,
                    someInnerException),

                new EventCallV2ProcessingDependencyException(
                    someMessage,
                    someInnerException),

                new EventCallV2ProcessingServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static RetryConfiguration CreateRandomRetryConfiguration()
        {
            return new RetryConfiguration
            {
                RetryAttemptsAllowed = GetRandomNumber(),
                RetryBackoffMaxMinutes = GetRandomNumber(),
                DeadAfterMinutes = GetRandomNumber()
            };
        }

        private static EventV2 CreateRandomEventV2() =>
            CreateEventV2Filler().Create();

        private static IQueryable<EventListenerV2> CreateRandomEventListenerV2s() =>
            CreateEventListenerV2Filler().Create(count: GetRandomNumber())
                .Select(l => { l.PromotedProperties = null; l.FilterCriteria = null; return l; })
                    .AsQueryable();

        private static IQueryable<EventListenerV2> CreateRandomEventListenerV2s(int count) =>
            CreateEventListenerV2Filler().Create(count)
                .Select(l => { l.PromotedProperties = null; l.FilterCriteria = null; return l; })
                    .AsQueryable();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private Expression<Func<ListenerEventV2, bool>> SameListenerEventAs(
           ListenerEventV2 expectedListenerEventV2)
        {
            return actualListenerEventV2 =>
                this.compareLogic.Compare(
                    expectedListenerEventV2,
                    actualListenerEventV2)
                        .AreEqual;
        }

        private Expression<Func<EventCallV2, bool>> SameEventCallAs(
           EventCallV2 expectedEventCallV2)
        {
            return actualEventCallV2 =>
                this.compareLogic.Compare(
                    expectedEventCallV2,
                    actualEventCallV2)
                        .AreEqual;
        }

        private static IEnumerable<string> SplitPromotedPropertyKeys(string promotedProperties) =>
            string.IsNullOrWhiteSpace(promotedProperties)
                ? Array.Empty<string>()
                : promotedProperties.Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        private static Filler<EventV2> CreateEventV2Filler()
        {
            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset())

                .OnProperty(eventV2 =>
                    eventV2.EventAddressV2).IgnoreIt()

                .OnProperty(eventV2 =>
                    eventV2.ListenerEventV2s).IgnoreIt()

                .OnType<EventAddressV2>().IgnoreIt()
                .OnType<ListenerEventV2>().IgnoreIt()
                .OnType<EventParticipantV2>().IgnoreIt();

            return filler;
        }

        private static Filler<EventListenerV2> CreateEventListenerV2Filler()
        {
            var filler = new Filler<EventListenerV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)

                .OnProperty(eventListenerV2 =>
                    eventListenerV2.EventAddressV2).IgnoreIt()

                .OnProperty(eventListenerV2 =>
                    eventListenerV2.ListenerEventV2s).IgnoreIt()

                .OnProperty(eventListenerV2 =>
                    eventListenerV2.ListenerEventArchiveV2s).IgnoreIt()

                .OnProperty(eventListenerV2 =>
                    eventListenerV2.EventParticipantV2).IgnoreIt()

                .OnType<EventAddressV2>().IgnoreIt()
                .OnType<EventV2>().IgnoreIt();

            return filler;
        }
    }
}
