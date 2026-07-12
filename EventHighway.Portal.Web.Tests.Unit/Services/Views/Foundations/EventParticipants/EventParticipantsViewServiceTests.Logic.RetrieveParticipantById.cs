// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventParticipants
{
    public partial class EventParticipantsViewServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveParticipantByIdAsync()
        {
            // given
            EventParticipantV2 randomParticipant = CreateRandomParticipants()[0];
            Guid inputId = randomParticipant.Id;
            EventParticipantView expectedView = MapToViews(new[] { randomParticipant })[0];

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveEventParticipantV2ByIdAsync(inputId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomParticipant);

            // when
            EventParticipantView actualView =
                await this.eventParticipantsViewService.RetrieveParticipantByIdAsync(
                    inputId, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveEventParticipantV2ByIdAsync(inputId, It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
