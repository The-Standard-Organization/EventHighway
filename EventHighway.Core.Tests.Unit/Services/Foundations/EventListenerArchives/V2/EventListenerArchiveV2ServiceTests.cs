// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;
using EventHighway.Core.Services.Foundations.EventListenerArchives.V2;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListenerArchives.V2
{
    public partial class EventListenerArchiveV2ServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventListenerArchiveV2Service eventListenerArchiveV2Service;

        public EventListenerArchiveV2ServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventListenerArchiveV2Service = new EventListenerArchiveV2Service(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static bool SameEventListenerArchiveV2sAs(
            List<EventListenerArchiveV2> expectedEventListenerArchiveV2s,
            List<EventListenerArchiveV2> actualEventListenerArchiveV2s)
        {
            var compareLogic = new CompareLogic();

            ComparisonResult comparisonResult =
                compareLogic.Compare(expectedEventListenerArchiveV2s, actualEventListenerArchiveV2s);

            return comparisonResult.AreEqual;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static SqlException CreateSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(type: typeof(SqlException));

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static EventListenerArchiveV2 CreateRandomEventListenerArchiveV2(DateTimeOffset date) =>
            CreateEventListenerArchiveV2Filler(date).Create();

        private static EventListenerArchiveV2 CreateRandomEventListenerArchiveV2() =>
            CreateEventListenerArchiveV2Filler(date: GetRandomDateTimeOffset()).Create();

        private static IQueryable<EventListenerArchiveV2> CreateRandomEventListenerArchiveV2s()
        {
            return CreateEventListenerArchiveV2Filler(date: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Filler<EventListenerArchiveV2> CreateEventListenerArchiveV2Filler(DateTimeOffset date)
        {
            var filler = new Filler<EventListenerArchiveV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(date);

            return filler;
        }
    }
}
