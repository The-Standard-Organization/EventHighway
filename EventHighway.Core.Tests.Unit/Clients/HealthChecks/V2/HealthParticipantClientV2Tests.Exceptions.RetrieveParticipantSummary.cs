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
    public partial class HealthParticipantClientV2Tests
    {
        [Theory]
        [MemberData(nameof(ClientValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveParticipantSummaryIfValidationErrorOccursAsync(
            Xeption coordinationValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            var expectedHealthParticipantClientV2ValidationException =
                new HealthParticipantClientV2ValidationException(
                    message: "Health client validation error occurred, fix the errors and try again.",
                    innerException: coordinationValidationException.InnerException as Xeption,
                    data: (coordinationValidationException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveParticipantUsageReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(coordinationValidationException);

            // when
            ValueTask<IReadOnlyList<ParticipantUsageV2>> retrieveTask =
                this.healthParticipantClientV2.RetrieveParticipantSummaryV2Async(
                    randomPeriod, randomWindowStart, null, randomCancellationToken);

            HealthParticipantClientV2ValidationException actualException =
                await Assert.ThrowsAsync<HealthParticipantClientV2ValidationException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthParticipantClientV2ValidationException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveParticipantUsageReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ClientDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveParticipantSummaryIfDependencyErrorOccursAsync(
            Xeption coordinationDependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            var expectedHealthParticipantClientV2DependencyException =
                new HealthParticipantClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: coordinationDependencyException.InnerException as Xeption,
                    data: (coordinationDependencyException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveParticipantUsageReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(coordinationDependencyException);

            // when
            ValueTask<IReadOnlyList<ParticipantUsageV2>> retrieveTask =
                this.healthParticipantClientV2.RetrieveParticipantSummaryV2Async(
                    randomPeriod, randomWindowStart, null, randomCancellationToken);

            HealthParticipantClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthParticipantClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthParticipantClientV2DependencyException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveParticipantUsageReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveParticipantSummaryIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            var someXeption = new Xeption(message: GetRandomString());

            var expectedHealthParticipantClientV2ServiceException =
                new HealthParticipantClientV2ServiceException(
                    message: "Health client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveParticipantUsageReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask<IReadOnlyList<ParticipantUsageV2>> retrieveTask =
                this.healthParticipantClientV2.RetrieveParticipantSummaryV2Async(
                    randomPeriod, randomWindowStart, null, randomCancellationToken);

            HealthParticipantClientV2ServiceException actualException =
                await Assert.ThrowsAsync<HealthParticipantClientV2ServiceException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthParticipantClientV2ServiceException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveParticipantUsageReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveParticipantSummaryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            var operationCanceledException =
                new OperationCanceledException();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveParticipantUsageReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IReadOnlyList<ParticipantUsageV2>> retrieveTask =
                this.healthParticipantClientV2.RetrieveParticipantSummaryV2Async(
                    randomPeriod, randomWindowStart, null, randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveParticipantUsageReportV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
