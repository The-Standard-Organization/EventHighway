// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V1
{
    public partial class ListenerEventV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidListenerEventV1Id = Guid.Empty;

            var invalidListenerEventV1Exception =
                new InvalidListenerEventV1Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV1Exception.AddData(
                key: nameof(ListenerEventV1.Id),
                values: "Required");

            var expectedListenerEventV1ValidationException =
                new ListenerEventV1ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV1Exception);

            // when
            ValueTask<ListenerEventV1> removeListenerEventV1ByIdTask =
                this.listenerEventV1Service.RemoveListenerEventV1ByIdAsync(
                    invalidListenerEventV1Id);

            ListenerEventV1ValidationException actualListenerEventV1ValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ValidationException>(
                    removeListenerEventV1ByIdTask.AsTask);

            // then
            actualListenerEventV1ValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV1ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventByIdV1Async(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfListenerEventV1IsNotFoundAndLogItAsync()
        {
            // given
            Guid nonExistingListenerEventV1Id = GetRandomId();
            ListenerEventV1 nullListenerEventV1 = null;

            var notFoundListenerEventV1Exception =
                new NotFoundListenerEventV1Exception(
                    message: $"Could not find listener event with id: {nonExistingListenerEventV1Id}.");

            var expectedListenerEventV1ValidationException =
                new ListenerEventV1ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: notFoundListenerEventV1Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectListenerEventByIdV1Async(It.IsAny<Guid>()))
                    .ReturnsAsync(nullListenerEventV1);

            // when
            ValueTask<ListenerEventV1> removeListenerEventV1ByIdTask =
                this.listenerEventV1Service.RemoveListenerEventV1ByIdAsync(nonExistingListenerEventV1Id);

            ListenerEventV1ValidationException actualListenerEventV1ValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ValidationException>(
                    removeListenerEventV1ByIdTask.AsTask);

            // then
            actualListenerEventV1ValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV1ValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventByIdV1Async(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteListenerEventV1Async(
                    It.IsAny<ListenerEventV1>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
