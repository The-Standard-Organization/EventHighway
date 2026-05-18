// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using EventHighway.Core.Services.Foundations.EventArchives.V1;
using EventHighway.Core.Services.Processings.EventArchives.V1;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V1
{
    public partial class EventArchiveV1ProcessingServiceTests
    {
        private readonly Mock<IEventArchiveV1Service> eventArchiveV1ServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventArchiveV1ProcessingService eventArchiveV1ProcessingService;

        public EventArchiveV1ProcessingServiceTests()
        {
            this.eventArchiveV1ServiceMock = new Mock<IEventArchiveV1Service>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventArchiveV1ProcessingService =
                new EventArchiveV1ProcessingService(
                    eventArchiveV1Service: this.eventArchiveV1ServiceMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
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
                    someInnerException)
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
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
                    someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static IQueryable<EventArchiveV1> CreateRandomEventArchiveV1s() =>
            CreateEventArchiveV1Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static EventArchiveV1 CreateRandomEventArchiveV1() =>
            CreateEventArchiveV1Filler().Create();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Filler<EventArchiveV1> CreateEventArchiveV1Filler()
        {
            var filler = new Filler<EventArchiveV1>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnType<DateTimeOffset?>().Use(GetRandomDateTimeOffset());

            return filler;
        }
    }
}
