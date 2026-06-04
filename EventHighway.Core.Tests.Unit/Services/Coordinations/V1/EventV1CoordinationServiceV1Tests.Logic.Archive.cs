// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V1
{
    public partial class EventV1CoordinationServiceV1Tests
    {
        [Fact]
        public async Task ShouldArchiveEventV1sAsync()
        {
            // given
            var mockSequence = new MockSequence();

            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            DateTimeOffset retrievedDateTimeOffset =
                randomDateTimeOffset;

            List<dynamic> randomListenerEventV1sProperties =
                CreateRandomListenerEventV1sProperties();

            List<dynamic> randomEventV1sProperties =
                CreateRandomEventV1sProperties();

            ICollection<ListenerEventV1> retrievedListenerEventV1s =
                randomListenerEventV1sProperties.Select(item =>
                    new ListenerEventV1
                    {
                        Id = item.Id,
                        Status = (ListenerEventStatusV1)item.Status,
                        Response = item.Response,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        EventId = item.EventId,
                        EventAddressId = item.EventAddressId,
                        EventListenerId = item.EventListenerId
                    }).ToList();

            IQueryable<EventV1> retrievedEventV1s =
                randomEventV1sProperties.Select(item =>
                    new EventV1
                    {
                        Id = item.Id,
                        Content = item.Content,
                        Type = (EventTypeV1)item.Type,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        ScheduledDate = item.ScheduledDate,
                        EventAddressId = item.EventAddressId,
                        ListenerEventV1s = retrievedListenerEventV1s
                    }).AsQueryable();

            List<ListenerEventArchiveV1> mappedListenerEventArchiveV1s =
                randomListenerEventV1sProperties.Select(item =>
                    new ListenerEventArchiveV1
                    {
                        Id = item.Id,
                        Status = (ListenerEventArchiveStatusV1)item.Status,
                        Response = item.Response,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        ArchivedDate = retrievedDateTimeOffset,
                        EventId = item.EventId,
                        EventAddressId = item.EventAddressId,
                        EventListenerId = item.EventListenerId
                    }).ToList();

            List<EventArchiveV1> mappedEventArchiveV1s =
                randomEventV1sProperties.Select(item =>
                    new EventArchiveV1
                    {
                        Id = item.Id,
                        Content = item.Content,
                        Type = (EventArchiveTypeV1)item.Type,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        ScheduledDate = item.ScheduledDate,
                        EventAddressId = item.EventAddressId,
                        ArchivedDate = retrievedDateTimeOffset,
                        ListenerEventArchiveV1s = mappedListenerEventArchiveV1s
                    }).ToList();

            this.eventV1OrchestrationServiceV1Mock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveAllDeadEventV1sWithListenersAsync())
                        .ReturnsAsync(retrievedEventV1s);

            foreach ((EventArchiveV1 mappedEventArchiveV1, EventV1 retrievedEventV1)
                in mappedEventArchiveV1s.Zip(retrievedEventV1s))
            {
                this.dateTimeBrokerMock
                    .InSequence(mockSequence).Setup(broker =>
                        broker.GetDateTimeOffsetAsync())
                            .ReturnsAsync(
                                retrievedDateTimeOffset);

                this.eventArchiveV1OrchestrationServiceMock
                    .InSequence(mockSequence).Setup(service =>
                        service.AddEventArchiveV1WithListenerEventArchiveV1sAsync(
                            It.Is(SameEventArchiveV1As(mappedEventArchiveV1))))
                                .Returns(ValueTask.CompletedTask);

                this.eventV1OrchestrationServiceV1Mock
                    .InSequence(mockSequence).Setup(service =>
                        service.RemoveEventV1AndListenerEventV1sAsync(
                            It.Is(SameEventV1As(retrievedEventV1))))
                                .Returns(ValueTask.CompletedTask);
            }

            // when
            await this.eventV1CoordinationServiceV1.ArchiveDeadEventV1sAsync();

            // then
            this.eventV1OrchestrationServiceV1Mock.Verify(service =>
                service.RetrieveAllDeadEventV1sWithListenersAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(mappedEventArchiveV1s.Count));

            foreach ((EventArchiveV1 mappedEventArchiveV1, EventV1 retrievedEventV1)
                in mappedEventArchiveV1s.Zip(retrievedEventV1s))
            {
                this.eventArchiveV1OrchestrationServiceMock.Verify(service =>
                    service.AddEventArchiveV1WithListenerEventArchiveV1sAsync(
                        It.Is(SameEventArchiveV1As(mappedEventArchiveV1))),
                            Times.Once);

                this.eventV1OrchestrationServiceV1Mock.Verify(service =>
                    service.RemoveEventV1AndListenerEventV1sAsync(
                        It.Is(SameEventV1As(retrievedEventV1))),
                            Times.Once);
            }

            this.eventV1OrchestrationServiceV1Mock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
