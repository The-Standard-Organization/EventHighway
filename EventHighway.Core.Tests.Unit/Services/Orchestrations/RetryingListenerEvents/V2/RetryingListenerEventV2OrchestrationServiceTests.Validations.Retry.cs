// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.RetryingListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RetryingListenerEvents.V2
{
    public partial class RetryingListenerEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetryIfListenerEventV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 nullListenerEventV2 = null;

            var nullRetryingListenerEventV2OrchestrationException =
                new NullRetryingListenerEventV2OrchestrationException(
                    message: "Listener event is null.");

            var expectedRetryingListenerEventV2OrchestrationValidationException =
                new RetryingListenerEventV2OrchestrationValidationException(
                    message: "Retrying listener event validation error occurred, fix the errors and try again.",
                    innerException: nullRetryingListenerEventV2OrchestrationException);

            // when
            ValueTask<ListenerEventV2> retryTask =
                this.retryingListenerEventV2OrchestrationService
                    .RetryListenerEventV2Async(
                        nullListenerEventV2,
                        randomCancellationToken);

            RetryingListenerEventV2OrchestrationValidationException actualException =
                await Assert.ThrowsAsync<RetryingListenerEventV2OrchestrationValidationException>(
                    retryTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedRetryingListenerEventV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRetryingListenerEventV2OrchestrationValidationException))),
                        Times.Once);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
