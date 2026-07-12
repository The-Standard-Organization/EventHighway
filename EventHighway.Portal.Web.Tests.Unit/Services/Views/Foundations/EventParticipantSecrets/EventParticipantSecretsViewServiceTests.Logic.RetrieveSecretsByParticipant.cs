// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipantSecrets;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventParticipantSecrets
{
    public partial class EventParticipantSecretsViewServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveSecretsByParticipantAsync()
        {
            // given
            Guid participantId = Guid.NewGuid();
            Guid otherParticipantId = Guid.NewGuid();

            List<EventParticipantSecretV2> participantSecrets =
                CreateRandomSecrets(participantId, count: 2);

            List<EventParticipantSecretV2> otherSecrets =
                CreateRandomSecrets(otherParticipantId, count: 2);

            var allSecrets = new List<EventParticipantSecretV2>();
            allSecrets.AddRange(participantSecrets);
            allSecrets.AddRange(otherSecrets);

            List<EventParticipantSecretView> expectedViews =
                MapToViews(participantSecrets);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventParticipantSecretV2sAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(allSecrets);

            // when
            List<EventParticipantSecretView> actualViews =
                await this.eventParticipantSecretsViewService
                    .RetrieveSecretsByParticipantAsync(
                        participantId, TestContext.Current.CancellationToken);

            // then
            actualViews.Should().BeEquivalentTo(expectedViews);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventParticipantSecretV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
