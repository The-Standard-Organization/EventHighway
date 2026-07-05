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
        [Fact]
        public async Task ShouldRetryListenerEventV2SuccessfullyAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 inputListenerEventV2 =
                CreateRandomListenerEventV2WithNavProps();

            inputListenerEventV2.EventListenerV2.PromotedProperties = null;

            DateTimeOffset randomNow = GetRandomDateTimeOffset();

            var ranEventCallV2 = new EventCallV2
            {
                IsSuccess = true,
                Response = GetRandomString(),
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString()
            };

            ListenerEventV2 expectedListenerEventV2 = inputListenerEventV2.DeepClone();
            expectedListenerEventV2.DispatchedDate = randomNow;
            expectedListenerEventV2.Status = ListenerEventStatusV2.Success;
            expectedListenerEventV2.NextRetryAttemptNotBefore = null;
            expectedListenerEventV2.Response = ranEventCallV2.Response;
            expectedListenerEventV2.ResponseCode = ranEventCallV2.ResponseCode;
            expectedListenerEventV2.ResponseMessage = ranEventCallV2.ResponseMessage;
            expectedListenerEventV2.UpdatedDate = randomNow;

            ListenerEventV2 returnedListenerEventV2 = expectedListenerEventV2.DeepClone();

            var mockSequence = new MockSequence();

            this.eventCallV2ProcessingServiceMock
                .InSequence(mockSequence)
                .Setup(service => service.RunEventCallV2Async(
                    It.Is<EventCallV2>(call =>
                        call.Content == inputListenerEventV2.EventV2.Content
                        && call.HandlerId == inputListenerEventV2.EventListenerV2.HandlerId
                        && call.HandlerName == inputListenerEventV2.EventListenerV2.HandlerName
                        && call.FilterCriteria == inputListenerEventV2.EventListenerV2.FilterCriteria),
                    randomCancellationToken))
                .ReturnsAsync(ranEventCallV2);

            this.dateTimeBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.GetDateTimeOffsetAsync())
                .ReturnsAsync(randomNow);

            this.listenerEventV2ProcessingServiceMock
                .InSequence(mockSequence)
                .Setup(service => service.ModifyListenerEventV2Async(
                    It.Is<ListenerEventV2>(lev =>
                        lev.Status == ListenerEventStatusV2.Success
                        && lev.NextRetryAttemptNotBefore == null
                        && lev.DispatchedDate == randomNow
                        && lev.Response == ranEventCallV2.Response
                        && lev.ResponseCode == ranEventCallV2.ResponseCode
                        && lev.ResponseMessage == ranEventCallV2.ResponseMessage
                        && lev.UpdatedDate == randomNow),
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

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetryListenerEventV2WithFibonacciBackoffWhenEventCallFailsAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 inputListenerEventV2 =
                CreateRandomListenerEventV2WithNavProps();

            inputListenerEventV2.EventListenerV2.PromotedProperties = null;
            inputListenerEventV2.RetryAttemptsAllowed = 15;
            inputListenerEventV2.RemainingRetryAttempts = 10;

            DateTimeOffset randomNow = GetRandomDateTimeOffset();

            var retryConfiguration = new RetryConfiguration
            {
                RetryAttemptsAllowed = 15,
                RetryBackoffMaxMinutes = 180,
                DeadAfterMinutes = 180
            };

            // after decrement Remaining = 9, attemptNumber = 15 - 9 = 6, Fib(6) = 8 (< cap 180)
            int expectedRemainingRetryAttempts = 9;
            DateTimeOffset expectedNextRetryAttemptNotBefore = randomNow.AddMinutes(8);

            var ranEventCallV2 = new EventCallV2
            {
                IsSuccess = false,
                Response = GetRandomString(),
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString()
            };

            ListenerEventV2 returnedListenerEventV2 = inputListenerEventV2.DeepClone();

            var mockSequence = new MockSequence();

            this.eventCallV2ProcessingServiceMock
                .InSequence(mockSequence)
                .Setup(service => service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    randomCancellationToken))
                .ReturnsAsync(ranEventCallV2);

            this.dateTimeBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.GetDateTimeOffsetAsync())
                .ReturnsAsync(randomNow);

            this.configurationBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.GetRetryConfiguration())
                .Returns(retryConfiguration);

            this.listenerEventV2ProcessingServiceMock
                .InSequence(mockSequence)
                .Setup(service => service.ModifyListenerEventV2Async(
                    It.Is<ListenerEventV2>(lev =>
                        lev.Status == ListenerEventStatusV2.Error
                        && lev.RemainingRetryAttempts == expectedRemainingRetryAttempts
                        && lev.RetryAttemptsAllowed == 15
                        && lev.NextRetryAttemptNotBefore == expectedNextRetryAttemptNotBefore
                        && lev.DispatchedDate == randomNow
                        && lev.Response == ranEventCallV2.Response
                        && lev.ResponseCode == ranEventCallV2.ResponseCode
                        && lev.ResponseMessage == ranEventCallV2.ResponseMessage
                        && lev.UpdatedDate == randomNow),
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

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetRetryConfiguration(),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldFloorFibonacciBackoffAtOneMinuteWhenBackoffMaxMinutesIsZeroAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 inputListenerEventV2 =
                CreateRandomListenerEventV2WithNavProps();

            inputListenerEventV2.EventListenerV2.PromotedProperties = null;
            inputListenerEventV2.RetryAttemptsAllowed = 15;
            inputListenerEventV2.RemainingRetryAttempts = 10;

            DateTimeOffset randomNow = GetRandomDateTimeOffset();

            var retryConfiguration = new RetryConfiguration
            {
                RetryAttemptsAllowed = 15,
                RetryBackoffMaxMinutes = 0,
                DeadAfterMinutes = 180
            };

            // a non-positive backoff cap must floor to 1 minute so the gate always advances
            int expectedRemainingRetryAttempts = 9;
            DateTimeOffset expectedNextRetryAttemptNotBefore = randomNow.AddMinutes(1);

            var ranEventCallV2 = new EventCallV2
            {
                IsSuccess = false,
                Response = GetRandomString(),
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString()
            };

            ListenerEventV2 returnedListenerEventV2 = inputListenerEventV2.DeepClone();

            var mockSequence = new MockSequence();

            this.eventCallV2ProcessingServiceMock
                .InSequence(mockSequence)
                .Setup(service => service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    randomCancellationToken))
                .ReturnsAsync(ranEventCallV2);

            this.dateTimeBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.GetDateTimeOffsetAsync())
                .ReturnsAsync(randomNow);

            this.configurationBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.GetRetryConfiguration())
                .Returns(retryConfiguration);

            this.listenerEventV2ProcessingServiceMock
                .InSequence(mockSequence)
                .Setup(service => service.ModifyListenerEventV2Async(
                    It.Is<ListenerEventV2>(lev =>
                        lev.Status == ListenerEventStatusV2.Error
                        && lev.RemainingRetryAttempts == expectedRemainingRetryAttempts
                        && lev.RetryAttemptsAllowed == 15
                        && lev.NextRetryAttemptNotBefore == expectedNextRetryAttemptNotBefore
                        && lev.DispatchedDate == randomNow
                        && lev.Response == ranEventCallV2.Response
                        && lev.ResponseCode == ranEventCallV2.ResponseCode
                        && lev.ResponseMessage == ranEventCallV2.ResponseMessage
                        && lev.UpdatedDate == randomNow),
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

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetRetryConfiguration(),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetryListenerEventV2AsErrorWhenDeliveryExceptionThrownAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 inputListenerEventV2 =
                CreateRandomListenerEventV2WithNavProps();

            inputListenerEventV2.EventListenerV2.PromotedProperties = null;
            inputListenerEventV2.RetryAttemptsAllowed = 15;
            inputListenerEventV2.RemainingRetryAttempts = 10;

            DateTimeOffset randomNow = GetRandomDateTimeOffset();
            var deliveryException = new Exception();

            var retryConfiguration = new RetryConfiguration
            {
                RetryAttemptsAllowed = 15,
                RetryBackoffMaxMinutes = 180,
                DeadAfterMinutes = 180
            };

            // after decrement Remaining = 9, attemptNumber = 15 - 9 = 6, Fib(6) = 8 (< cap 180)
            int expectedRemainingRetryAttempts = 9;
            DateTimeOffset expectedNextRetryAttemptNotBefore = randomNow.AddMinutes(8);

            ListenerEventV2 returnedListenerEventV2 = inputListenerEventV2.DeepClone();

            var mockSequence = new MockSequence();

            this.eventCallV2ProcessingServiceMock
                .InSequence(mockSequence)
                .Setup(service => service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    randomCancellationToken))
                .ThrowsAsync(deliveryException);

            this.loggingBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.LogErrorAsync(deliveryException))
                .Returns(ValueTask.CompletedTask);

            this.dateTimeBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.GetDateTimeOffsetAsync())
                .ReturnsAsync(randomNow);

            this.configurationBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.GetRetryConfiguration())
                .Returns(retryConfiguration);

            this.listenerEventV2ProcessingServiceMock
                .InSequence(mockSequence)
                .Setup(service => service.ModifyListenerEventV2Async(
                    It.Is<ListenerEventV2>(lev =>
                        lev.Status == ListenerEventStatusV2.Error
                        && lev.RemainingRetryAttempts == expectedRemainingRetryAttempts
                        && lev.RetryAttemptsAllowed == 15
                        && lev.NextRetryAttemptNotBefore == expectedNextRetryAttemptNotBefore
                        && lev.DispatchedDate == randomNow
                        && lev.Response == deliveryException.Message
                        && lev.UpdatedDate == randomNow),
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

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(deliveryException),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetRetryConfiguration(),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
