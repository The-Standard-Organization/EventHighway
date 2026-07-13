// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Bunit;
using EventHighway.Portal.Web.Views.Bases;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Bases
{
    public partial class SpinnerComponentTests
    {
        [Fact]
        public void ShouldInitializeComponent()
        {
            // given . when
            IRenderedComponent<Spinner> renderedSpinner = Render<Spinner>();

            // then
            renderedSpinner.Instance.Visible.Should().BeTrue();
            renderedSpinner.Find("div.spinner-border").Should().NotBeNull();
        }

        [Fact]
        public void ShouldNotRenderSpinnerWhenVisibleIsFalse()
        {
            // given . when
            IRenderedComponent<Spinner> renderedSpinner =
                Render<Spinner>(parameters =>
                    parameters.Add(spinner => spinner.Visible, false));

            // then
            renderedSpinner.FindAll("div.spinner-border").Should().BeEmpty();
        }
    }
}
