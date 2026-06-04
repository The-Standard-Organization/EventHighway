// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Processings.EventListeners.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V1.Exceptions;
using EventHighway.Core.Services.Orchestrations.EventListeners.V1;
using EventHighway.Core.Services.Processings.EventListeners.V1;
using EventHighway.Core.Services.Processings.ListenerEvents.V1;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V1
{
    public partial class EventListenerV1OrchestrationServiceTests
    {
        private readonly Mock<IEventListenerV1ProcessingService> eventListenerV1ProcessingServiceMock;
        private readonly Mock<IListenerEventV1ProcessingService> listenerEventV1ProcessingServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventListenerV1OrchestrationService eventListenerV1OrchestrationService;

        public EventListenerV1OrchestrationServiceTests()
        {
            this.eventListenerV1ProcessingServiceMock =
                new Mock<IEventListenerV1ProcessingService>();

            this.listenerEventV1ProcessingServiceMock =
                new Mock<IListenerEventV1ProcessingService>();

            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventListenerV1OrchestrationService =
                new EventListenerV1OrchestrationService(
                    eventListenerV1ProcessingService: this.eventListenerV1ProcessingServiceMock.Object,
                    listenerEventV1ProcessingService: this.listenerEventV1ProcessingServiceMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> EventListenerV1ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventListenerV1ProcessingValidationException(
                    someMessage,
                    someInnerException),

                new EventListenerV1ProcessingDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> EventListenerV1DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventListenerV1ProcessingDependencyException(
                    someMessage,
                    someInnerException),

                new EventListenerV1ProcessingServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> ListenerEventV1ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new ListenerEventV1ProcessingValidationException(
                    someMessage,
                    someInnerException),

                new ListenerEventV1ProcessingDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> ListenerEventV1DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new ListenerEventV1ProcessingDependencyException(
                    someMessage,
                    someInnerException),

                new ListenerEventV1ProcessingServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static IQueryable<ListenerEventV1> CreateRandomListenerEventV1s() =>
            CreateListenerEventV1Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static ListenerEventV1 CreateRandomListenerEventV1() =>
            CreateListenerEventV1Filler().Create();

        private static IQueryable<EventListenerV1> CreateRandomEventListenerV1s() =>
            CreateEventListenerV1Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static EventListenerV1 CreateRandomEventListenerV1() =>
            CreateEventListenerV1Filler().Create();

        private static Filler<ListenerEventV1> CreateListenerEventV1Filler()
        {
            var filler = new Filler<ListenerEventV1>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)

                .OnProperty(listenerEventV1 => listenerEventV1.Event)
                    .IgnoreIt()

                .OnProperty(listenerEventV1 => listenerEventV1.EventAddress)
                    .IgnoreIt()

                .OnProperty(listenerEventV1 => listenerEventV1.EventListener)
                    .IgnoreIt();

            return filler;
        }

        private static Filler<EventListenerV1> CreateEventListenerV1Filler()
        {
            var filler = new Filler<EventListenerV1>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)

                .OnProperty(eventListenerV1 => eventListenerV1.EventAddressV1s)
                    .IgnoreIt()

                .OnProperty(eventListenerV1 => eventListenerV1.ListenerEventV1s)
                    .IgnoreIt();

            return filler;
        }
    }
}
