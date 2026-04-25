// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Services.Foundations.ListernEvents.V1;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V1
{
    public partial class ListenerEventV1ServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IListenerEventV1Service listenerEventV1Service;

        public ListenerEventV1ServiceTests()
        {
            this.storageBrokerMock =
                new Mock<IStorageBroker>();

            this.loggingBrokerMock =
                new Mock<ILoggingBroker>();

            this.dateTimeBrokerMock =
                new Mock<IDateTimeBroker>();

            this.listenerEventV1Service = new ListenerEventV1Service(
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

        private static ListenerEventV1 CreateRandomListenerEventV1() =>
            CreateListenerEventV1Filler(dates: GetRandomDateTimeOffset()).Create();

        private static IQueryable<ListenerEventV1> CreateRandomListenerEventV1s()
        {
            return CreateListenerEventV1Filler(
                dates: GetRandomDateTimeOffset()).Create(
                    count: GetRandomNumber())
                        .AsQueryable();
        }

        private static ListenerEventV1 CreateRandomListenerEventV1(DateTimeOffset dates) =>
            CreateListenerEventV1Filler(dates).Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Filler<ListenerEventV1> CreateListenerEventV1Filler(DateTimeOffset dates)
        {
            var filler = new Filler<ListenerEventV1>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates)

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
