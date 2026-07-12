// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Portal.Web.Brokers.DateTimes;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventAddresses;
using EventHighway.Portal.Web.Services.Views.Foundations.EventAddresses;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventAddresses
{
    public partial class EventAddressesViewServiceTests
    {
        private readonly Mock<IEventHighwayBroker> eventHighwayBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventAddressesViewService eventAddressesViewService;

        public EventAddressesViewServiceTests()
        {
            this.eventHighwayBrokerMock = new Mock<IEventHighwayBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventAddressesViewService = new EventAddressesViewService(
                eventHighwayBroker: this.eventHighwayBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static EventAddressView CreateRandomAddressView() =>
            new EventAddressView
            {
                Name = GetRandomString(),
                Description = GetRandomString()
            };

        private static List<EventAddressV2> CreateRandomAddresses(int count)
        {
            DateTimeOffset now = GetRandomDateTimeOffset();

            return Enumerable.Range(0, count).Select(_ => new EventAddressV2
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString(),
                Description = GetRandomString(),
                CreatedDate = now,
                UpdatedDate = now
            }).ToList();
        }

        private static List<EventAddressView> MapToViews(IEnumerable<EventAddressV2> addresses) =>
            addresses.Select(address => new EventAddressView
            {
                Id = address.Id,
                Name = address.Name,
                Description = address.Description
            }).ToList();
    }
}
