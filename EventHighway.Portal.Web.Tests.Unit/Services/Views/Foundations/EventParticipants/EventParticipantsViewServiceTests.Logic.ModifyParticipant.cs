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
        public async Task ShouldModifyParticipantAsync()
        {
            // given
            EventParticipantView inputView = CreateRandomParticipantView();
            inputView.Id = Guid.NewGuid();

            DateTimeOffset createdDate = GetRandomDateTimeOffset();
            DateTimeOffset now = GetRandomDateTimeOffset();

            EventParticipantV2 existingParticipant = new EventParticipantV2
            {
                Id = inputView.Id,
                Name = GetRandomString(),
                Description = GetRandomString(),
                ContactEmail = GetRandomString(),
                ContactPhone = GetRandomString(),
                IsActive = false,
                IsSecretRequired = false,
                CreatedDate = createdDate,
                UpdatedDate = createdDate
            };

            EventParticipantV2 returnedParticipant = new EventParticipantV2
            {
                Id = inputView.Id,
                Name = inputView.Name,
                Description = inputView.Description,
                ContactEmail = inputView.ContactEmail,
                ContactPhone = inputView.ContactPhone,
                IsActive = inputView.IsActive,
                IsSecretRequired = inputView.IsSecretRequired,
                ActiveFrom = inputView.ActiveFrom,
                ActiveTo = inputView.ActiveTo,
                CreatedDate = createdDate,
                UpdatedDate = now
            };

            EventParticipantView expectedView =
                MapToViews(new[] { returnedParticipant })[0];

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveEventParticipantV2ByIdAsync(
                    inputView.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(existingParticipant);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.ModifyEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(returnedParticipant);

            // when
            EventParticipantView actualView =
                await this.eventParticipantsViewService.ModifyParticipantAsync(
                    inputView, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveEventParticipantV2ByIdAsync(
                    inputView.Id, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.ModifyEventParticipantV2Async(
                    It.Is<EventParticipantV2>(participant =>
                        participant.Id == inputView.Id
                        && participant.Name == inputView.Name
                        && participant.Description == inputView.Description
                        && participant.ContactEmail == inputView.ContactEmail
                        && participant.ContactPhone == inputView.ContactPhone
                        && participant.IsActive == inputView.IsActive
                        && participant.IsSecretRequired == inputView.IsSecretRequired
                        && participant.CreatedDate == createdDate
                        && participant.UpdatedDate == now),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
