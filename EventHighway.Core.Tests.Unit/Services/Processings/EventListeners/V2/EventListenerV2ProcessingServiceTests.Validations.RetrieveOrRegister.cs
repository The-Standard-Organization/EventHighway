// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventListeners.V2
{
    public partial class EventListenerV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveOrRegisterIfEventListenerV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventListenerV2 nullEventListenerV2 = null;

            var nullEventListenerV2ProcessingException =
                new NullEventListenerV2ProcessingException(
                    message: "Event listener is null.");

            var expectedEventListenerV2ProcessingValidationException =
                new EventListenerV2ProcessingValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: nullEventListenerV2ProcessingException);

            // when
            ValueTask<EventListenerV2> retrieveOrRegisterEventListenerV2Task =
                this.eventListenerV2ProcessingService.RetrieveOrRegisterEventListenerV2Async(
                    nullEventListenerV2,
                    randomCancellationToken);

            EventListenerV2ProcessingValidationException
                actualEventListenerV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventListenerV2ProcessingValidationException>(
                        retrieveOrRegisterEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ProcessingValidationException.Should().BeEquivalentTo(
                expectedEventListenerV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ProcessingValidationException))),
                        Times.Once);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveOrRegisterIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventListenerV2 invalidEventListenerV2 =
                CreateRandomEventListenerV2();

            invalidEventListenerV2.Id = Guid.Empty;

            var invalidEventListenerV2ProcessingException =
                new InvalidEventListenerV2ProcessingException(
                    message: "Event listener is invalid, fix the errors and try again.");

            invalidEventListenerV2ProcessingException.AddData(
                key: nameof(EventListenerV2.Id),
                values: "Required");

            var expectedEventListenerV2ProcessingValidationException =
                new EventListenerV2ProcessingValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV2ProcessingException);

            // when
            ValueTask<EventListenerV2> retrieveOrRegisterEventListenerV2Task =
                this.eventListenerV2ProcessingService.RetrieveOrRegisterEventListenerV2Async(
                    invalidEventListenerV2,
                    randomCancellationToken);

            EventListenerV2ProcessingValidationException
                actualEventListenerV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventListenerV2ProcessingValidationException>(
                        retrieveOrRegisterEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ProcessingValidationException.Should().BeEquivalentTo(
                expectedEventListenerV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ProcessingValidationException))),
                        Times.Once);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
