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
    public partial class ListenerEventArchiveV1ProcessingServiceTests
    {
        private readonly Mock<IListenerEventArchiveV1Service> listenerEventArchiveV1ServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IListenerEventArchiveV1ProcessingService listenerEventArchiveV1ProcessingService;

        public ListenerEventArchiveV1ProcessingServiceTests()
        {
            this.listenerEventArchiveV1ServiceMock =
                new Mock<IListenerEventArchiveV1Service>();

            this.loggingBrokerMock =
                new Mock<ILoggingBroker>();

            this.listenerEventArchiveV1ProcessingService = new ListenerEventArchiveV1ProcessingService(
                listenerEventArchiveV1Service: this.listenerEventArchiveV1ServiceMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
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

        public static TheoryData<Xeption> DependencyExceptions()
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
                    someInnerException)
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static ListenerEventArchiveV1 CreateRandomListenerEventArchiveV1() =>
            CreateListenerEventArchiveV1Filler().Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Filler<ListenerEventArchiveV1> CreateListenerEventArchiveV1Filler()
        {
            var filler = new Filler<ListenerEventArchiveV1>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset);

            return filler;
        }
    }
}
