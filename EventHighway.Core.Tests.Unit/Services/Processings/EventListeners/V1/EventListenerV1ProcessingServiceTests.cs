// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1.Exceptions;
using EventHighway.Core.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Services.Processings.EventListeners.V1;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventListeners.V1
{
    public partial class EventListenerV1ProcessingServiceTests
    {
        private readonly Mock<IEventListenerV1Service> eventListenerV1ServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventListenerV1ProcessingService eventListenerV1ProcessingService;

        public EventListenerV1ProcessingServiceTests()
        {
            this.eventListenerV1ServiceMock =
                new Mock<IEventListenerV1Service>();

            this.loggingBrokerMock =
                new Mock<ILoggingBroker>();

            this.eventListenerV1ProcessingService =
                new EventListenerV1ProcessingService(
                    eventListenerV1Service: eventListenerV1ServiceMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventListenerV1ValidationException(
                    someMessage,
                    someInnerException),

                new EventListenerV1DependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventListenerV1DependencyException(
                    someMessage,
                    someInnerException),

                new EventListenerV1ServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static IQueryable<EventListenerV1> CreateRandomEventListenerV1s() =>
            CreateEventListenerV1Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static EventListenerV1 CreateRandomEventListenerV1() =>
            CreateEventListenerV1Filler().Create();

        private static IQueryable<EventListenerV1> CreateRandomEventListenerV1s(
            Guid eventAddressId)
        {
            IQueryable<EventListenerV1> randomEventListenerV1s =
                CreateRandomEventListenerV1s();

            randomEventListenerV1s.ToList().ForEach(listener =>
            {
                listener.EventAddressId = eventAddressId;
            });

            return randomEventListenerV1s;
        }

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

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
