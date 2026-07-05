// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RestoringEvents.V2
{
    public partial class RestoringEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldGenerateReplayListenerEventV2sForNewListenersAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid eventAddressId = GetRandomId();

            EventArchiveV2 inputEventArchiveV2 = CreateRandomEventArchiveV2s().First();
            inputEventArchiveV2.EventAddressV2Id = eventAddressId;

            List<EventArchiveV2> inputEventArchiveV2s =
                new List<EventArchiveV2> { inputEventArchiveV2 };

            EventListenerV2 existingEventListenerV2 = CreateRandomEventListenerV2s().First();
            EventListenerV2 newEventListenerV2 = CreateRandomEventListenerV2s().First();

            IQueryable<EventListenerV2> eventListenerV2s =
                new List<EventListenerV2> { existingEventListenerV2, newEventListenerV2 }
                    .AsQueryable();

            IQueryable<ListenerEventV2> existingListenerEventV2s =
                new List<ListenerEventV2>
                {
                    new ListenerEventV2
                    {
                        EventV2Id = inputEventArchiveV2.Id,
                        EventListenerV2Id = existingEventListenerV2.Id
                    }
                }.AsQueryable();

            List<ListenerEventV2> expectedGeneratedListenerEventV2s =
                new List<ListenerEventV2>
                {
                    new ListenerEventV2
                    {
                        Status = ListenerEventStatusV2.Replay,
                        EventV2Id = inputEventArchiveV2.Id,
                        EventAddressV2Id = eventAddressId,
                        EventListenerV2Id = newEventListenerV2.Id,
                        CreatedDate = inputEventArchiveV2.CreatedDate,
                        UpdatedDate = inputEventArchiveV2.CreatedDate,
                        RemainingRetryAttempts = this.retryConfiguration.RetryAttemptsAllowed,
                        RetryAttemptsAllowed = this.retryConfiguration.RetryAttemptsAllowed,
                        NextRetryAttemptNotBefore = null,
                        DispatchedDate = null
                    }
                };

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(existingListenerEventV2s);

            this.eventListenerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    eventAddressId, randomCancellationToken))
                        .ReturnsAsync(eventListenerV2s);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    randomCancellationToken))
                        .ReturnsAsync(expectedGeneratedListenerEventV2s);

            // when
            await this.restoringEventV2OrchestrationService.GenerateReplayForNewListenersAsync(
                inputEventArchiveV2s,
                randomCancellationToken);

            // then
            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    eventAddressId, randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.Is<List<ListenerEventV2>>(actual =>
                        SameGeneratedListenerEventV2sAs(expectedGeneratedListenerEventV2s, actual)),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
