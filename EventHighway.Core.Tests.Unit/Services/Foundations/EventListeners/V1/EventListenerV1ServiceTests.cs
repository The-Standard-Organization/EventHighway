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
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Services.Foundations.EventListeners.V1;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListeners.V1
{
    public partial class EventListenerV1ServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly IEventListenerV1Service eventListenerV1Service;

        public EventListenerV1ServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();

            this.eventListenerV1Service = new EventListenerV1Service(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

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

        private static SqlException GetSqlException()
        {
            return (SqlException)RuntimeHelpers
                .GetUninitializedObject(type: typeof(SqlException));
        }

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static IQueryable<EventListenerV1> CreateRandomEventListenerV1s()
        {
            return CreateEventListenerV1Filler(
                dates: GetRandomDateTimeOffset())
                    .Create(count: GetRandomNumber())
                        .AsQueryable();
        }

        private static EventListenerV1 CreateRandomEventListenerV1() =>
            CreateEventListenerV1Filler(dates: GetRandomDateTimeOffset()).Create();

        private static EventListenerV1 CreateRandomEventListenerV1(DateTimeOffset dates) =>
            CreateEventListenerV1Filler(dates).Create();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * GetRandomNumber();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Filler<EventListenerV1> CreateEventListenerV1Filler(DateTimeOffset dates)
        {
            var filler = new Filler<EventListenerV1>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates)

                .OnProperty(eventListenerV1 =>
                    eventListenerV1.EventAddressV1s).IgnoreIt()

                .OnProperty(eventListenerV1 =>
                    eventListenerV1.ListenerEventV1s).IgnoreIt();

            return filler;
        }
    }
}
