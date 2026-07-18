// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventParticipants
{
    public partial class EventParticipantsViewServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllParticipantsAsync()
        {
            // given
            List<EventParticipantV2> randomParticipants = CreateRandomParticipants();
            IReadOnlyList<EventParticipantV2> returnedParticipants = randomParticipants;
            List<EventParticipantView> expectedViews = MapToViews(randomParticipants);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventParticipantV2sAsync(
                    It.IsAny<EventParticipantV2Query>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(returnedParticipants);

            // when
            List<EventParticipantView> actualViews =
                await this.eventParticipantsViewService.RetrieveAllParticipantsAsync(
                    TestContext.Current.CancellationToken);

            // then
            actualViews.Should().BeEquivalentTo(expectedViews);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventParticipantV2sAsync(
                    It.IsAny<EventParticipantV2Query>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
