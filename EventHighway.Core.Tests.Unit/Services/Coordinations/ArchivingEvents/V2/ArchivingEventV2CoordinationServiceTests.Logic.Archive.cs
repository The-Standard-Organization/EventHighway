// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldArchiveDeadEventV2sAsync()
        {
            // given
            var mockSequence = new MockSequence();

            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            DateTimeOffset retrievedDateTimeOffset =
                randomDateTimeOffset;

            List<dynamic> randomListenerEventV2sProperties =
                CreateRandomListenerEventV2sProperties();

            List<dynamic> randomEventV2sProperties =
                CreateRandomEventV2sProperties();

            ICollection<ListenerEventV2> retrievedListenerEventV2s =
                randomListenerEventV2sProperties.Select(item =>
                    new ListenerEventV2
                    {
                        Id = item.Id,
                        Status = (ListenerEventStatusV2)item.Status,
                        Response = item.Response,
                        ResponseCode = item.ResponseCode,
                        ResponseMessage = item.ResponseMessage,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        EventId = item.EventId,
                        EventAddressId = item.EventAddressId,
                        EventListenerId = item.EventListenerId
                    }).ToList();

            List<EventV2> retrievedEventV2s =
                randomEventV2sProperties.Select(item =>
                    new EventV2
                    {
                        Id = item.Id,
                        Content = item.Content,
                        EventName = item.EventName,
                        Type = (EventTypeV2)item.Type,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        ScheduledDate = item.ScheduledDate,
                        RemainingRetryAttempts = item.RemainingRetryAttempts,
                        EventAddressId = item.EventAddressId,
                        ListenerEventV2s = retrievedListenerEventV2s
                    }).ToList();

            List<ListenerEventArchiveV2> mappedListenerEventArchiveV2s =
                randomListenerEventV2sProperties.Select(item =>
                    new ListenerEventArchiveV2
                    {
                        Id = item.Id,
                        Status = (ListenerEventArchiveStatusV2)item.Status,
                        Response = item.Response,
                        ResponseCode = item.ResponseCode,
                        ResponseMessage = item.ResponseMessage,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        ArchivedDate = retrievedDateTimeOffset,
                        EventId = item.EventId,
                        EventAddressId = item.EventAddressId,
                        EventListenerId = item.EventListenerId,
                        EventArchiveV2Id = item.EventId
                    }).ToList();

            List<EventArchiveV2> mappedEventArchiveV2s =
                randomEventV2sProperties.Select(item =>
                    new EventArchiveV2
                    {
                        Id = item.Id,
                        Content = item.Content,
                        EventName = item.EventName,
                        Type = (EventArchiveTypeV2)item.Type,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        ScheduledDate = item.ScheduledDate,
                        RemainingRetryAttempts = item.RemainingRetryAttempts,
                        EventAddressId = item.EventAddressId,
                        ArchivedDate = retrievedDateTimeOffset,
                        ListenerEventArchiveV2s = mappedListenerEventArchiveV2s
                    }).ToList();

            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveAllDeadEventV2sWithListenersAsync(
                        It.IsAny<CancellationToken>()))
                            .Returns(CreateAsyncEnumerable(retrievedEventV2s));

            foreach ((EventArchiveV2 mappedEventArchiveV2, EventV2 retrievedEventV2)
                in mappedEventArchiveV2s.Zip(retrievedEventV2s))
            {
                this.dateTimeBrokerMock
                    .InSequence(mockSequence).Setup(broker =>
                        broker.GetDateTimeOffsetAsync())
                            .ReturnsAsync(retrievedDateTimeOffset);

                this.eventArchiveV2OrchestrationServiceMock
                    .InSequence(mockSequence).Setup(service =>
                        service.AddEventArchiveV2WithListenerEventArchiveV2sAsync(
                            It.Is(SameEventArchiveV2As(mappedEventArchiveV2)),
                            It.IsAny<CancellationToken>()))
                                .Returns(ValueTask.CompletedTask);

                this.archivingEventV2OrchestrationServiceMock
                    .InSequence(mockSequence).Setup(service =>
                        service.RemoveEventV2AndListenerEventV2sAsync(
                            It.Is(SameEventV2As(retrievedEventV2)),
                            It.IsAny<CancellationToken>()))
                                .Returns(ValueTask.CompletedTask);
            }

            // when
            await this.archivingEventV2CoordinationService
                .ArchiveDeadEventV2sAsync(TestContext.Current.CancellationToken);

            // then
            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(mappedEventArchiveV2s.Count));

            foreach ((EventArchiveV2 mappedEventArchiveV2, EventV2 retrievedEventV2)
                in mappedEventArchiveV2s.Zip(retrievedEventV2s))
            {
                this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                    service.AddEventArchiveV2WithListenerEventArchiveV2sAsync(
                        It.Is(SameEventArchiveV2As(mappedEventArchiveV2)),
                        It.IsAny<CancellationToken>()),
                            Times.Once);

                this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                    service.RemoveEventV2AndListenerEventV2sAsync(
                        It.Is(SameEventV2As(retrievedEventV2)),
                        It.IsAny<CancellationToken>()),
                            Times.Once);
            }

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
