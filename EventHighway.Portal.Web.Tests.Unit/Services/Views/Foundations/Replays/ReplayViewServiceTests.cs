// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Services.Views.Foundations.Replays;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.Replays
{
    public partial class ReplayViewServiceTests
    {
        private readonly Mock<IEventHighwayBroker> eventHighwayBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IReplayViewService replayViewService;

        public ReplayViewServiceTests()
        {
            this.eventHighwayBrokerMock = new Mock<IEventHighwayBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.replayViewService = new ReplayViewService(
                eventHighwayBroker: this.eventHighwayBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();
    }
}
