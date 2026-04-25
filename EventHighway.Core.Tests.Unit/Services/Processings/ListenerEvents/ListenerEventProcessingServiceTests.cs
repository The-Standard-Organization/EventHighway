// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents;
using EventHighway.Core.Services.Foundations.ListenerEvents;
using EventHighway.Core.Services.Processings.ListenerEvents;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents
{
    public partial class ListenerEventProcessingServiceTests
    {
        private readonly Mock<IListenerEventService> listenerEventServiceMock;
        private readonly IListenerEventProcessingService listenerEventProcessingService;

        public ListenerEventProcessingServiceTests()
        {
            this.listenerEventServiceMock = new Mock<IListenerEventService>();

            this.listenerEventProcessingService = new ListenerEventProcessingService(
                listenerEventService: this.listenerEventServiceMock.Object);
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
