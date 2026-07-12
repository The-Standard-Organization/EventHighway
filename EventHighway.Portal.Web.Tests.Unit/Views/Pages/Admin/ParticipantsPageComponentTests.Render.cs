// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using Bunit;
using EventHighway.Portal.Web.Views.Pages.Admin;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Pages.Admin
{
    public partial class ParticipantsPageComponentTests
    {
        [Fact]
        public void ShouldRenderParticipantsWithIdAsFirstColumn()
        {
            // given
            List<EventParticipantView> participants = CreateRandomParticipants(count: 2);

            this.participantsViewServiceMock.Setup(service =>
                service.RetrieveAllParticipantsAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(participants);

            // when
            IRenderedComponent<ParticipantsPage> renderedPage = Render<ParticipantsPage>();

            // then
            renderedPage.FindAll("thead th")[0].TextContent.Trim().Should().Be("Id");
            renderedPage.Markup.Should().Contain(participants[0].Id.ToString());
            renderedPage.Markup.Should().Contain(participants[0].Name);
        }
    }
}
