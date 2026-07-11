// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions;
using EventHighway.Core.Services.Foundations.EventHandlers.V2;
using EventHighway.Core.Services.Processings.EventHandlers.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventHandlers.V2
{
    public partial class EventHandlerV2ProcessingServiceTests
    {
        private readonly Mock<IEventHandlerV2Service> eventHandlerV2ServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventHandlerV2ProcessingService eventHandlerV2ProcessingService;

        public EventHandlerV2ProcessingServiceTests()
        {
            this.eventHandlerV2ServiceMock =
                new Mock<IEventHandlerV2Service>();

            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventHandlerV2ProcessingService =
                new EventHandlerV2ProcessingService(
                    eventHandlerV2Service: this.eventHandlerV2ServiceMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new EventHandlerV2ValidationException(
                    someMessage,
                    someInnerException),

                new EventHandlerV2DependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new EventHandlerV2DependencyException(
                    someMessage,
                    someInnerException),

                new EventHandlerV2ServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static EventHandlerV2 CreateRandomEventHandlerV2() =>
            new Filler<EventHandlerV2>().Create();

        private static IEventHandler CreateRandomEventHandler()
        {
            var eventHandlerMock = new Mock<IEventHandler>();
            eventHandlerMock.SetupGet(eventHandler => eventHandler.Id).Returns(Guid.NewGuid());
            eventHandlerMock.SetupGet(eventHandler => eventHandler.Name).Returns(new MnemonicString(1).GetValue());

            return eventHandlerMock.Object;
        }
    }
}
