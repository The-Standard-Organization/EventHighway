// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveBatchIfBatchConfigurationIsNullAndLogItAsync()
        {
            // given
            BatchConfiguration batchConfiguration = null;

            var invalidConfigurationException =
                new InvalidListenerEventArchiveV2ProcessingException(
                    message: "Listener event archive is invalid, fix the errors and try again.");

            invalidConfigurationException.AddData(
                key: nameof(BatchConfiguration),
                values: "Required.");

            var expectedException =
                new ListenerEventArchiveV2ProcessingValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidConfigurationException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            // when
            ValueTask<List<ListenerEventArchiveV2>> retrieveBatchTask =
                this.listenerEventArchiveV2ProcessingService
                    .RetrieveNextPurgeBatchOfArchivedEventV2sAsync(
                        DateTimeOffset.UtcNow,
                        CancellationToken.None);

            // then
            ListenerEventArchiveV2ProcessingValidationException actualException =
                await Assert.ThrowsAsync<
                    ListenerEventArchiveV2ProcessingValidationException>(
                        retrieveBatchTask.AsTask);

            actualException.Should().BeEquivalentTo(expectedException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveBatchIfOlderThanIsInvalidAndLogItAsync()
        {
            // given
            DateTimeOffset olderThan = default;

            var batchConfiguration = new BatchConfiguration
            {
                BatchSizeForBulkProcessing = 10
            };

            var invalidListenerEventArchiveV2ProcessingException =
                new InvalidListenerEventArchiveV2ProcessingException(
                    message: "Listener event archive is invalid, fix the errors and try again.");

            invalidListenerEventArchiveV2ProcessingException.UpsertDataList(
                key: nameof(olderThan),
                value: "Required.");

            var expectedException =
                new ListenerEventArchiveV2ProcessingValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventArchiveV2ProcessingException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            // when
            ValueTask<List<ListenerEventArchiveV2>> retrieveBatchTask =
                this.listenerEventArchiveV2ProcessingService
                    .RetrieveNextPurgeBatchOfArchivedEventV2sAsync(
                        olderThan,
                        CancellationToken.None);

            // then
            var actualException =
                await Assert.ThrowsAsync<
                    ListenerEventArchiveV2ProcessingValidationException>(
                        retrieveBatchTask.AsTask);

            actualException.Should().BeEquivalentTo(expectedException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
