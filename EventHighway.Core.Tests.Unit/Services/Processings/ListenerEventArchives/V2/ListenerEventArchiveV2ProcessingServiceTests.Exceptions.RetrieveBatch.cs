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
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            var batchConfiguration = new BatchConfiguration
            {
                BatchSizeForBulkProcessing = 10
            };

            var expectedException =
                new ListenerEventArchiveV2ProcessingDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync())
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<List<ListenerEventArchiveV2>> retrieveBatchTask =
                this.listenerEventArchiveV2ProcessingService
                    .RetrieveNextPurgeBatchOfArchivedEventV2sAsync(
                        DateTimeOffset.UtcNow,
                        CancellationToken.None);

            ListenerEventArchiveV2ProcessingDependencyException actualException =
                await Assert.ThrowsAsync<
                    ListenerEventArchiveV2ProcessingDependencyException>(
                        retrieveBatchTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(),
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
