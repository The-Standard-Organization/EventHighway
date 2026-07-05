// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ListenerEvents.V2
{
    public partial class ListenerEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnResetRetriesByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidListenerEventV2Id = Guid.Empty;

            var invalidListenerEventV2OrchestrationException =
                new InvalidListenerEventV2OrchestrationException(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV2OrchestrationException.AddData(
                key: nameof(ListenerEventV2.Id),
                values: "Required");

            var expectedListenerEventV2OrchestrationValidationException =
                new ListenerEventV2OrchestrationValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV2OrchestrationException);

            // when
            ValueTask<ListenerEventV2> resetRetriesByIdTask =
                this.listenerEventV2OrchestrationService
                    .ResetRetriesForListenerEventV2ByIdAsync(
                        invalidListenerEventV2Id,
                        randomCancellationToken);

            ListenerEventV2OrchestrationValidationException
                actualListenerEventV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<ListenerEventV2OrchestrationValidationException>(
                        resetRetriesByIdTask.AsTask);

            // then
            actualListenerEventV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2OrchestrationValidationException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ResetRetriesForListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
