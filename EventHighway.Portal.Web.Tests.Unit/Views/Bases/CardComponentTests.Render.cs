// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Bunit;
using EventHighway.Portal.Web.Views.Bases;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Bases
{
    public partial class CardComponentTests
    {
        [Fact]
        public void ShouldInitializeComponent()
        {
            // given . when
            IRenderedComponent<Card> renderedCard = Render<Card>();

            // then
            renderedCard.Find("div.card").Should().NotBeNull();
            renderedCard.Instance.Title.Should().BeNull();
        }

        [Fact]
        public void ShouldRenderTitleBodyAndFooter()
        {
            // given
            string randomTitle = GetRandomString();
            string randomBody = GetRandomString();
            string randomFooter = GetRandomString();

            // when
            IRenderedComponent<Card> renderedCard =
                Render<Card>(parameters => parameters
                    .Add(card => card.Title, randomTitle)
                    .AddChildContent(randomBody)
                    .Add(card => card.FooterContent, randomFooter));

            // then
            renderedCard.Find("div.card-header").TextContent.Should().Contain(randomTitle);
            renderedCard.Find("div.card-body").TextContent.Should().Contain(randomBody);
            renderedCard.Find("div.card-footer").TextContent.Should().Contain(randomFooter);
        }
    }
}
