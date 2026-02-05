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
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Services.Foundations.EventArchives.V1;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventV1ArchiveServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventV1ArchiveService eventV1ArchiveService;

        public EventV1ArchiveServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventV1ArchiveService = new EventV1ArchiveService(
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
            new IntRange(min: 2, max: 10).GetValue();

        private static EventV1Archive CreateRandomEventV1Archive(DateTimeOffset date) =>
            CreateEventV1ArchiveFiller(date).Create();

        private static EventV1Archive CreateRandomEventV1Archive() =>
            CreateEventV1ArchiveFiller(date: GetRandomDateTimeOffset()).Create();

        private static IQueryable<EventV1Archive> CreateRandomEventV1Archives()
        {
            return CreateEventV1ArchiveFiller(date: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

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

        private static Filler<EventV1Archive> CreateEventV1ArchiveFiller(
            DateTimeOffset date)
        {
            var filler = new Filler<EventV1Archive>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(date)

                .OnType<DateTimeOffset?>().Use(
                    GetRandomDateTimeOffset());

            return filler;
        }
    }
}
