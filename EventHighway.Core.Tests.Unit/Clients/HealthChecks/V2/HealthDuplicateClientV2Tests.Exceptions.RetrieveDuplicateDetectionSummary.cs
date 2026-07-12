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
    public partial class HealthDuplicateClientV2Tests
    {
        [Theory]
        [MemberData(nameof(ClientValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveDuplicateDetectionSummaryIfValidationErrorOccursAsync(
            Xeption coordinationValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            var expectedHealthDuplicateClientV2ValidationException =
                new HealthDuplicateClientV2ValidationException(
                    message: "Health client validation error occurred, fix the errors and try again.",
                    innerException: coordinationValidationException.InnerException as Xeption,
                    data: (coordinationValidationException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveDuplicateReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(coordinationValidationException);

            // when
            ValueTask<DuplicateDetectionSummaryV2> retrieveTask =
                this.healthDuplicateClientV2.RetrieveDuplicateDetectionSummaryV2Async(
                    randomPeriod, randomWindowStart, null, randomCancellationToken);

            HealthDuplicateClientV2ValidationException actualException =
                await Assert.ThrowsAsync<HealthDuplicateClientV2ValidationException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthDuplicateClientV2ValidationException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveDuplicateReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ClientDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveDuplicateDetectionSummaryIfDependencyErrorOccursAsync(
            Xeption coordinationDependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            var expectedHealthDuplicateClientV2DependencyException =
                new HealthDuplicateClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: coordinationDependencyException.InnerException as Xeption,
                    data: (coordinationDependencyException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveDuplicateReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(coordinationDependencyException);

            // when
            ValueTask<DuplicateDetectionSummaryV2> retrieveTask =
                this.healthDuplicateClientV2.RetrieveDuplicateDetectionSummaryV2Async(
                    randomPeriod, randomWindowStart, null, randomCancellationToken);

            HealthDuplicateClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthDuplicateClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthDuplicateClientV2DependencyException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveDuplicateReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveDuplicateDetectionSummaryIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            var someXeption = new Xeption(message: GetRandomString());

            var expectedHealthDuplicateClientV2ServiceException =
                new HealthDuplicateClientV2ServiceException(
                    message: "Health client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveDuplicateReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask<DuplicateDetectionSummaryV2> retrieveTask =
                this.healthDuplicateClientV2.RetrieveDuplicateDetectionSummaryV2Async(
                    randomPeriod, randomWindowStart, null, randomCancellationToken);

            HealthDuplicateClientV2ServiceException actualException =
                await Assert.ThrowsAsync<HealthDuplicateClientV2ServiceException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthDuplicateClientV2ServiceException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveDuplicateReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveDuplicateDetectionSummaryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            var operationCanceledException =
                new OperationCanceledException();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveDuplicateReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<DuplicateDetectionSummaryV2> retrieveTask =
                this.healthDuplicateClientV2.RetrieveDuplicateDetectionSummaryV2Async(
                    randomPeriod, randomWindowStart, null, randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveDuplicateReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
