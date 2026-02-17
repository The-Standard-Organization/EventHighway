// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Services.Processings.ListenerEventArchives.V1;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V1
{
    public partial class ListenerEventV1ArchiveProcessingServiceTests
    {
        private readonly Mock<IListenerEventV1ArchiveService> listenerEventV1ArchiveServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IListenerEventV1ArchiveProcessingService listenerEventV1ArchiveProcessingService;

        public ListenerEventV1ArchiveProcessingServiceTests()
        {
            this.listenerEventV1ArchiveServiceMock =
                new Mock<IListenerEventV1ArchiveService>();

            this.loggingBrokerMock =
                new Mock<ILoggingBroker>();

            this.listenerEventV1ArchiveProcessingService = new ListenerEventV1ArchiveProcessingService(
                listenerEventV1ArchiveService: this.listenerEventV1ArchiveServiceMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new ListenerEventV1ArchiveValidationException(
                    someMessage,
                    someInnerException),

                new ListenerEventV1ArchiveDependencyValidationException(
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
                new ListenerEventV1ArchiveDependencyException(
                    someMessage,
                    someInnerException),

                new ListenerEventV1ArchiveServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNegativeNumber() =>
            -1 * GetRandomNumber();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static ListenerEventV1Archive CreateRandomListenerEventV1Archive() =>
            CreateListenerEventV1ArchiveFiller().Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Filler<ListenerEventV1Archive> CreateListenerEventV1ArchiveFiller()
        {
            var filler = new Filler<ListenerEventV1Archive>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset);

            return filler;
        }
    }
}
