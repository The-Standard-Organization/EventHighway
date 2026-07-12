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
        public async Task ShouldModifySecretAsync()
        {
            // given
            Guid participantId = Guid.NewGuid();

            List<EventParticipantSecretV2> existingSecrets =
                CreateRandomSecrets(participantId, count: 3);

            EventParticipantSecretV2 targetSecret = existingSecrets[1];
            DateTimeOffset now = GetRandomDateTimeOffset();

            EventParticipantSecretView inputView = new EventParticipantSecretView
            {
                Id = targetSecret.Id,
                Secret = targetSecret.Secret,
                IsActive = false,
                ActiveFrom = GetRandomDateTimeOffset(),
                ActiveTo = GetRandomDateTimeOffset(),
                EventParticipantV2Id = participantId
            };

            EventParticipantSecretV2 returnedSecret = new EventParticipantSecretV2
            {
                Id = targetSecret.Id,
                Secret = targetSecret.Secret,
                IsActive = inputView.IsActive,
                ActiveFrom = inputView.ActiveFrom,
                ActiveTo = inputView.ActiveTo,
                EventParticipantV2Id = participantId,
                CreatedDate = targetSecret.CreatedDate,
                UpdatedDate = now
            };

            EventParticipantSecretView expectedView =
                MapToViews(new[] { returnedSecret })[0];

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventParticipantSecretV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(existingSecrets);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.ModifyEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(returnedSecret);

            // when
            EventParticipantSecretView actualView =
                await this.eventParticipantSecretsViewService.ModifySecretAsync(
                    inputView, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventParticipantSecretV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.ModifyEventParticipantSecretV2Async(
                    It.Is<EventParticipantSecretV2>(secret =>
                        secret.Id == inputView.Id
                        && secret.IsActive == inputView.IsActive
                        && secret.ActiveFrom == inputView.ActiveFrom
                        && secret.ActiveTo == inputView.ActiveTo
                        && secret.CreatedDate == targetSecret.CreatedDate
                        && secret.UpdatedDate == now),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
