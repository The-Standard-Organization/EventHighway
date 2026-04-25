// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1.Exceptions;
using EventHighway.Core.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Services.Processings.ListenerEvents.V1;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V1
{
    public partial class ListenerEventV1ProcessingServiceTests
    {
        private readonly Mock<IListenerEventV1Service> listenerEventV1ServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IListenerEventV1ProcessingService listenerEventV1ProcessingService;

        public ListenerEventV1ProcessingServiceTests()
        {
            this.listenerEventV1ServiceMock = new Mock<IListenerEventV1Service>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.listenerEventV1ProcessingService = new ListenerEventV1ProcessingService(
                listenerEventV1Service: this.listenerEventV1ServiceMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> ListenerEventV1ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new ListenerEventV1ValidationException(
                    someMessage,
                    someInnerException),

                new ListenerEventV1DependencyValidationException(
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
                new ListenerEventV1DependencyException(
                    someMessage,
                    someInnerException),

                new ListenerEventV1ServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static IQueryable<ListenerEventV1> CreateRandomListenerEventV1s() =>
            CreateListenerEventV1Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static ListenerEventV1 CreateRandomListenerEventV1() =>
            CreateListenerEventV1Filler().Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

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
    }
}
