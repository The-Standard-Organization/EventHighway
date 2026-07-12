// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Bunit;
using EventHighway.Portal.Web.Views.Bases;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Bases
{
    public partial class ModalComponentTests
    {
        [Fact]
        public void ShouldInitializeComponent()
        {
            // given . when
            IRenderedComponent<Modal> renderedModal = Render<Modal>();

            // then
            renderedModal.Instance.Visible.Should().BeFalse();
            renderedModal.FindAll("div.modal").Should().BeEmpty();
        }

        [Fact]
        public void ShouldRenderModalWithTitleAndBodyWhenVisible()
        {
            // given
            string randomTitle = GetRandomString();
            string randomBody = GetRandomString();

            // when
            IRenderedComponent<Modal> renderedModal =
                Render<Modal>(parameters => parameters
                    .Add(modal => modal.Visible, true)
                    .Add(modal => modal.Title, randomTitle)
                    .AddChildContent(randomBody));

            // then
            renderedModal.Find("div.modal").Should().NotBeNull();
            renderedModal.Find("div.modal-title").TextContent.Should().Contain(randomTitle);
            renderedModal.Find("div.modal-body").TextContent.Should().Contain(randomBody);
        }

        [Fact]
        public void ShouldRenderDefaultDialogWithoutSizeOrScrollableClasses()
        {
            // given . when
            IRenderedComponent<Modal> renderedModal =
                Render<Modal>(parameters => parameters
                    .Add(modal => modal.Visible, true));

            // then
            string dialogClass = renderedModal.Find("div.modal-dialog").GetAttribute("class");

            dialogClass.Should().Contain("modal-dialog-centered");
            dialogClass.Should().NotContain("modal-dialog-scrollable");
            dialogClass.Should().NotContain("modal-lg");
            dialogClass.Should().NotContain("modal-xl");
        }

        [Theory]
        [InlineData("lg")]
        [InlineData("xl")]
        public void ShouldRenderSizeClassWhenSizeProvided(string size)
        {
            // given . when
            IRenderedComponent<Modal> renderedModal =
                Render<Modal>(parameters => parameters
                    .Add(modal => modal.Visible, true)
                    .Add(modal => modal.Size, size));

            // then
            renderedModal.Find("div.modal-dialog").GetAttribute("class")
                .Should().Contain($"modal-{size}");
        }

        [Fact]
        public void ShouldRenderScrollableClassWhenScrollableIsTrue()
        {
            // given . when
            IRenderedComponent<Modal> renderedModal =
                Render<Modal>(parameters => parameters
                    .Add(modal => modal.Visible, true)
                    .Add(modal => modal.Scrollable, true));

            // then
            renderedModal.Find("div.modal-dialog").GetAttribute("class")
                .Should().Contain("modal-dialog-scrollable");
        }

        [Fact]
        public void ShouldInvokeOnCloseWhenCloseButtonClicked()
        {
            // given
            bool wasClosed = false;

            IRenderedComponent<Modal> renderedModal =
                Render<Modal>(parameters => parameters
                    .Add(modal => modal.Visible, true)
                    .Add(modal => modal.OnClose, () => wasClosed = true));

            // when
            renderedModal.Find("button.btn-close").Click();

            // then
            wasClosed.Should().BeTrue();
        }
    }
}
