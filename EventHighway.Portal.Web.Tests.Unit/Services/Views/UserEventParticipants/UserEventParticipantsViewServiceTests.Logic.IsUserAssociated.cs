// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Foundations.UserEventParticipants;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.UserEventParticipants
{
    public partial class UserEventParticipantsViewServiceTests
    {
        [Fact]
        public async Task ShouldReturnTrueOnIsUserAssociatedWithParticipantWhenAssociationExistsAsync()
        {
            // given
            Guid inputUserId = GetRandomGuid();
            Guid inputParticipantId = GetRandomGuid();

            UserEventParticipant matchingAssociation =
                CreateRandomAssociation(inputUserId, inputParticipantId);

            var associations = new List<UserEventParticipant> { matchingAssociation };

            this.userEventParticipantBrokerMock.Setup(broker =>
                broker.SelectAllUserEventParticipants())
                    .Returns(associations.AsQueryable());

            // when
            bool actualResult =
                await this.userEventParticipantsViewService
                    .IsUserAssociatedWithParticipantAsync(
                        inputUserId, inputParticipantId, TestContext.Current.CancellationToken);

            // then
            actualResult.Should().BeTrue();

            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.SelectAllUserEventParticipants(), Times.Once);

            this.userEventParticipantBrokerMock.VerifyNoOtherCalls();
            this.identityBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnFalseOnIsUserAssociatedWithParticipantWhenNoAssociationAsync()
        {
            // given
            Guid inputUserId = GetRandomGuid();
            Guid inputParticipantId = GetRandomGuid();

            UserEventParticipant nonMatchingAssociation =
                CreateRandomAssociation(GetRandomGuid(), GetRandomGuid());

            var associations = new List<UserEventParticipant> { nonMatchingAssociation };

            this.userEventParticipantBrokerMock.Setup(broker =>
                broker.SelectAllUserEventParticipants())
                    .Returns(associations.AsQueryable());

            // when
            bool actualResult =
                await this.userEventParticipantsViewService
                    .IsUserAssociatedWithParticipantAsync(
                        inputUserId, inputParticipantId, TestContext.Current.CancellationToken);

            // then
            actualResult.Should().BeFalse();

            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.SelectAllUserEventParticipants(), Times.Once);

            this.userEventParticipantBrokerMock.VerifyNoOtherCalls();
            this.identityBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
