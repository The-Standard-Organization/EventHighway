// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V1;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V1
{
    public partial class ListenerEventArchiveV1ServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IListenerEventArchiveV1Service listenerEventArchiveV1Service;

        public ListenerEventArchiveV1ServiceTests()
        {
            this.storageBrokerMock =
                new Mock<IStorageBroker>();

            this.loggingBrokerMock =
                new Mock<ILoggingBroker>();

            this.dateTimeBrokerMock =
                new Mock<IDateTimeBroker>();

            this.listenerEventArchiveV1Service = new ListenerEventArchiveV1Service(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static SqlException GetSqlException()
        {
            return (SqlException)RuntimeHelpers
                .GetUninitializedObject(
                    type: typeof(SqlException));
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static T GetInvalidEnum<T>()
        {
            int randomNumber =
                GetLocalRandomNumber();

            while (Enum.IsDefined(typeof(T), randomNumber) is true)
            {
                randomNumber = GetLocalRandomNumber();
            }

            return (T)(object)randomNumber;

            static int GetLocalRandomNumber()
            {
                return new IntRange(
                    min: int.MinValue,
                    max: int.MaxValue)
                        .GetValue();
            }
        }

        public static TheoryData<int> MinutesBeforeAndAfterNow()
        {
            int randomMoreThanOneMinuteAhead =
                GetRandomNumber();

            int randomMoreThanOneMinuteAgo =
                GetRandomNegativeNumber();

            return new TheoryData<int>
            {
                randomMoreThanOneMinuteAhead,
                randomMoreThanOneMinuteAgo
            };
        }

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNegativeNumber() =>
            -1 * GetRandomNumber();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static ListenerEventArchiveV1 CreateRandomListenerEventArchiveV1() =>
            CreateListenerEventArchiveV1Filler(dates: GetRandomDateTimeOffset()).Create();

        private static ListenerEventArchiveV1 CreateRandomListenerEventArchiveV1(DateTimeOffset dates) =>
            CreateListenerEventArchiveV1Filler(dates).Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Filler<ListenerEventArchiveV1> CreateListenerEventArchiveV1Filler(DateTimeOffset dates)
        {
            var filler = new Filler<ListenerEventArchiveV1>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }
    }
}
