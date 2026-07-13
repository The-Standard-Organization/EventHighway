// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using Bunit;
using EventHighway.Portal.Web.Views.Bases;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Bases
{
    public partial class ConfirmDialogComponentTests
    {
        [Fact]
        public void ShouldNotRenderWhenHidden()
        {
            // given . when
            IRenderedComponent<ConfirmDialog> renderedDialog =
                Render<ConfirmDialog>(parameters => parameters
                    .Add(dialog => dialog.Visible, false));

            // then
            renderedDialog.FindAll(".modal").Should().BeEmpty();
        }

        [Fact]
        public void ShouldRenderMessageWhenVisible()
        {
            // given
            string randomMessage = GetRandomString();

            // when
            IRenderedComponent<ConfirmDialog> renderedDialog =
                Render<ConfirmDialog>(parameters => parameters
                    .Add(dialog => dialog.Visible, true)
                    .Add(dialog => dialog.Message, randomMessage));

            // then
            renderedDialog.Markup.Should().Contain(randomMessage);
        }

        [Fact]
        public void ShouldInvokeOnConfirmWhenConfirmClicked()
        {
            // given
            bool confirmed = false;

            IRenderedComponent<ConfirmDialog> renderedDialog =
                Render<ConfirmDialog>(parameters => parameters
                    .Add(dialog => dialog.Visible, true)
                    .Add(dialog => dialog.ConfirmText, "OK")
                    .Add(dialog => dialog.OnConfirm, () => confirmed = true));

            // when
            renderedDialog.FindAll("button")
                .First(button => button.TextContent.Trim() == "OK")
                .Click();

            // then
            confirmed.Should().BeTrue();
        }

        [Fact]
        public void ShouldInvokeOnCancelWhenCancelClicked()
        {
            // given
            bool cancelled = false;

            IRenderedComponent<ConfirmDialog> renderedDialog =
                Render<ConfirmDialog>(parameters => parameters
                    .Add(dialog => dialog.Visible, true)
                    .Add(dialog => dialog.CancelText, "Cancel")
                    .Add(dialog => dialog.OnCancel, () => cancelled = true));

            // when
            renderedDialog.FindAll("button")
                .First(button => button.TextContent.Trim() == "Cancel")
                .Click();

            // then
            cancelled.Should().BeTrue();
        }
    }
}
