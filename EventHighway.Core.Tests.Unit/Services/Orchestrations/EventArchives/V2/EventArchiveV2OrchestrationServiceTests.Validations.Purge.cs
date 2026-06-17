// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnPurgeBatchIfBatchConfigurationIsNullAndLogItAsync()
        {
            // given
            BatchConfiguration batchConfiguration = null;

            var invalidConfigurationException =
                new InvalidEventArchiveV2OrchestrationException(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidConfigurationException.AddData(
                key: nameof(BatchConfiguration),
                values: "Required.");

            var expectedException =
            new EventArchiveV2OrchestrationValidationException(
                message: "Event archive validation error occurred, fix the errors and try again.",
                innerException: invalidConfigurationException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            // when
            ValueTask purgeTask =
                this.eventArchiveV2OrchestrationService
                    .PurgeArchivedEventV2sAsync(
                        DateTimeOffset.UtcNow,
                        CancellationToken.None);

            // then
            EventArchiveV2OrchestrationValidationException actualException =
                await Assert.ThrowsAsync<
                    EventArchiveV2OrchestrationValidationException>(
                        purgeTask.AsTask);

            actualException.Should().BeEquivalentTo(expectedException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnPurgeBatchIfOlderThanIsInvalidAndLogItAsync()
        {
            // given
            DateTimeOffset olderThan = default;

            var batchConfiguration = new BatchConfiguration
            {
                BatchSizeForBulkProcessing = 10
            };

            var invalidEventArchiveV2OrchestrationException =
                new InvalidEventArchiveV2OrchestrationException(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventArchiveV2OrchestrationException.UpsertDataList(
                key: nameof(olderThan),
                value: "Required.");

            var expectedException =
                new EventArchiveV2OrchestrationValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventArchiveV2OrchestrationException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            // when
            ValueTask purgeTask =
                this.eventArchiveV2OrchestrationService
                    .PurgeArchivedEventV2sAsync(
                        olderThan,
                        CancellationToken.None);

            // then
            var actualException =
                await Assert.ThrowsAsync<
                    EventArchiveV2OrchestrationValidationException>(
                        purgeTask.AsTask);

            actualException.Should().BeEquivalentTo(expectedException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
