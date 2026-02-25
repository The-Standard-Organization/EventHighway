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
    public partial class EventV1ArchiveProcessingServiceTests
    {
        private readonly Mock<IEventV1ArchiveService> eventV1ArchiveServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventV1ArchiveProcessingService eventV1ArchiveProcessingService;

        public EventV1ArchiveProcessingServiceTests()
        {
            this.eventV1ArchiveServiceMock = new Mock<IEventV1ArchiveService>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventV1ArchiveProcessingService =
                new EventV1ArchiveProcessingService(
                    eventV1ArchiveService: this.eventV1ArchiveServiceMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventV1ArchiveValidationException(
                    someMessage,
                    someInnerException),

                new EventV1ArchiveDependencyValidationException(
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
                new EventV1ArchiveDependencyException(
                    someMessage,
                    someInnerException),

                new EventV1ArchiveServiceException(
                    someMessage,
                    someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static IQueryable<EventV1Archive> CreateRandomEventV1Archives() =>
            CreateEventV1ArchiveFiller().Create(count: GetRandomNumber()).AsQueryable();

        private static EventV1Archive CreateRandomEventV1Archive() =>
            CreateEventV1ArchiveFiller().Create();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Filler<EventV1Archive> CreateEventV1ArchiveFiller()
        {
            var filler = new Filler<EventV1Archive>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnType<DateTimeOffset?>().Use(GetRandomDateTimeOffset());

            return filler;
        }
    }
}
