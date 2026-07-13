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
        public async Task ShouldRemoveParticipantByIdAsync()
        {
            // given
            Guid participantId = Guid.NewGuid();

            EventParticipantV2 removedParticipant = CreateRandomParticipants()[0];
            removedParticipant.Id = participantId;

            EventParticipantView expectedView =
                MapToViews(new[] { removedParticipant })[0];

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RemoveEventParticipantV2ByIdAsync(
                    participantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(removedParticipant);

            // when
            EventParticipantView actualView =
                await this.eventParticipantsViewService.RemoveParticipantByIdAsync(
                    participantId, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RemoveEventParticipantV2ByIdAsync(
                    participantId, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
