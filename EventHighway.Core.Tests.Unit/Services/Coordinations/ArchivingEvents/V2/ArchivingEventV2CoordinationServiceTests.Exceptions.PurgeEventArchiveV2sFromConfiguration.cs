// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Configurations.Purging;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnPurgeEventArchiveV2sFromConfigurationAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask purgeEventArchiveV2sTask =
                this.archivingEventV2CoordinationService
                    .PurgeEventArchiveV2sAsync(cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    purgeEventArchiveV2sTask.AsTask);

            actualException.Should().NotBeOfType<ArchivingEventV2CoordinationDependencyException>();
            actualException.Should().NotBeOfType<ArchivingEventV2CoordinationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnPurgeEventArchiveV2sFromConfigurationIfTimeoutOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var purgeConfiguration = new PurgeConfiguration();
            var batchConfiguration = new BatchConfiguration();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.", operationCanceledException);

            var timeoutArchivingEventV2CoordinationException =
                new TimeoutArchivingEventV2CoordinationException(
                    message: "Failed archiving event coordination timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedArchivingEventV2CoordinationDependencyException =
                new ArchivingEventV2CoordinationDependencyException(
                    message: "Archiving event dependency error occurred, contact support.",
                    innerException: timeoutArchivingEventV2CoordinationException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetPurgeConfiguration())
                    .Returns(purgeConfiguration);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask purgeEventArchiveV2sTask =
                this.archivingEventV2CoordinationService
                    .PurgeEventArchiveV2sAsync(randomCancellationToken);

            ArchivingEventV2CoordinationDependencyException
                actualArchivingEventV2CoordinationDependencyException =
                    await Assert.ThrowsAsync<ArchivingEventV2CoordinationDependencyException>(
                        purgeEventArchiveV2sTask.AsTask);

            // then
            actualArchivingEventV2CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEventV2CoordinationDependencyException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetPurgeConfiguration(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationDependencyException))),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnPurgeFromConfigurationIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var purgeConfiguration = new PurgeConfiguration();
            var batchConfiguration = new BatchConfiguration();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            var expectedArchivingEventV2CoordinationDependencyValidationException =
                new ArchivingEventV2CoordinationDependencyValidationException(
                    message: "Archiving event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetPurgeConfiguration())
                    .Returns(purgeConfiguration);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask purgeEventArchiveV2sTask =
                this.archivingEventV2CoordinationService
                    .PurgeEventArchiveV2sAsync(randomCancellationToken);

            ArchivingEventV2CoordinationDependencyValidationException
                actualArchivingEventV2CoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<ArchivingEventV2CoordinationDependencyValidationException>(
                        purgeEventArchiveV2sTask.AsTask);

            // then
            actualArchivingEventV2CoordinationDependencyValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2CoordinationDependencyValidationException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetPurgeConfiguration(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationDependencyValidationException))),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnPurgeFromConfigurationIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var purgeConfiguration = new PurgeConfiguration();
            var batchConfiguration = new BatchConfiguration();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            var expectedArchivingEventV2CoordinationDependencyException =
                new ArchivingEventV2CoordinationDependencyException(
                    message: "Archiving event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetPurgeConfiguration())
                    .Returns(purgeConfiguration);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask purgeEventArchiveV2sTask =
                this.archivingEventV2CoordinationService
                    .PurgeEventArchiveV2sAsync(randomCancellationToken);

            ArchivingEventV2CoordinationDependencyException
                actualArchivingEventV2CoordinationDependencyException =
                    await Assert.ThrowsAsync<ArchivingEventV2CoordinationDependencyException>(
                        purgeEventArchiveV2sTask.AsTask);

            // then
            actualArchivingEventV2CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEventV2CoordinationDependencyException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetPurgeConfiguration(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationDependencyException))),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnPurgeFromConfigurationIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var serviceException = new Exception();
            var purgeConfiguration = new PurgeConfiguration();
            var batchConfiguration = new BatchConfiguration();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            var failedArchivingEventV2CoordinationServiceException =
                new FailedArchivingEventV2CoordinationServiceException(
                    message: "Failed archiving event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedArchivingEventV2CoordinationServiceException =
                new ArchivingEventV2CoordinationServiceException(
                    message: "Archiving event service error occurred, contact support.",
                    innerException: failedArchivingEventV2CoordinationServiceException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetPurgeConfiguration())
                    .Returns(purgeConfiguration);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask purgeEventArchiveV2sTask =
                this.archivingEventV2CoordinationService
                    .PurgeEventArchiveV2sAsync(randomCancellationToken);

            ArchivingEventV2CoordinationServiceException
                actualArchivingEventV2CoordinationServiceException =
                    await Assert.ThrowsAsync<ArchivingEventV2CoordinationServiceException>(
                        purgeEventArchiveV2sTask.AsTask);

            // then
            actualArchivingEventV2CoordinationServiceException.Should()
                .BeEquivalentTo(expectedArchivingEventV2CoordinationServiceException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetPurgeConfiguration(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationServiceException))),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
