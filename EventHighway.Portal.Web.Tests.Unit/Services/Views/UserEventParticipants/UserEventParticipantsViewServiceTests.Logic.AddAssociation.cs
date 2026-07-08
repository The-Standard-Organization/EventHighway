// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Portal.Web.Models.Foundations.UserEventParticipants;
using EventHighway.Portal.Web.Models.Foundations.Users;
using EventHighway.Portal.Web.Models.Views.UserEventParticipants;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.UserEventParticipants
{
    public partial class UserEventParticipantsViewServiceTests
    {
        [Fact]
        public async Task ShouldAddAssociationAsync()
        {
            // given
            Guid inputUserId = GetRandomGuid();
            Guid inputParticipantId = GetRandomGuid();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            AppUser randomUser = CreateRandomUser(inputUserId);
            EventParticipantV2 randomParticipant = CreateRandomParticipant(inputParticipantId);

            var insertedAssociation = new UserEventParticipant
            {
                Id = GetRandomGuid(),
                UserId = inputUserId,
                EventParticipantId = inputParticipantId,
                CreatedDate = randomDateTimeOffset
            };

            var expectedView = new UserEventParticipantView
            {
                Id = insertedAssociation.Id,
                UserId = inputUserId,
                UserName = randomUser.UserName,
                UserEmail = randomUser.Email,
                EventParticipantId = inputParticipantId,
                EventParticipantName = randomParticipant.Name
            };

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync(randomUser);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveEventParticipantV2ByIdAsync(
                    inputParticipantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(randomParticipant);

            this.userEventParticipantBrokerMock.Setup(broker =>
                broker.SelectAllUserEventParticipants())
                    .Returns(Enumerable.Empty<UserEventParticipant>().AsQueryable());

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.userEventParticipantBrokerMock.Setup(broker =>
                broker.InsertUserEventParticipantAsync(
                    It.Is<UserEventParticipant>(association =>
                        association.UserId == inputUserId
                            && association.EventParticipantId == inputParticipantId
                            && association.CreatedDate == randomDateTimeOffset)))
                                .ReturnsAsync(insertedAssociation);

            // when
            UserEventParticipantView actualView =
                await this.userEventParticipantsViewService.AddAssociationAsync(
                    inputUserId, inputParticipantId, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveEventParticipantV2ByIdAsync(
                    inputParticipantId, It.IsAny<CancellationToken>()), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(), Times.Once);

            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.SelectAllUserEventParticipants(), Times.Once);

            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.InsertUserEventParticipantAsync(
                    It.IsAny<UserEventParticipant>()), Times.Once);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.userEventParticipantBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
