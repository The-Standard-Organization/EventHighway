// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V2
{
    public partial class EventListenerV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByEventAddressIdByQueryIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidEventAddressId = Guid.Empty;
            var someEventListenerV2Query = new EventListenerV2Query();

            var invalidEventListenerV2OrchestrationException =
                new InvalidEventListenerV2OrchestrationException(
                    message: "Event listener is invalid, fix the errors and try again.");

            invalidEventListenerV2OrchestrationException.AddData(
                key: nameof(EventListenerV2.EventAddressV2Id),
                values: "Required");

            var expectedEventListenerV2OrchestrationValidationException =
                new EventListenerV2OrchestrationValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV2OrchestrationException);

            // when
            ValueTask<IReadOnlyList<EventListenerV2>> retrieveEventListenerV2sByQueryTask =
                this.eventListenerV2OrchestrationService
                    .RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                        invalidEventAddressId,
                        someEventListenerV2Query,
                        randomCancellationToken);

            EventListenerV2OrchestrationValidationException
                actualEventListenerV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationValidationException>(
                        retrieveEventListenerV2sByQueryTask.AsTask);

            // then
            actualEventListenerV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationValidationException))),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByEventAddressIdByQueryIfQueryIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid inputEventAddressId = GetRandomId();
            EventListenerV2Query nullEventListenerV2Query = null;

            var nullEventListenerV2QueryOrchestrationException =
                new NullEventListenerV2QueryOrchestrationException(
                    message: "Event listener query is null.");

            var expectedEventListenerV2OrchestrationValidationException =
                new EventListenerV2OrchestrationValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: nullEventListenerV2QueryOrchestrationException);

            // when
            ValueTask<IReadOnlyList<EventListenerV2>> retrieveEventListenerV2sByQueryTask =
                this.eventListenerV2OrchestrationService
                    .RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                        inputEventAddressId,
                        nullEventListenerV2Query,
                        randomCancellationToken);

            EventListenerV2OrchestrationValidationException
                actualEventListenerV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationValidationException>(
                        retrieveEventListenerV2sByQueryTask.AsTask);

            // then
            actualEventListenerV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationValidationException))),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1001)]
        public async Task ShouldThrowValidationExceptionOnRetrieveByEventAddressIdByQueryIfQueryIsInvalidAndLogItAsync(
            int invalidTake)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid inputEventAddressId = GetRandomId();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            var invalidEventListenerV2Query = new EventListenerV2Query
            {
                Skip = -1,
                Take = invalidTake,
                CreatedFrom = randomDateTimeOffset,
                CreatedTo = randomDateTimeOffset.AddMinutes(-1)
            };

            var invalidEventListenerV2QueryOrchestrationException =
                new InvalidEventListenerV2QueryOrchestrationException(
                    message: "Event listener query is invalid, fix the errors and try again.");

            invalidEventListenerV2QueryOrchestrationException.AddData(
                key: nameof(EventListenerV2Query.Skip),
                values: "Value must be zero or greater");

            invalidEventListenerV2QueryOrchestrationException.AddData(
                key: nameof(EventListenerV2Query.Take),
                values: "Value must be between 1 and 1000");

            invalidEventListenerV2QueryOrchestrationException.AddData(
                key: nameof(EventListenerV2Query.CreatedTo),
                values: $"Date must be after {nameof(EventListenerV2Query.CreatedFrom)}");

            var expectedEventListenerV2OrchestrationValidationException =
                new EventListenerV2OrchestrationValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV2QueryOrchestrationException);

            // when
            ValueTask<IReadOnlyList<EventListenerV2>> retrieveEventListenerV2sByQueryTask =
                this.eventListenerV2OrchestrationService
                    .RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                        inputEventAddressId,
                        invalidEventListenerV2Query,
                        randomCancellationToken);

            EventListenerV2OrchestrationValidationException
                actualEventListenerV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationValidationException>(
                        retrieveEventListenerV2sByQueryTask.AsTask);

            // then
            actualEventListenerV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationValidationException))),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
