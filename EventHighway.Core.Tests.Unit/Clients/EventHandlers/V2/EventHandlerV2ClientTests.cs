// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Clients.EventHandlers.V2;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions;
using EventHighway.Core.Services.Processings.EventHandlers.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventHandlers.V2
{
    public partial class EventHandlerV2ClientTests
    {
        private readonly Mock<IEventHandlerV2ProcessingService> eventHandlerV2ProcessingServiceMock;
        private readonly IEventHandlerV2Client eventHandlerV2Client;

        public EventHandlerV2ClientTests()
        {
            this.eventHandlerV2ProcessingServiceMock =
                new Mock<IEventHandlerV2ProcessingService>();

            this.eventHandlerV2Client =
                new EventHandlerV2Client(
                    eventHandlerV2ProcessingService:
                        this.eventHandlerV2ProcessingServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            return new TheoryData<Xeption>
            {
                new EventHandlerV2ProcessingValidationException(
                    someMessage,
                    someInnerException),

                new EventHandlerV2ProcessingDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static EventHandlerV2 CreateRandomEventHandlerV2() =>
            new Filler<EventHandlerV2>().Create();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static IQueryable<EventHandlerV2> CreateRandomEventHandlerV2s() =>
            new Filler<EventHandlerV2>().Create(count: GetRandomNumber()).AsQueryable();

        private static IEventHandler CreateRandomEventHandler()
        {
            var eventHandlerMock = new Mock<IEventHandler>();
            eventHandlerMock.SetupGet(eventHandler => eventHandler.Id).Returns(Guid.NewGuid());
            eventHandlerMock.SetupGet(eventHandler => eventHandler.Name).Returns(new MnemonicString(1).GetValue());

            return eventHandlerMock.Object;
        }
    }
}
