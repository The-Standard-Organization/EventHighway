// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldArchiveQuarantinedEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var mockSequence = new MockSequence();

            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();

            Guid quarantinedEventV2Id = GetRandomId();

            var quarantinedEventV2 = new EventV2
            {
                Id = quarantinedEventV2Id,
                Content = GetRandomString(),
                EventName = GetRandomString(),
                Type = GetRandomEnum<EventTypeV2>(),
                Status = EventStatusV2.Quarantined,
                CreatedDate = GetRandomDateTimeOffset(),
                UpdatedDate = GetRandomDateTimeOffset(),
                ScheduledDate = GetRandomDateTimeOffset(),
                EventAddressV2Id = GetRandomId()
            };

            List<EventV2> retrievedQuarantinedEventV2s =
                new List<EventV2> { quarantinedEventV2 };

            var expectedEventArchiveV2 = new EventArchiveV2
            {
                Id = quarantinedEventV2.Id,
                Content = quarantinedEventV2.Content,
                EventName = quarantinedEventV2.EventName,
                Type = (EventArchiveTypeV2)quarantinedEventV2.Type,
                Status = (EventArchiveStatusV2)quarantinedEventV2.Status,
                CreatedDate = quarantinedEventV2.CreatedDate,
                UpdatedDate = quarantinedEventV2.CreatedDate,
                ScheduledDate = quarantinedEventV2.ScheduledDate,
                EventAddressV2Id = quarantinedEventV2.EventAddressV2Id
            };

            List<EventArchiveV2> expectedEventArchiveV2s =
                new List<EventArchiveV2> { expectedEventArchiveV2 };

            // Step 1
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfQuarantinedEventV2sAsync(randomCancellationToken))
                        .ReturnsAsync((IEnumerable<EventV2>)retrievedQuarantinedEventV2s);

            // Step 2
            this.eventArchiveV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkAddEventArchiveV2sAsync(
                        It.Is(SameEventArchiveV2sAs(expectedEventArchiveV2s)),
                        randomCancellationToken))
                            .ReturnsAsync(expectedEventArchiveV2s);

            // Step 3
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkRemoveEventV2sAsync(
                        It.Is(SameEventV2sAs(retrievedQuarantinedEventV2s)),
                        randomCancellationToken))
                            .Returns(ValueTask.CompletedTask);

            // Step 4
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfQuarantinedEventV2sAsync(randomCancellationToken))
                        .ReturnsAsync(Enumerable.Empty<EventV2>());

            this.configurationBrokerMock
                .InSequence(mockSequence).Setup(broker =>
                    broker.GetBatchConfiguration())
                        .Returns(randomBatchConfiguration);

            // Step 5
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken))
                        .ReturnsAsync(Enumerable.Empty<EventV2>());

            // when
            await this.archivingEventV2CoordinationService
                .ArchiveEventV2sAsync(randomCancellationToken);

            // then
            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfQuarantinedEventV2sAsync(randomCancellationToken),
                    Times.Exactly(2));

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.BulkAddEventArchiveV2sAsync(
                    It.Is(SameEventArchiveV2sAs(expectedEventArchiveV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.BulkRemoveEventV2sAsync(
                    It.Is(SameEventV2sAs(retrievedQuarantinedEventV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldCopyContentHashAndParticipantAttributionWhenArchivingQuarantinedEventV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var mockSequence = new MockSequence();

            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();

            var quarantinedEventV2 = new EventV2
            {
                Id = GetRandomId(),
                Content = GetRandomString(),
                EventName = GetRandomString(),
                ContentHash = GetRandomString(),
                Type = GetRandomEnum<EventTypeV2>(),
                Status = EventStatusV2.Quarantined,
                CreatedDate = GetRandomDateTimeOffset(),
                UpdatedDate = GetRandomDateTimeOffset(),
                ScheduledDate = GetRandomDateTimeOffset(),
                EventAddressV2Id = GetRandomId(),
                EventParticipantV2Id = GetRandomId(),
                EventParticipantV2Secret = GetRandomString()
            };

            List<EventV2> retrievedQuarantinedEventV2s =
                new List<EventV2> { quarantinedEventV2 };

            var expectedEventArchiveV2 = new EventArchiveV2
            {
                Id = quarantinedEventV2.Id,
                Content = quarantinedEventV2.Content,
                EventName = quarantinedEventV2.EventName,
                ContentHash = quarantinedEventV2.ContentHash,
                Type = (EventArchiveTypeV2)quarantinedEventV2.Type,
                Status = (EventArchiveStatusV2)quarantinedEventV2.Status,
                CreatedDate = quarantinedEventV2.CreatedDate,
                UpdatedDate = quarantinedEventV2.CreatedDate,
                ScheduledDate = quarantinedEventV2.ScheduledDate,
                EventAddressV2Id = quarantinedEventV2.EventAddressV2Id,
                EventParticipantV2Id = quarantinedEventV2.EventParticipantV2Id
            };

            List<EventArchiveV2> expectedEventArchiveV2s =
                new List<EventArchiveV2> { expectedEventArchiveV2 };

            // Step 1
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfQuarantinedEventV2sAsync(randomCancellationToken))
                        .ReturnsAsync((IEnumerable<EventV2>)retrievedQuarantinedEventV2s);

            // Step 2
            this.eventArchiveV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkAddEventArchiveV2sAsync(
                        It.Is(SameEventArchiveV2sAs(expectedEventArchiveV2s)),
                        randomCancellationToken))
                            .ReturnsAsync(expectedEventArchiveV2s);

            // Step 3
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkRemoveEventV2sAsync(
                        It.Is(SameEventV2sAs(retrievedQuarantinedEventV2s)),
                        randomCancellationToken))
                            .Returns(ValueTask.CompletedTask);

            // Step 4
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfQuarantinedEventV2sAsync(randomCancellationToken))
                        .ReturnsAsync(Enumerable.Empty<EventV2>());

            this.configurationBrokerMock
                .InSequence(mockSequence).Setup(broker =>
                    broker.GetBatchConfiguration())
                        .Returns(randomBatchConfiguration);

            // Step 5
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken))
                        .ReturnsAsync(Enumerable.Empty<EventV2>());

            // when
            await this.archivingEventV2CoordinationService
                .ArchiveEventV2sAsync(randomCancellationToken);

            // then
            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.BulkAddEventArchiveV2sAsync(
                    It.Is(SameEventArchiveV2sAs(expectedEventArchiveV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfQuarantinedEventV2sAsync(randomCancellationToken),
                    Times.Exactly(2));

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.BulkRemoveEventV2sAsync(
                    It.Is(SameEventV2sAs(retrievedQuarantinedEventV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
