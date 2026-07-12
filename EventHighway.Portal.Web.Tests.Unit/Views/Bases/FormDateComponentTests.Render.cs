// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Bunit;
using EventHighway.Portal.Web.Views.Bases;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Bases
{
    public partial class FormDateComponentTests
    {
        [Fact]
        public void ShouldInitializeComponent()
        {
            // given . when
            IRenderedComponent<FormDate> renderedFormDate = Render<FormDate>();

            // then
            renderedFormDate.Instance.Value.Should().BeNull();
            renderedFormDate.Find("input[type=date]").Should().NotBeNull();
        }

        [Fact]
        public void ShouldRenderLabelAndValue()
        {
            // given
            string randomLabel = GetRandomString();

            var inputValue =
                new DateTimeOffset(2026, 6, 29, 0, 0, 0, TimeSpan.Zero);

            // when
            IRenderedComponent<FormDate> renderedFormDate =
                Render<FormDate>(parameters => parameters
                    .Add(formDate => formDate.Label, randomLabel)
                    .Add(formDate => formDate.Value, inputValue));

            // then
            renderedFormDate.Find("label").TextContent.Should().Contain(randomLabel);

            renderedFormDate.Find("input[type=date]").GetAttribute("value")
                .Should().Be("2026-06-29");
        }

        [Fact]
        public void ShouldInvokeValueChangedOnChange()
        {
            // given
            DateTimeOffset? boundValue = null;

            IRenderedComponent<FormDate> renderedFormDate =
                Render<FormDate>(parameters => parameters
                    .Add(formDate => formDate.ValueChanged, value => boundValue = value));

            // when
            renderedFormDate.Find("input[type=date]").Change("2026-06-29");

            // then
            boundValue.Should().Be(
                new DateTimeOffset(2026, 6, 29, 0, 0, 0, TimeSpan.Zero));
        }
    }
}
