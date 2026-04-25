// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
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
                        Status = (ListenerEventV1Status)item.Status,
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
                        Type = (EventV1Type)item.Type,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        ScheduledDate = item.ScheduledDate,
                        EventAddressId = item.EventAddressId,
                        ListenerEvents = retrievedListenerEventV1s
                    }).AsQueryable();

            List<ListenerEventArchiveV1> mappedListenerEventV1Archives =
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

            List<EventArchiveV1> mappedEventV1Archives =
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
                        ListenerEventArchives = mappedListenerEventV1Archives
                    }).ToList();

            this.eventV1OrchestrationServiceV1Mock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveAllDeadEventV1sWithListenersAsync())
                        .ReturnsAsync(retrievedEventV1s);

            foreach ((EventArchiveV1 mappedEventV1Archive, EventV1 retrievedEventV1)
                in mappedEventV1Archives.Zip(retrievedEventV1s))
            {
                this.dateTimeBrokerMock
                    .InSequence(mockSequence).Setup(broker =>
                        broker.GetDateTimeOffsetAsync())
                            .ReturnsAsync(
                                retrievedDateTimeOffset);

                this.eventV1ArchiveOrchestrationServiceMock
                    .InSequence(mockSequence).Setup(service =>
                        service.AddEventArchiveV1WithListenerEventArchiveV1sAsync(
                            It.Is(SameEventV1ArchiveAs(mappedEventV1Archive))))
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
                    Times.Exactly(mappedEventV1Archives.Count));

            foreach ((EventArchiveV1 mappedEventV1Archive, EventV1 retrievedEventV1)
                in mappedEventV1Archives.Zip(retrievedEventV1s))
            {
                this.eventV1ArchiveOrchestrationServiceMock.Verify(service =>
                    service.AddEventArchiveV1WithListenerEventArchiveV1sAsync(
                        It.Is(SameEventV1ArchiveAs(mappedEventV1Archive))),
                            Times.Once);

                this.eventV1OrchestrationServiceV1Mock.Verify(service =>
                    service.RemoveEventV1AndListenerEventV1sAsync(
                        It.Is(SameEventV1As(retrievedEventV1))),
                            Times.Once);
            }

            this.eventV1OrchestrationServiceV1Mock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventV1ArchiveOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
