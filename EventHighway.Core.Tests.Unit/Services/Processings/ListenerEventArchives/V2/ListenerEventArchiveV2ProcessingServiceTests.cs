// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Services.Processings.ListenerEventArchives.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ProcessingServiceTests
    {
        private readonly Mock<IListenerEventArchiveV2Service> listenerEventArchiveV2ServiceMock;
        private readonly Mock<IConfigurationBroker> configurationBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IListenerEventArchiveV2ProcessingService listenerEventArchiveV2ProcessingService;

        public ListenerEventArchiveV2ProcessingServiceTests()
        {
            this.listenerEventArchiveV2ServiceMock =
                new Mock<IListenerEventArchiveV2Service>();

            this.loggingBrokerMock =
                new Mock<ILoggingBroker>();

            this.configurationBrokerMock =
                new Mock<IConfigurationBroker>();

            this.listenerEventArchiveV2ProcessingService = new ListenerEventArchiveV2ProcessingService(
                listenerEventArchiveV2Service: this.listenerEventArchiveV2ServiceMock.Object,
                configurationBroker: this.configurationBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new ListenerEventArchiveV2ValidationException(
                    someMessage,
                    someInnerException),

                new ListenerEventArchiveV2DependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new ListenerEventArchiveV2DependencyException(
                    someMessage,
                    someInnerException),

                new ListenerEventArchiveV2ServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static IQueryable<ListenerEventArchiveV2> CreateRandomListenerEventArchiveV2s() =>
            CreateListenerEventArchiveV2Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static ListenerEventArchiveV2 CreateRandomListenerEventArchiveV2() =>
            CreateListenerEventArchiveV2Filler().Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Filler<ListenerEventArchiveV2> CreateListenerEventArchiveV2Filler()
        {
            var filler = new Filler<ListenerEventArchiveV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset);

            return filler;
        }
    }
}
