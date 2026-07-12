// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Bunit;
using EventHighway.Portal.Web.Views.Bases;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.HealthDashboards;
using EventHighway.Portal.Web.Services.Views.Foundations.HealthDashboards;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Components.Foundations.Dashboards
{
    public partial class RagTileRowComponentTests : BunitContext
    {
        private readonly Mock<IHealthViewService> healthViewServiceMock;

        public RagTileRowComponentTests()
        {
            this.healthViewServiceMock = new Mock<IHealthViewService>();
            Services.AddSingleton(this.healthViewServiceMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static List<HealthRagTile> CreateRandomRagTiles() =>
            Enumerable.Range(0, 4).Select(_ => new HealthRagTile
            {
                Grouping = GetRandomString(),
                Label = GetRandomString(),
                Value = GetRandomString(),
                Description = GetRandomString(),
                Variant = StatTileVariant.Green
            }).ToList();
    }
}
