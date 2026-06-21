// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using EventHighway.Core.Brokers.Jsons;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.Events.V2;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V2
{
    public partial class EventV2ServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<IJsonBroker> jsonBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventV2Service eventV2Service;

        public EventV2ServiceTests()
        {
            this.storageBrokerMock =
                new Mock<IStorageBroker>();

            this.loggingBrokerMock =
                new Mock<ILoggingBroker>();

            this.dateTimeBrokerMock =
                new Mock<IDateTimeBroker>();

            this.jsonBrokerMock =
                new Mock<IJsonBroker>();

            this.eventV2Service = new EventV2Service(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                jsonBroker: this.jsonBrokerMock.Object,
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


        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomStringWithLengthOf(int length) =>
            new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * GetRandomNumber();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

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

        private static IQueryable<EventV2> CreateRandomEventV2s()
        {
            return CreateEventV2Filler(dates: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static EventV2 CreateRandomEventV2()
        {
            return CreateEventV2Filler(
                dates: GetRandomDateTimeOffset())
                    .Create();
        }

        private static EventV2 CreateRandomEventV2(DateTimeOffset dates) =>
            CreateEventV2Filler(dates).Create();

        private static DateTimeOffset GetRandomDateTimeOffset()
        {
            return new DateTimeRange(
                earliestDate: DateTime.UnixEpoch)
                    .GetValue();
        }

        private static Filler<EventV2> CreateEventV2Filler(DateTimeOffset dates)
        {
            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset())

                .OnProperty(eventV2 => eventV2.EventAddressV2)
                    .IgnoreIt()

                .OnProperty(eventV2 => eventV2.ListenerEventV2s)
                    .IgnoreIt()

                .OnType<EventAddressV2>().IgnoreIt()
                .OnType<ListenerEventV2>().IgnoreIt();

            return filler;
        }
    }
}
