// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.HealthDashboards;
using EventHighway.Portal.Web.Services.Views.Foundations.HealthDashboards;
using EventHighway.Portal.Web.Views.Bases;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.HealthDashboards
{
    public partial class HealthViewServiceTests
    {
        private readonly Mock<IEventHighwayBroker> eventHighwayBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IHealthViewService healthViewService;

        public HealthViewServiceTests()
        {
            this.eventHighwayBrokerMock = new Mock<IEventHighwayBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.healthViewService = new HealthViewService(
                eventHighwayBroker: this.eventHighwayBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            var someInnerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new HealthStatusClientV2DependencyException(
                    message: GetRandomString(),
                    innerException: someInnerException,
                    data: new Hashtable()),

                new HealthStatusClientV2ServiceException(
                    message: GetRandomString(),
                    innerException: someInnerException,
                    data: new Hashtable()),
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static List<HealthCheckItemV2> CreateRandomHealthCheckItems()
        {
            string[] statuses = { "Green", "Amber", "Red", "Unknown" };

            return statuses.Select((status, index) => new HealthCheckItemV2
            {
                Grouping = GetRandomString(),
                Item = GetRandomString(),
                Value = GetRandomString(),
                Description = GetRandomString(),
                StatusCode = index,
                Status = status
            }).ToList();
        }

        private static StatTileVariant MapVariant(string status) =>
            status switch
            {
                "Green" => StatTileVariant.Green,
                "Amber" => StatTileVariant.Amber,
                "Red" => StatTileVariant.Red,
                _ => StatTileVariant.Na
            };

        private static List<HealthRagTile> MapToTiles(IEnumerable<HealthCheckItemV2> items) =>
            items.Select(item => new HealthRagTile
            {
                Grouping = item.Grouping,
                Label = item.Item,
                Value = item.Value,
                Description = item.Description,
                StatusCode = item.StatusCode,
                Variant = MapVariant(item.Status)
            }).ToList();
    }
}
