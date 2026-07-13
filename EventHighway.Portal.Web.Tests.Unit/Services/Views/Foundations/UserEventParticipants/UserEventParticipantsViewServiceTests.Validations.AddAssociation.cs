// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.UserEventParticipants;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.Users;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.UserEventParticipants.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.UserEventParticipants
{
    public partial class UserEventParticipantsViewServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddAssociationWhenUserNotFoundAsync()
        {
            // given
            Guid inputUserId = GetRandomGuid();
            Guid inputParticipantId = GetRandomGuid();

            var expectedNotFoundException =
                new NotFoundUserEventParticipantsViewException();

            var expectedValidationException =
                new UserEventParticipantsViewValidationException(
                    innerException: expectedNotFoundException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync((AppUser)null);

            // when
            ValueTask<Models.Services.Views.Foundations.UserEventParticipants.UserEventParticipantView> addTask =
                this.userEventParticipantsViewService.AddAssociationAsync(
                    inputUserId, inputParticipantId, TestContext.Current.CancellationToken);

            UserEventParticipantsViewValidationException actualException =
                await Assert.ThrowsAsync<UserEventParticipantsViewValidationException>(
                    addTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedValidationException))), Times.Once);

            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.InsertUserEventParticipantAsync(
                    It.IsAny<UserEventParticipant>()), Times.Never);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.userEventParticipantBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddAssociationWhenParticipantNotFoundAsync()
        {
            // given
            Guid inputUserId = GetRandomGuid();
            Guid inputParticipantId = GetRandomGuid();

            AppUser randomUser = CreateRandomUser(inputUserId);

            var expectedNotFoundException =
                new NotFoundUserEventParticipantsViewException();

            var expectedValidationException =
                new UserEventParticipantsViewValidationException(
                    innerException: expectedNotFoundException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync(randomUser);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveEventParticipantV2ByIdAsync(
                    inputParticipantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync((EventParticipantV2)null);

            // when
            ValueTask<Models.Services.Views.Foundations.UserEventParticipants.UserEventParticipantView> addTask =
                this.userEventParticipantsViewService.AddAssociationAsync(
                    inputUserId, inputParticipantId, TestContext.Current.CancellationToken);

            UserEventParticipantsViewValidationException actualException =
                await Assert.ThrowsAsync<UserEventParticipantsViewValidationException>(
                    addTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveEventParticipantV2ByIdAsync(
                    inputParticipantId, It.IsAny<CancellationToken>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedValidationException))), Times.Once);

            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.InsertUserEventParticipantAsync(
                    It.IsAny<UserEventParticipant>()), Times.Never);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.userEventParticipantBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddAssociationWhenAlreadyExistsAsync()
        {
            // given
            Guid inputUserId = GetRandomGuid();
            Guid inputParticipantId = GetRandomGuid();

            AppUser randomUser = CreateRandomUser(inputUserId);
            EventParticipantV2 randomParticipant = CreateRandomParticipant(inputParticipantId);

            UserEventParticipant existingAssociation =
                CreateRandomAssociation(inputUserId, inputParticipantId);

            var associations = new System.Collections.Generic.List<UserEventParticipant>
            {
                existingAssociation
            };

            var expectedAlreadyExistsException =
                new AlreadyExistsUserEventParticipantsViewException();

            var expectedValidationException =
                new UserEventParticipantsViewValidationException(
                    innerException: expectedAlreadyExistsException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync(randomUser);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveEventParticipantV2ByIdAsync(
                    inputParticipantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(randomParticipant);

            this.userEventParticipantBrokerMock.Setup(broker =>
                broker.SelectAllUserEventParticipants())
                    .Returns(associations.AsQueryable());

            // when
            ValueTask<Models.Services.Views.Foundations.UserEventParticipants.UserEventParticipantView> addTask =
                this.userEventParticipantsViewService.AddAssociationAsync(
                    inputUserId, inputParticipantId, TestContext.Current.CancellationToken);

            UserEventParticipantsViewValidationException actualException =
                await Assert.ThrowsAsync<UserEventParticipantsViewValidationException>(
                    addTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveEventParticipantV2ByIdAsync(
                    inputParticipantId, It.IsAny<CancellationToken>()), Times.Once);

            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.SelectAllUserEventParticipants(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedValidationException))), Times.Once);

            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.InsertUserEventParticipantAsync(
                    It.IsAny<UserEventParticipant>()), Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(), Times.Never);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.userEventParticipantBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
