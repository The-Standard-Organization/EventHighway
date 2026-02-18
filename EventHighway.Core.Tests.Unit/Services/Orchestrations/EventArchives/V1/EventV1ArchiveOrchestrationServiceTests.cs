// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Services.Foundations.EventArchives.V1;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Services.Orchestrations.EventArchives.V1;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V1
{
    public partial class EventV1ArchiveOrchestrationServiceTests
    {
        private readonly Mock<IListenerEventV1ArchiveService> listenerEventV1ArchiveServiceMock;
        private readonly Mock<IEventV1ArchiveService> eventV1ArchiveServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventV1ArchiveOrchestrationService eventV1ArchiveOrchestrationService;

        public EventV1ArchiveOrchestrationServiceTests()
        {
            this.listenerEventV1ArchiveServiceMock = new Mock<IListenerEventV1ArchiveService>();
            this.eventV1ArchiveServiceMock = new Mock<IEventV1ArchiveService>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventV1ArchiveOrchestrationService = new EventV1ArchiveOrchestrationService(
                listenerEventV1ArchiveService: this.listenerEventV1ArchiveServiceMock.Object,
                eventV1ArchiveService: this.eventV1ArchiveServiceMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static EventV1Archive CreateRandomEventV1Archive() =>
            CreateEventV1ArchiveFiller().Create();

        private static Filler<EventV1Archive> CreateEventV1ArchiveFiller()
        {
            var filler = new Filler<EventV1Archive>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset());

            return filler;
        }
    }
}
