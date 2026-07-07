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
    public partial class HealthLoopClientV2Tests
    {
        [Theory]
        [MemberData(nameof(ClientValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveLoopDetectionSummaryIfValidationErrorOccursAsync(
            Xeption coordinationValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            var expectedHealthLoopClientV2ValidationException =
                new HealthLoopClientV2ValidationException(
                    message: "Health client validation error occurred, fix the errors and try again.",
                    innerException: coordinationValidationException.InnerException as Xeption,
                    data: (coordinationValidationException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveLoopDetectionReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(coordinationValidationException);

            // when
            ValueTask<LoopDetectionSummaryV2> retrieveTask =
                this.healthLoopClientV2.RetrieveLoopDetectionSummaryV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken);

            HealthLoopClientV2ValidationException actualException =
                await Assert.ThrowsAsync<HealthLoopClientV2ValidationException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthLoopClientV2ValidationException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveLoopDetectionReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ClientDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveLoopDetectionSummaryIfDependencyErrorOccursAsync(
            Xeption coordinationDependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            var expectedHealthLoopClientV2DependencyException =
                new HealthLoopClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: coordinationDependencyException.InnerException as Xeption,
                    data: (coordinationDependencyException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveLoopDetectionReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(coordinationDependencyException);

            // when
            ValueTask<LoopDetectionSummaryV2> retrieveTask =
                this.healthLoopClientV2.RetrieveLoopDetectionSummaryV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken);

            HealthLoopClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthLoopClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthLoopClientV2DependencyException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveLoopDetectionReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveLoopDetectionSummaryIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            var someXeption = new Xeption(message: GetRandomString());

            var expectedHealthLoopClientV2ServiceException =
                new HealthLoopClientV2ServiceException(
                    message: "Health client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveLoopDetectionReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask<LoopDetectionSummaryV2> retrieveTask =
                this.healthLoopClientV2.RetrieveLoopDetectionSummaryV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken);

            HealthLoopClientV2ServiceException actualException =
                await Assert.ThrowsAsync<HealthLoopClientV2ServiceException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthLoopClientV2ServiceException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveLoopDetectionReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveLoopDetectionSummaryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            var operationCanceledException =
                new OperationCanceledException();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveLoopDetectionReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<LoopDetectionSummaryV2> retrieveTask =
                this.healthLoopClientV2.RetrieveLoopDetectionSummaryV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveLoopDetectionReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
