// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1.Exceptions;
using EventHighway.Core.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Services.Processings.EventAddresses.V1;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventAddresses.V1
{
    public partial class EventAddressV1ProcessingServiceTests
    {
        private readonly Mock<IEventAddressV1Service> eventAddressV1ServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventAddressV1ProcessingService eventAddressV1ProcessingService;

        public EventAddressV1ProcessingServiceTests()
        {
            this.eventAddressV1ServiceMock =
                new Mock<IEventAddressV1Service>();

            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventAddressV1ProcessingService =
                new EventAddressV1ProcessingService(
                    eventAddressV1Service: this.eventAddressV1ServiceMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventAddressV1ValidationException(
                    someMessage,
                    someInnerException),

                new EventAddressV1DependencyValidationException(
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
                new EventAddressV1DependencyException(
                    someMessage,
                    someInnerException),

                new EventAddressV1ServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static EventAddressV1 CreateRandomEventAddressV1() =>
            CreateEventAddressV1Filler().Create();

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
    }
}
