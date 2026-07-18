// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ListenerEvents.V2
{
    public partial class ListenerEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByQueryIfQueryIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2Query nullListenerEventV2Query = null;

            var nullListenerEventV2QueryOrchestrationException =
                new NullListenerEventV2QueryOrchestrationException(
                    message: "Listener event query is null.");

            var expectedListenerEventV2OrchestrationValidationException =
                new ListenerEventV2OrchestrationValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventV2QueryOrchestrationException);

            // when
            ValueTask<IReadOnlyList<ListenerEventV2>> retrieveListenerEventV2sByQueryTask =
                this.listenerEventV2OrchestrationService.RetrieveListenerEventV2sByQueryAsync(
                    nullListenerEventV2Query,
                    randomCancellationToken);

            ListenerEventV2OrchestrationValidationException
                actualListenerEventV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<ListenerEventV2OrchestrationValidationException>(
                        retrieveListenerEventV2sByQueryTask.AsTask);

            // then
            actualListenerEventV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2OrchestrationValidationException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
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

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            var invalidListenerEventV2Query = new ListenerEventV2Query
            {
                Skip = -1,
                Take = invalidTake,
                CreatedFrom = randomDateTimeOffset,
                CreatedTo = randomDateTimeOffset.AddMinutes(-1)
            };

            var invalidListenerEventV2QueryOrchestrationException =
                new InvalidListenerEventV2QueryOrchestrationException(
                    message: "Listener event query is invalid, fix the errors and try again.");

            invalidListenerEventV2QueryOrchestrationException.AddData(
                key: nameof(ListenerEventV2Query.Skip),
                values: "Value must be zero or greater");

            invalidListenerEventV2QueryOrchestrationException.AddData(
                key: nameof(ListenerEventV2Query.Take),
                values: "Value must be between 1 and 1000");

            invalidListenerEventV2QueryOrchestrationException.AddData(
                key: nameof(ListenerEventV2Query.CreatedTo),
                values: $"Date must be after {nameof(ListenerEventV2Query.CreatedFrom)}");

            var expectedListenerEventV2OrchestrationValidationException =
                new ListenerEventV2OrchestrationValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV2QueryOrchestrationException);

            // when
            ValueTask<IReadOnlyList<ListenerEventV2>> retrieveListenerEventV2sByQueryTask =
                this.listenerEventV2OrchestrationService.RetrieveListenerEventV2sByQueryAsync(
                    invalidListenerEventV2Query,
                    randomCancellationToken);

            ListenerEventV2OrchestrationValidationException
                actualListenerEventV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<ListenerEventV2OrchestrationValidationException>(
                        retrieveListenerEventV2sByQueryTask.AsTask);

            // then
            actualListenerEventV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2OrchestrationValidationException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
