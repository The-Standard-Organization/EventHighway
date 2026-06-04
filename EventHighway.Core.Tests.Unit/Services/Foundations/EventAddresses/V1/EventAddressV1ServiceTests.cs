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
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Services.Foundations.EventAddresses.V1;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventAddresses.V1
{
    public partial class EventAddressV1ServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventAddressV1Service eventAddressV1Service;

        public EventAddressV1ServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventAddressV1Service = new EventAddressV1Service(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static SqlException CreateSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(type: typeof(SqlException));

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

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * GetRandomNumber();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static EventAddressV1 CreateRandomEventAddressV1(DateTimeOffset dates) =>
            CreateEventAddressV1Filler(dates).Create();

        private static EventAddressV1 CreateRandomEventAddressV1() =>
            CreateEventAddressV1Filler(dates: GetRandomDateTimeOffset()).Create();

        private static IQueryable<EventAddressV1> CreateRandomEventAddressV1s()
        {
            return CreateEventAddressV1Filler(dates: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Filler<EventAddressV1> CreateEventAddressV1Filler(DateTimeOffset dates)
        {
            var filler = new Filler<EventAddressV1>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates)

                .OnProperty(eventAddressV1 => eventAddressV1.Events)
                    .IgnoreIt()

                .OnProperty(eventAddressV1 => eventAddressV1.EventListenerV1s)
                    .IgnoreIt()

                .OnProperty(eventAddressV1 => eventAddressV1.ListenerEventV1s)
                    .IgnoreIt();

            return filler;
        }
    }
}
