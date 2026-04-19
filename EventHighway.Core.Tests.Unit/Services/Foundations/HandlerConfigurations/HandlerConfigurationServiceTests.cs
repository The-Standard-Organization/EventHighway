// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Services.Foundations.HandlerConfigurations;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xunit;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.HandlerConfigurations
{
    public partial class HandlerConfigurationServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IHandlerConfigurationService handlerConfigurationService;

        public HandlerConfigurationServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.handlerConfigurationService = new HandlerConfigurationService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static HandlerConfiguration CreateRandomHandlerConfiguration() =>
            CreateHandlerConfigurationFiller(GetRandomDateTimeOffset()).Create();

        private static HandlerConfiguration CreateRandomHandlerConfiguration(DateTimeOffset dates) =>
            CreateHandlerConfigurationFiller(dates).Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static string GetRandomStringWithLengthOf(int length) =>
            new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        public static TheoryData<int> MinutesBeforeAndAfterNow()
        {
            int randomMoreThanOneMinuteAhead = GetRandomNumber();
            int randomMoreThanOneMinuteAgo = -1 * GetRandomNumber();

            return new TheoryData<int>
            {
                randomMoreThanOneMinuteAhead,
                randomMoreThanOneMinuteAgo
            };
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static SqlException GetSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(type: typeof(SqlException));

        private static Filler<HandlerConfiguration> CreateHandlerConfigurationFiller(DateTimeOffset dates)
        {
            var filler = new Filler<HandlerConfiguration>();

            filler.Setup()
                .OnType<Guid>().Use(Guid.NewGuid)
                .OnType<DateTimeOffset>().Use(dates)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)GetRandomDateTimeOffset());

            return filler;
        }
    }
}
