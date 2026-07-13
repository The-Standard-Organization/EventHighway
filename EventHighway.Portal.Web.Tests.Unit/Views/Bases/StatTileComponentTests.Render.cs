// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Bunit;
using EventHighway.Portal.Web.Views.Bases;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Bases
{
    public partial class StatTileComponentTests
    {
        [Fact]
        public void ShouldInitializeComponent()
        {
            // given . when
            IRenderedComponent<StatTile> renderedStatTile = Render<StatTile>();

            // then
            renderedStatTile.Instance.Variant.Should().Be(StatTileVariant.Na);
            renderedStatTile.Instance.Value.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(VariantCssClasses))]
        public void ShouldApplyVariantCssClass(StatTileVariant variant, string expectedCssClass)
        {
            // given . when
            IRenderedComponent<StatTile> renderedStatTile =
                Render<StatTile>(parameters =>
                    parameters.Add(statTile => statTile.Variant, variant));

            // then
            renderedStatTile.Find("div.stat-tile").ClassList.Should().Contain(expectedCssClass);
        }

        [Fact]
        public void ShouldRenderValueAndLabel()
        {
            // given
            string randomValue = GetRandomString();
            string randomLabel = GetRandomString();

            // when
            IRenderedComponent<StatTile> renderedStatTile =
                Render<StatTile>(parameters => parameters
                    .Add(statTile => statTile.Value, randomValue)
                    .Add(statTile => statTile.Label, randomLabel));

            // then
            renderedStatTile.Markup.Should().Contain(randomValue);
            renderedStatTile.Markup.Should().Contain(randomLabel);
        }
    }
}
