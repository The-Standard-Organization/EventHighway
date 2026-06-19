// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveBatchOfListenerEventV2sIfValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();
            BatchConfiguration retrievedBatchConfiguration = randomBatchConfiguration;
            int inputTake = retrievedBatchConfiguration.BatchSizeForBulkProcessing;

            List<Guid> randomEventV2Ids = Enumerable
                .Range(0, GetRandomNumber())
                .Select(_ => Guid.NewGuid())
                .ToList();

            IEnumerable<Guid> inputEventV2Ids = randomEventV2Ids;

            var expectedArchivingEventV2OrchestrationDependencyValidationException =
                new ArchivingEventV2OrchestrationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(retrievedBatchConfiguration);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    inputEventV2Ids,
                    inputTake))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchOfListenerEventV2sTask =
                this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfListenerEventV2sAsync(
                        inputEventV2Ids,
                        TestContext.Current.CancellationToken);

            ArchivingEventV2OrchestrationDependencyValidationException
                actualArchivingEventV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationDependencyValidationException>(
                        retrieveBatchOfListenerEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2OrchestrationDependencyValidationException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    inputEventV2Ids,
                    inputTake),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchOfListenerEventV2sIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();
            BatchConfiguration retrievedBatchConfiguration = randomBatchConfiguration;
            int inputTake = retrievedBatchConfiguration.BatchSizeForBulkProcessing;

            List<Guid> randomEventV2Ids = Enumerable
                .Range(0, GetRandomNumber())
                .Select(_ => Guid.NewGuid())
                .ToList();

            IEnumerable<Guid> inputEventV2Ids = randomEventV2Ids;

            var expectedArchivingEventV2OrchestrationDependencyException =
                new ArchivingEventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(retrievedBatchConfiguration);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    inputEventV2Ids,
                    inputTake))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchOfListenerEventV2sTask =
                this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfListenerEventV2sAsync(
                        inputEventV2Ids,
                        TestContext.Current.CancellationToken);

            ArchivingEventV2OrchestrationDependencyException
                actualArchivingEventV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationDependencyException>(
                        retrieveBatchOfListenerEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEventV2OrchestrationDependencyException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    inputEventV2Ids,
                    inputTake),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveBatchOfListenerEventV2sIfExceptionOccursAndLogItAsync()
        {
            // given
            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();
            BatchConfiguration retrievedBatchConfiguration = randomBatchConfiguration;
            int inputTake = retrievedBatchConfiguration.BatchSizeForBulkProcessing;

            List<Guid> randomEventV2Ids = Enumerable
                .Range(0, GetRandomNumber())
                .Select(_ => Guid.NewGuid())
                .ToList();

            IEnumerable<Guid> inputEventV2Ids = randomEventV2Ids;

            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedArchivingEventV2OrchestrationServiceException =
                new FailedArchivingEventV2OrchestrationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedArchivingEventV2OrchestrationServiceException =
                new ArchivingEventV2OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedArchivingEventV2OrchestrationServiceException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(retrievedBatchConfiguration);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    inputEventV2Ids,
                    inputTake))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchOfListenerEventV2sTask =
                this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfListenerEventV2sAsync(
                        inputEventV2Ids,
                        TestContext.Current.CancellationToken);

            ArchivingEventV2OrchestrationServiceException
                actualArchivingEventV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationServiceException>(
                        retrieveBatchOfListenerEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedArchivingEventV2OrchestrationServiceException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    inputEventV2Ids,
                    inputTake),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationServiceException))),
                        Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
