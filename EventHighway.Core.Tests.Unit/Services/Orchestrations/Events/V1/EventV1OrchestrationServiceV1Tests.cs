// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Processings.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V1.Exceptions;
using EventHighway.Core.Services.Orchestrations.Events.V1;
using EventHighway.Core.Services.Processings.Events.V1;
using EventHighway.Core.Services.Processings.ListenerEvents.V1;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V1
{
    public partial class EventV1OrchestrationServiceV1Tests
    {
        private readonly Mock<IEventV1ProcessingService> eventV1ProcessingServiceMock;
        private readonly Mock<IListenerEventV1ProcessingService> listenerEventV1ProcessingServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventV1OrchestrationServiceV1 eventV1OrchestrationServiceV1;

        public EventV1OrchestrationServiceV1Tests()
        {
            this.eventV1ProcessingServiceMock =
                new Mock<IEventV1ProcessingService>();

            this.listenerEventV1ProcessingServiceMock =
                new Mock<IListenerEventV1ProcessingService>();

            this.loggingBrokerMock =
                new Mock<ILoggingBroker>();

            this.eventV1OrchestrationServiceV1 =
                new EventV1OrchestrationServiceV1(
                    eventV1ProcessingService: this.eventV1ProcessingServiceMock.Object,
                    listenerEventV1ProcessingService: this.listenerEventV1ProcessingServiceMock.Object,
                    loggingBroker: loggingBrokerMock.Object);
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
                    someInnerException)
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
                    someInnerException)
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

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

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

        private static EventV1 CreateRandomEventV1() =>
            CreateEventV1Filler().Create();

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
                    .Use(GetRandomDateTimeOffset());

            return filler;
        }
    }
}
