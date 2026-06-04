// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventCalls.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.Events.V1.Exceptions;
using EventHighway.Core.Services.Orchestrations.Events.V1;
using EventHighway.Core.Services.Processings.EventAddresses.V1;
using EventHighway.Core.Services.Processings.EventCalls.V1;
using EventHighway.Core.Services.Processings.Events.V1;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V1
{
    public partial class EventV1OrchestrationServiceTests
    {
        private readonly Mock<IEventV1ProcessingService> eventV1ProcessingServiceMock;
        private readonly Mock<IEventAddressV1ProcessingService> eventAddressV1ProcessingServiceMock;
        private readonly Mock<IEventCallV1ProcessingService> eventCallV1ProcessingServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventV1OrchestrationService eventV1OrchestrationService;

        public EventV1OrchestrationServiceTests()
        {
            this.eventV1ProcessingServiceMock =
                new Mock<IEventV1ProcessingService>();

            this.eventAddressV1ProcessingServiceMock =
                new Mock<IEventAddressV1ProcessingService>();

            this.eventCallV1ProcessingServiceMock =
                new Mock<IEventCallV1ProcessingService>();

            this.loggingBrokerMock =
                new Mock<ILoggingBroker>();

            this.eventV1OrchestrationService =
                new EventV1OrchestrationService(
                    eventV1ProcessingService: this.eventV1ProcessingServiceMock.Object,
                    eventAddressV1ProcessingService: this.eventAddressV1ProcessingServiceMock.Object,
                    eventCallV1ProcessingService: this.eventCallV1ProcessingServiceMock.Object,
                    loggingBroker: loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> EventCallV1ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventCallV1ProcessingValidationException(
                    someMessage,
                    someInnerException),

                new EventCallV1ProcessingDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> EventCallV1DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventCallV1ProcessingDependencyException(
                    someMessage,
                    someInnerException),

                new EventCallV1ProcessingServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> EventAddressV1ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventAddressV1ProcessingValidationException(
                    someMessage,
                    someInnerException),

                new EventAddressV1ProcessingDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> EventAddressV1DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventAddressV1ProcessingDependencyException(
                    someMessage,
                    someInnerException),

                new EventAddressV1ProcessingServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> EventV1ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventV1ProcessingValidationException(
                    someMessage,
                    someInnerException),

                new EventV1ProcessingDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> EventV1DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventV1ProcessingDependencyException(
                    someMessage,
                    someInnerException),

                new EventV1ProcessingServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset()
        {
            return new DateTimeRange(
                earliestDate: DateTime.UnixEpoch)
                    .GetValue();
        }

        private static EventCallV1 CreateRandomEventCallV1() =>
            CreateEventCallV1Filler().Create();

        private static IQueryable<EventV1> CreateRandomEventV1s()
        {
            return CreateEventV1Filler()
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static EventAddressV1 CreateRandomEventAddressV1() =>
            CreateEventAddressV1Filler().Create();

        private static EventV1 CreateRandomEventV1() =>
            CreateEventV1Filler().Create();

        private static Filler<EventCallV1> CreateEventCallV1Filler() =>
            new Filler<EventCallV1>();

        private static Filler<EventAddressV1> CreateEventAddressV1Filler()
        {
            var filler = new Filler<EventAddressV1>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)

                .OnProperty(eventAddressV1 => eventAddressV1.Events)
                    .IgnoreIt()

                .OnProperty(eventAddressV1 => eventAddressV1.EventListenerV1s)
                    .IgnoreIt()

                .OnProperty(eventAddressV1 => eventAddressV1.ListenerEventV1s)
                    .IgnoreIt();

            return filler;
        }

        private static Filler<EventV1> CreateEventV1Filler()
        {
            var filler = new Filler<EventV1>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset());

            return filler;
        }
    }
}
