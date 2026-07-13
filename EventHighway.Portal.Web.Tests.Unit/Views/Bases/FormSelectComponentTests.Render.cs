// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Bunit;
using EventHighway.Portal.Web.Views.Bases;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Bases
{
    public partial class FormSelectComponentTests
    {
        [Fact]
        public void ShouldInitializeComponent()
        {
            // given . when
            IRenderedComponent<FormSelect> renderedFormSelect = Render<FormSelect>();

            // then
            renderedFormSelect.Find("select").Should().NotBeNull();
        }

        [Fact]
        public void ShouldRenderOptions()
        {
            // given . when
            IRenderedComponent<FormSelect> renderedFormSelect =
                Render<FormSelect>(parameters => parameters
                    .Add(formSelect => formSelect.Options, CreateOptions()));

            // then
            renderedFormSelect.FindAll("option").Should().HaveCount(2);
            renderedFormSelect.Markup.Should().Contain("5 min");
        }

        [Fact]
        public void ShouldInvokeValueChangedOnChange()
        {
            // given
            string? boundValue = null;

            IRenderedComponent<FormSelect> renderedFormSelect =
                Render<FormSelect>(parameters => parameters
                    .Add(formSelect => formSelect.Options, CreateOptions())
                    .Add(formSelect => formSelect.ValueChanged, value => boundValue = value));

            // when
            renderedFormSelect.Find("select").Change("5");

            // then
            boundValue.Should().Be("5");
        }
    }
}
