// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.Retries;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RetryingListenerEvents.V2
{
    public partial class RetryingListenerEventV2OrchestrationServiceTests
    {
        [Theory]

        // Fibonacci sequence with allowed = 15, cap = 180 (attemptNumber = allowed - remainingAfterDecrement)
        [InlineData(15, 15, 180, 1)]   // attempt 1  -> Fib(1)  = 1
        [InlineData(15, 14, 180, 1)]   // attempt 2  -> Fib(2)  = 1
        [InlineData(15, 13, 180, 2)]   // attempt 3  -> Fib(3)  = 2
        [InlineData(15, 12, 180, 3)]   // attempt 4  -> Fib(4)  = 3
        [InlineData(15, 11, 180, 5)]   // attempt 5  -> Fib(5)  = 5
        [InlineData(15, 10, 180, 8)]   // attempt 6  -> Fib(6)  = 8
        [InlineData(15, 9, 180, 13)]   // attempt 7  -> Fib(7)  = 13
        [InlineData(15, 8, 180, 21)]   // attempt 8  -> Fib(8)  = 21
        [InlineData(15, 7, 180, 34)]   // attempt 9  -> Fib(9)  = 34
        [InlineData(15, 6, 180, 55)]   // attempt 10 -> Fib(10) = 55
        [InlineData(15, 5, 180, 89)]   // attempt 11 -> Fib(11) = 89
        [InlineData(15, 4, 180, 144)]  // attempt 12 -> Fib(12) = 144
        [InlineData(15, 3, 180, 180)]  // attempt 13 -> Fib(13) = 233 capped at 180
        [InlineData(15, 2, 180, 180)]  // attempt 14 -> Fib(14) = 377 capped at 180

        // cap engagement with a small cap
        [InlineData(15, 10, 10, 8)]    // attempt 6  -> Fib(6)  = 8  (< cap 10)
        [InlineData(15, 9, 10, 10)]    // attempt 7  -> Fib(7)  = 13 capped at 10

        // continuation after extend (allowed grown to 30, high attemptNumber stays capped, overflow-safe)
        [InlineData(30, 10, 180, 180)] // attempt 21 -> Fib(21) huge, capped at 180
        public async Task ShouldApplyFibonacciBackoffWithCapWhenRetryFailsAsync(
            int retryAttemptsAllowed,
            int remainingRetryAttempts,
            int backoffMaxMinutes,
            int expectedDelayMinutes)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 inputListenerEventV2 =
                CreateRandomListenerEventV2WithNavProps();

            inputListenerEventV2.EventListenerV2.PromotedProperties = null;
            inputListenerEventV2.RetryAttemptsAllowed = retryAttemptsAllowed;
            inputListenerEventV2.RemainingRetryAttempts = remainingRetryAttempts;

            DateTimeOffset randomNow = GetRandomDateTimeOffset();

            var retryConfiguration = new RetryConfiguration
            {
                RetryAttemptsAllowed = retryAttemptsAllowed,
                RetryBackoffMaxMinutes = backoffMaxMinutes,
                DeadAfterMinutes = 180
            };

            int expectedRemainingRetryAttempts = remainingRetryAttempts - 1;
            DateTimeOffset expectedNextRetryAttemptNotBefore = randomNow.AddMinutes(expectedDelayMinutes);

            var ranEventCallV2 = new EventCallV2 { IsSuccess = false };

            ListenerEventV2 returnedListenerEventV2 = inputListenerEventV2.DeepClone();

            this.eventCallV2ProcessingServiceMock.Setup(service =>
                service.RunEventCallV2Async(It.IsAny<EventCallV2>(), randomCancellationToken))
                    .ReturnsAsync(ranEventCallV2);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomNow);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetRetryConfiguration())
                    .Returns(retryConfiguration);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventV2Async(
                    It.Is<ListenerEventV2>(lev =>
                        lev.Status == ListenerEventStatusV2.Error
                        && lev.RemainingRetryAttempts == expectedRemainingRetryAttempts
                        && lev.NextRetryAttemptNotBefore == expectedNextRetryAttemptNotBefore
                        && lev.DispatchedDate == randomNow),
                    randomCancellationToken))
                        .ReturnsAsync(returnedListenerEventV2);

            // when
            ListenerEventV2 actualListenerEventV2 =
                await this.retryingListenerEventV2OrchestrationService
                    .RetryListenerEventV2Async(
                        inputListenerEventV2,
                        randomCancellationToken);

            // then
            actualListenerEventV2.Should().BeEquivalentTo(returnedListenerEventV2);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.Is<ListenerEventV2>(lev =>
                        lev.RemainingRetryAttempts == expectedRemainingRetryAttempts
                        && lev.NextRetryAttemptNotBefore == expectedNextRetryAttemptNotBefore),
                    randomCancellationToken),
                        Times.Once);
        }
    }
}
