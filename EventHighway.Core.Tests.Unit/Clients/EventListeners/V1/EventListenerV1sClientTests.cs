// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Clients.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using EventHighway.Core.Services.Orchestrations.EventListeners.V1;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventListeners.V1
{
    public partial class EventListenerV1sClientTests
    {
        private readonly Mock<IEventListenerV1OrchestrationService> eventListenerV1OrchestrationServiceMock;
        private readonly IEventListenerV1sClient eventListenerV1SClient;

        public EventListenerV1sClientTests()
        {
            this.eventListenerV1OrchestrationServiceMock =
                new Mock<IEventListenerV1OrchestrationService>();

            this.eventListenerV1SClient =
                new EventListenerV1sClient(
                    eventListenerV1OrchestrationService:
                        this.eventListenerV1OrchestrationServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventListenerV1OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new EventListenerV1OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static EventListenerV1 CreateRandomEventListenerV1() =>
            CreateEventListenerV1Filler().Create();

        private static Filler<EventListenerV1> CreateEventListenerV1Filler()
        {
            var filler = new Filler<EventListenerV1>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)

                .OnProperty(eventListenerV1 => eventListenerV1.EventAddressV1s)
                    .IgnoreIt()

                .OnProperty(eventListenerV1 => eventListenerV1.ListenerEventV1s)
                    .IgnoreIt();

            return filler;
        }
    }
}
