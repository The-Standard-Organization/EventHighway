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
        public async Task ShouldAddParticipantAsync()
        {
            // given
            EventParticipantView inputView = CreateRandomParticipantView();
            DateTimeOffset now = GetRandomDateTimeOffset();

            EventParticipantV2 returnedParticipant = new EventParticipantV2
            {
                Id = Guid.NewGuid(),
                Name = inputView.Name,
                Description = inputView.Description,
                ContactEmail = inputView.ContactEmail,
                ContactPhone = inputView.ContactPhone,
                IsActive = inputView.IsActive,
                IsSecretRequired = inputView.IsSecretRequired,
                ActiveFrom = inputView.ActiveFrom,
                ActiveTo = inputView.ActiveTo,
                CreatedDate = now,
                UpdatedDate = now
            };

            EventParticipantView expectedView =
                MapToViews(new[] { returnedParticipant })[0];

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.AddEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(returnedParticipant);

            // when
            EventParticipantView actualView =
                await this.eventParticipantsViewService.AddParticipantAsync(
                    inputView, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.AddEventParticipantV2Async(
                    It.Is<EventParticipantV2>(participant =>
                        participant.Name == inputView.Name
                        && participant.Description == inputView.Description
                        && participant.ContactEmail == inputView.ContactEmail
                        && participant.ContactPhone == inputView.ContactPhone
                        && participant.IsActive == inputView.IsActive
                        && participant.IsSecretRequired == inputView.IsSecretRequired
                        && participant.CreatedDate == now
                        && participant.UpdatedDate == now),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
