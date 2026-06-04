// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;
using EventHighway.Core.Services.Foundations.EventArchives.V1;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Services.Orchestrations.EventArchives.V1;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V1
{
    public partial class EventArchiveV1OrchestrationServiceTests
    {
        private readonly Mock<IListenerEventArchiveV1Service> listenerEventArchiveV1ServiceMock;
        private readonly Mock<IEventArchiveV1Service> eventArchiveV1ServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventArchiveV1OrchestrationService eventArchiveV1OrchestrationService;

        public EventArchiveV1OrchestrationServiceTests()
        {
            this.listenerEventArchiveV1ServiceMock = new Mock<IListenerEventArchiveV1Service>();
            this.eventArchiveV1ServiceMock = new Mock<IEventArchiveV1Service>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventArchiveV1OrchestrationService = new EventArchiveV1OrchestrationService(
                listenerEventArchiveV1Service: this.listenerEventArchiveV1ServiceMock.Object,
                eventArchiveV1Service: this.eventArchiveV1ServiceMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> EventArchiveV1ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventArchiveV1ValidationException(
                    someMessage,
                    someInnerException),

                new EventArchiveV1DependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> EventArchiveV1DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventArchiveV1DependencyException(
                    someMessage,
                    someInnerException),

                new EventArchiveV1ServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> ListenerEventArchiveV1ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new ListenerEventArchiveV1ValidationException(
                    someMessage,
                    someInnerException),

                new ListenerEventArchiveV1DependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> ListenerEventArchiveV1DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new ListenerEventArchiveV1DependencyException(
                    someMessage,
                    someInnerException),

                new ListenerEventArchiveV1ServiceException(
                    someMessage,
                    someInnerException,
                    data: someInnerException.Data),
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static EventArchiveV1 CreateRandomEventArchiveV1() =>
            CreateEventArchiveV1Filler().Create();

        private static Filler<EventArchiveV1> CreateEventArchiveV1Filler()
        {
            var filler = new Filler<EventArchiveV1>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset());

            return filler;
        }
    }
}
