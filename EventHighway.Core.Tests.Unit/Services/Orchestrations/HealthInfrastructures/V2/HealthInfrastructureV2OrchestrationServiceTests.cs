// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Services.Orchestrations.HealthInfrastructures.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.HealthInfrastructures.V2
{
    public partial class HealthInfrastructureV2OrchestrationServiceTests
    {
        private readonly Mock<IEventAddressV2Service> eventAddressV2ServiceMock;
        private readonly Mock<IEventListenerV2Service> eventListenerV2ServiceMock;
        private readonly Mock<IEventParticipantV2Service> eventParticipantV2ServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IHealthInfrastructureV2OrchestrationService healthInfrastructureV2OrchestrationService;

        public HealthInfrastructureV2OrchestrationServiceTests()
        {
            this.eventAddressV2ServiceMock = new Mock<IEventAddressV2Service>();
            this.eventListenerV2ServiceMock = new Mock<IEventListenerV2Service>();
            this.eventParticipantV2ServiceMock = new Mock<IEventParticipantV2Service>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.healthInfrastructureV2OrchestrationService =
                new HealthInfrastructureV2OrchestrationService(
                    eventAddressV2Service: this.eventAddressV2ServiceMock.Object,
                    eventListenerV2Service: this.eventListenerV2ServiceMock.Object,
                    eventParticipantV2Service: this.eventParticipantV2ServiceMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static IQueryable<EventAddressV2> CreateRandomEventAddressV2s(int count) =>
            Enumerable.Range(start: 0, count: count)
                .Select(item => new EventAddressV2 { Id = GetRandomId() })
                .AsQueryable();

        private static IQueryable<EventParticipantV2> CreateRandomEventParticipantV2s(int count) =>
            Enumerable.Range(start: 0, count: count)
                .Select(item => new EventParticipantV2 { Id = GetRandomId() })
                .AsQueryable();

        private static IQueryable<EventListenerV2> CreateEventListenerV2sWithHandlerIds(
            IEnumerable<Guid> handlerIds) =>
            handlerIds
                .Select(handlerId => new EventListenerV2 { Id = GetRandomId(), HandlerId = handlerId })
                .AsQueryable();
    }
}
