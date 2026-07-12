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
        public async Task ShouldRemoveSecretByIdAsync()
        {
            // given
            Guid participantId = Guid.NewGuid();

            List<EventParticipantSecretV2> randomSecrets =
                CreateRandomSecrets(participantId, count: 1);

            EventParticipantSecretV2 removedSecret = randomSecrets[0];
            Guid secretId = removedSecret.Id;

            EventParticipantSecretView expectedView =
                MapToViews(new[] { removedSecret })[0];

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RemoveEventParticipantSecretV2ByIdAsync(
                    secretId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(removedSecret);

            // when
            EventParticipantSecretView actualView =
                await this.eventParticipantSecretsViewService.RemoveSecretByIdAsync(
                    secretId, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RemoveEventParticipantSecretV2ByIdAsync(
                    secretId, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
