// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthStatusClientV2Tests
    {
        [Theory]
        [MemberData(nameof(ClientValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveHealthSummaryIfValidationErrorOccursAsync(
            Xeption coordinationValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomTrafficPeriod();
            DateTimeOffset inputWindowStart = GetRandomDateTimeOffset();

            var expectedHealthStatusClientV2ValidationException =
                new HealthStatusClientV2ValidationException(
                    message: "Health client validation error occurred, fix the errors and try again.",
                    innerException: coordinationValidationException.InnerException as Xeption,
                    data: (coordinationValidationException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveHealthCheckItemsReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(coordinationValidationException);

            // when
            ValueTask<IReadOnlyList<HealthCheckItemV2>> retrieveTask =
                this.healthV2Client.RetrieveHealthRagStatusV2Async(
                    inputPeriod, inputWindowStart, randomCancellationToken);

            HealthStatusClientV2ValidationException actualException =
                await Assert.ThrowsAsync<HealthStatusClientV2ValidationException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthStatusClientV2ValidationException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveHealthCheckItemsReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ClientDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveHealthSummaryIfDependencyErrorOccursAsync(
            Xeption coordinationDependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomTrafficPeriod();
            DateTimeOffset inputWindowStart = GetRandomDateTimeOffset();

            var expectedHealthStatusClientV2DependencyException =
                new HealthStatusClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: coordinationDependencyException.InnerException as Xeption,
                    data: (coordinationDependencyException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveHealthCheckItemsReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(coordinationDependencyException);

            // when
            ValueTask<IReadOnlyList<HealthCheckItemV2>> retrieveTask =
                this.healthV2Client.RetrieveHealthRagStatusV2Async(
                    inputPeriod, inputWindowStart, randomCancellationToken);

            HealthStatusClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthStatusClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthStatusClientV2DependencyException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveHealthCheckItemsReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveHealthSummaryIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomTrafficPeriod();
            DateTimeOffset inputWindowStart = GetRandomDateTimeOffset();

            var someXeption = new Xeption(message: GetRandomString());

            var expectedHealthStatusClientV2ServiceException =
                new HealthStatusClientV2ServiceException(
                    message: "Health client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveHealthCheckItemsReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask<IReadOnlyList<HealthCheckItemV2>> retrieveTask =
                this.healthV2Client.RetrieveHealthRagStatusV2Async(
                    inputPeriod, inputWindowStart, randomCancellationToken);

            HealthStatusClientV2ServiceException actualException =
                await Assert.ThrowsAsync<HealthStatusClientV2ServiceException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthStatusClientV2ServiceException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveHealthCheckItemsReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveHealthSummaryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomTrafficPeriod();
            DateTimeOffset inputWindowStart = GetRandomDateTimeOffset();

            var operationCanceledException =
                new OperationCanceledException();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveHealthCheckItemsReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IReadOnlyList<HealthCheckItemV2>> retrieveTask =
                this.healthV2Client.RetrieveHealthRagStatusV2Async(
                    inputPeriod, inputWindowStart, randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveHealthCheckItemsReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
