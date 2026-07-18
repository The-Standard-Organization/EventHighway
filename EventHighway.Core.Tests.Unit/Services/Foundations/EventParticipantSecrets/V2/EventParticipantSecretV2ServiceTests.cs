// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using EventHighway.Core.Brokers.Hashings;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Services.Foundations.EventParticipantSecrets.V2;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IHashBroker> hashBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventParticipantSecretV2Service eventParticipantSecretV2Service;

        public EventParticipantSecretV2ServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.hashBrokerMock = new Mock<IHashBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventParticipantSecretV2Service = new EventParticipantSecretV2Service(
                storageBroker: this.storageBrokerMock.Object,
                hashBroker: this.hashBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static SqlException CreateSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(type: typeof(SqlException));

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomSecret() =>
            $"{Guid.NewGuid()}{Guid.NewGuid()}";

        private static EventParticipantSecretV2 CreateRandomEventParticipantSecretV2(DateTimeOffset dates) =>
            CreateEventParticipantSecretV2Filler(dates).Create();

        private static EventParticipantSecretV2 CreateRandomEventParticipantSecretV2() =>
            CreateEventParticipantSecretV2Filler(dates: GetRandomDateTimeOffset()).Create();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static IQueryable<EventParticipantSecretV2> CreateRandomEventParticipantSecretV2s()
        {
            return CreateEventParticipantSecretV2Filler(dates: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static int GetRandomNegativeNumber() =>
            -1 * GetRandomNumber();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Filler<EventParticipantSecretV2> CreateEventParticipantSecretV2Filler(DateTimeOffset dates)
        {
            var filler = new Filler<EventParticipantSecretV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates)

                .OnProperty(secret => secret.Secret)
                    .Use(() => GetRandomSecret())

                .OnProperty(secret => secret.IsActive)
                    .Use(true)

                .OnProperty(secret => secret.ActiveFrom)
                    .IgnoreIt()

                .OnProperty(secret => secret.ActiveTo)
                    .IgnoreIt()

                .OnProperty(secret => secret.EventParticipantV2)
                    .IgnoreIt();

            return filler;
        }
    }
}
