// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldAddSecretAsync()
        {
            // given
            Guid participantId = Guid.NewGuid();
            DateTimeOffset now = GetRandomDateTimeOffset();

            var inputView = new EventParticipantSecretView
            {
                Secret = GetRandomString(),
                IsActive = true,
                ActiveFrom = null,
                ActiveTo = null,
                EventParticipantV2Id = participantId
            };

            var returnedSecret = new EventParticipantSecretV2
            {
                Id = Guid.NewGuid(),
                Secret = inputView.Secret,
                IsActive = inputView.IsActive,
                ActiveFrom = inputView.ActiveFrom,
                ActiveTo = inputView.ActiveTo,
                CreatedDate = now,
                UpdatedDate = now,
                EventParticipantV2Id = participantId
            };

            EventParticipantSecretView expectedView =
                MapToViews(new[] { returnedSecret })[0];

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.AddEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(returnedSecret);

            // when
            EventParticipantSecretView actualView =
                await this.eventParticipantSecretsViewService.AddSecretAsync(
                    inputView, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.AddEventParticipantSecretV2Async(
                    It.Is<EventParticipantSecretV2>(secret =>
                        secret.Id != Guid.Empty
                        && secret.Secret == inputView.Secret
                        && secret.EventParticipantV2Id == participantId
                        && secret.IsActive == inputView.IsActive
                        && secret.CreatedDate == now
                        && secret.UpdatedDate == now),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
