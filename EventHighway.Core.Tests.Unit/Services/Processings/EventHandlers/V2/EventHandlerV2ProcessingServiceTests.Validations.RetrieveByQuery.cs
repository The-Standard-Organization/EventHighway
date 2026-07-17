// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventHandlers.V2
{
    public partial class EventHandlerV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByQueryIfQueryIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventHandlerV2Query nullEventHandlerV2Query = null;

            var nullEventHandlerV2QueryProcessingException =
                new NullEventHandlerV2QueryProcessingException(
                    message: "Event handler query is null.");

            var expectedEventHandlerV2ProcessingValidationException =
                new EventHandlerV2ProcessingValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: nullEventHandlerV2QueryProcessingException);

            // when
            ValueTask<IReadOnlyList<EventHandlerV2>> retrieveEventHandlerV2sByQueryTask =
                this.eventHandlerV2ProcessingService.RetrieveEventHandlerV2sByQueryAsync(
                    nullEventHandlerV2Query,
                    randomCancellationToken);

            EventHandlerV2ProcessingValidationException
                actualEventHandlerV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventHandlerV2ProcessingValidationException>(
                        retrieveEventHandlerV2sByQueryTask.AsTask);

            // then
            actualEventHandlerV2ProcessingValidationException.Should()
                .BeEquivalentTo(expectedEventHandlerV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventHandlerV2ProcessingValidationException))),
                        Times.Once);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1001)]
        public async Task ShouldThrowValidationExceptionOnRetrieveByQueryIfQueryIsInvalidAndLogItAsync(
            int invalidTake)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var invalidEventHandlerV2Query = new EventHandlerV2Query
            {
                Skip = -1,
                Take = invalidTake
            };

            var invalidEventHandlerV2QueryProcessingException =
                new InvalidEventHandlerV2QueryProcessingException(
                    message: "Event handler query is invalid, fix the errors and try again.");

            invalidEventHandlerV2QueryProcessingException.AddData(
                key: nameof(EventHandlerV2Query.Skip),
                values: "Value must be zero or greater");

            invalidEventHandlerV2QueryProcessingException.AddData(
                key: nameof(EventHandlerV2Query.Take),
                values: "Value must be between 1 and 1000");

            var expectedEventHandlerV2ProcessingValidationException =
                new EventHandlerV2ProcessingValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: invalidEventHandlerV2QueryProcessingException);

            // when
            ValueTask<IReadOnlyList<EventHandlerV2>> retrieveEventHandlerV2sByQueryTask =
                this.eventHandlerV2ProcessingService.RetrieveEventHandlerV2sByQueryAsync(
                    invalidEventHandlerV2Query,
                    randomCancellationToken);

            EventHandlerV2ProcessingValidationException
                actualEventHandlerV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventHandlerV2ProcessingValidationException>(
                        retrieveEventHandlerV2sByQueryTask.AsTask);

            // then
            actualEventHandlerV2ProcessingValidationException.Should()
                .BeEquivalentTo(expectedEventHandlerV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventHandlerV2ProcessingValidationException))),
                        Times.Once);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
