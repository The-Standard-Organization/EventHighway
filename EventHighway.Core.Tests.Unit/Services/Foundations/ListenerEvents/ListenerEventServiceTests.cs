// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents;
using EventHighway.Core.Services.Foundations.ListenerEvents;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Core.Tests.Unit.Services.ListenerEvents
{
    public partial class ListenerEventServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly IListenerEventService listenerEventService;

        public ListenerEventServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();

            this.listenerEventService = new ListenerEventService(
                storageBroker: this.storageBrokerMock.Object);
        }

        private static ListenerEvent CreateRandomListenerEvent() =>
            CreateListenerEventFiller().Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Filler<ListenerEvent> CreateListenerEventFiller()
        {
            var filler = new Filler<ListenerEvent>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset);

            return filler;
        }
    }
}
