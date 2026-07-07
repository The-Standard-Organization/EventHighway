// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.HealthEvents.V2;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.HealthEvents.V2
{
    public partial class HealthEventsV2OrchestrationServiceTests
    {
        private readonly Mock<IEventV2Service> eventV2ServiceMock;
        private readonly Mock<IListenerEventV2Service> listenerEventV2ServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IHealthEventsV2OrchestrationService healthEventsV2OrchestrationService;

        public HealthEventsV2OrchestrationServiceTests()
        {
            this.eventV2ServiceMock = new Mock<IEventV2Service>();
            this.listenerEventV2ServiceMock = new Mock<IListenerEventV2Service>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.healthEventsV2OrchestrationService =
                new HealthEventsV2OrchestrationService(
                    eventV2Service: this.eventV2ServiceMock.Object,
                    listenerEventV2Service: this.listenerEventV2ServiceMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static int GetRandomRetryCount() =>
            new IntRange(min: 0, max: 4).GetValue();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static T GetRandomEnum<T>() where T : struct, Enum
        {
            T[] enumValues = Enum.GetValues<T>();

            return enumValues[new IntRange(min: 0, max: enumValues.Length - 1).GetValue()];
        }

        private static List<EventV2> CreateRandomEventV2s(int count) =>
            Enumerable.Range(start: 0, count: count)
                .Select(item => new EventV2
                {
                    Id = GetRandomId(),
                    EventAddressV2Id = GetRandomId(),
                    EventParticipantV2Id = GetRandomId(),
                    Status = GetRandomEnum<EventStatusV2>(),
                    Type = GetRandomEnum<EventTypeV2>(),
                    ContentHash = GetRandomString(),
                    CreatedDate = GetRandomDateTimeOffset()
                })
                .ToList();

        private static List<ListenerEventV2> CreateRandomListenerEventV2s(int count) =>
            Enumerable.Range(start: 0, count: count)
                .Select(item => new ListenerEventV2
                {
                    Id = GetRandomId(),
                    EventAddressV2Id = GetRandomId(),
                    EventV2Id = GetRandomId(),
                    EventParticipantV2Id = GetRandomId(),
                    Status = GetRandomEnum<ListenerEventStatusV2>(),
                    RemainingRetryAttempts = GetRandomRetryCount(),
                    CreatedDate = GetRandomDateTimeOffset()
                })
                .ToList();
    }
}
