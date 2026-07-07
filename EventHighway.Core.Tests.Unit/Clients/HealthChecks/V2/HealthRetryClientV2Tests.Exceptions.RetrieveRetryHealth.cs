// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthRetryClientV2Tests
    {
        [Theory]
        [MemberData(nameof(ClientValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveRetryHealthIfValidationErrorOccursAsync(
            Xeption coordinationValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            var expectedHealthRetryClientV2ValidationException =
                new HealthRetryClientV2ValidationException(
                    message: "Health client validation error occurred, fix the errors and try again.",
                    innerException: coordinationValidationException.InnerException as Xeption,
                    data: (coordinationValidationException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveRetryReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(coordinationValidationException);

            // when
            ValueTask<RetryHealthSummaryV2> retrieveTask =
                this.healthRetryClientV2.RetrieveRetryHealthV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken);

            HealthRetryClientV2ValidationException actualException =
                await Assert.ThrowsAsync<HealthRetryClientV2ValidationException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthRetryClientV2ValidationException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveRetryReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ClientDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveRetryHealthIfDependencyErrorOccursAsync(
            Xeption coordinationDependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            var expectedHealthRetryClientV2DependencyException =
                new HealthRetryClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: coordinationDependencyException.InnerException as Xeption,
                    data: (coordinationDependencyException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveRetryReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(coordinationDependencyException);

            // when
            ValueTask<RetryHealthSummaryV2> retrieveTask =
                this.healthRetryClientV2.RetrieveRetryHealthV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken);

            HealthRetryClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthRetryClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthRetryClientV2DependencyException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveRetryReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveRetryHealthIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            var someXeption = new Xeption(message: GetRandomString());

            var expectedHealthRetryClientV2ServiceException =
                new HealthRetryClientV2ServiceException(
                    message: "Health client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveRetryReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask<RetryHealthSummaryV2> retrieveTask =
                this.healthRetryClientV2.RetrieveRetryHealthV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken);

            HealthRetryClientV2ServiceException actualException =
                await Assert.ThrowsAsync<HealthRetryClientV2ServiceException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthRetryClientV2ServiceException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveRetryReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveRetryHealthAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            var operationCanceledException =
                new OperationCanceledException();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveRetryReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<RetryHealthSummaryV2> retrieveTask =
                this.healthRetryClientV2.RetrieveRetryHealthV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveRetryReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
