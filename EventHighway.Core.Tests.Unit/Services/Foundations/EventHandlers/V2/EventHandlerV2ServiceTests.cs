// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Services.Foundations.EventHandlers.V2;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventHandlers.V2
{
    public partial class EventHandlerV2ServiceTests
    {
        private readonly Mock<IEventHandlerBroker> eventHandlerBrokerMock;
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventHandlerV2Service eventHandlerV2Service;

        public EventHandlerV2ServiceTests()
        {
            this.eventHandlerBrokerMock = new Mock<IEventHandlerBroker>();
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventHandlerV2Service = new EventHandlerV2Service(
                eventHandlerBroker: this.eventHandlerBrokerMock.Object,
                storageBroker: this.storageBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString(1).GetValue();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static SqlException CreateSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(type: typeof(SqlException));

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static EventHandlerV2 CreateRandomEventHandlerV2() =>
            new Filler<EventHandlerV2>().Create();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static IQueryable<EventHandlerV2> CreateRandomEventHandlerV2s() =>
            new Filler<EventHandlerV2>().Create(count: GetRandomNumber()).AsQueryable();

        private static IEventHandler CreateRandomEventHandler()
        {
            var mock = new Mock<IEventHandler>();
            mock.SetupGet(h => h.Id).Returns(Guid.NewGuid());
            mock.SetupGet(h => h.Name).Returns(new MnemonicString(1).GetValue());
            return mock.Object;
        }

        private static IEnumerable<IEventHandler> CreateRandomEventHandlers()
        {
            int count = new IntRange(min: 2, max: 9).GetValue();
            var handlers = new List<IEventHandler>();

            for (int i = 0; i < count; i++)
                handlers.Add(CreateRandomEventHandler());

            return handlers;
        }
    }
}
