// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.RetryingListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RetryingListenerEvents.V2
{
    public partial class RetryingListenerEventV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task
            ShouldThrowDependencyValidationExceptionOnRetryIfDependencyValidationErrorOccursAndLogItAsync(
                Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 someListenerEventV2 =
                CreateRandomListenerEventV2WithNavProps();

            someListenerEventV2.EventListenerV2.PromotedProperties = null;

            var expectedRetryingListenerEventV2OrchestrationDependencyValidationException =
                new RetryingListenerEventV2OrchestrationDependencyValidationException(
                    message: "Retrying listener event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.eventCallV2ProcessingServiceMock.Setup(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EventCallV2 { IsSuccess = true });

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                .ReturnsAsync(GetRandomDateTimeOffset());

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken))
                .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<ListenerEventV2> retryTask =
                this.retryingListenerEventV2OrchestrationService
                    .RetryListenerEventV2Async(
                        someListenerEventV2,
                        randomCancellationToken);

            RetryingListenerEventV2OrchestrationDependencyValidationException actualException =
                await Assert.ThrowsAsync<RetryingListenerEventV2OrchestrationDependencyValidationException>(
                    retryTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedRetryingListenerEventV2OrchestrationDependencyValidationException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRetryingListenerEventV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
