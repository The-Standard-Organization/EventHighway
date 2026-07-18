// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Abstractions.EventHandlers.Exceptions;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Configurations.Dispatch;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Services.Foundations.EventCalls.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventCalls.V2
{
    public partial class EventCallV2ServiceTests
    {
        private readonly Mock<IEventHandlerBroker> eventHandlerBrokerMock;
        private readonly Mock<IEventHandler> eventHandlerMock;
        private readonly Mock<IConfigurationBroker> configurationBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventCallV2Service eventCallV2Service;

        public EventCallV2ServiceTests()
        {
            this.eventHandlerBrokerMock = new Mock<IEventHandlerBroker>();
            this.eventHandlerMock = new Mock<IEventHandler>();
            this.configurationBrokerMock = new Mock<IConfigurationBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.configurationBrokerMock.Setup(broker =>
                broker.GetDispatchConfiguration())
                    .Returns(new DispatchConfiguration());

            this.eventCallV2Service = new EventCallV2Service(
                eventHandlerBroker: this.eventHandlerBrokerMock.Object,
                configurationBroker: this.configurationBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static EventHandlerResult CreateRandomEventHandlerResult() =>
            new Filler<EventHandlerResult>().Create();

        private static EventCallV2 CreateRandomEventCallV2() =>
            CreateEventCallV2Filler().Create();

        public static TheoryData<Exception> CriticalEventHandlerDependencyExceptions() =>
            new TheoryData<Exception>
            {
                new SomeCriticalEventHandlerDependencyException()
            };

        public static TheoryData<Exception> DependencyValidationExceptions() =>
            new TheoryData<Exception>
            {
                new SomeDependencyValidationEventHandlerException()
            };

        private class SomeCriticalEventHandlerDependencyException
            : Exception, IEventHandlerDependencyException
        { }

        public static TheoryData<Exception> ServiceExceptions() =>
            new TheoryData<Exception>
            {
                new SomeServiceEventHandlerException()
            };

        private class SomeDependencyValidationEventHandlerException
            : Exception, IEventHandlerValidationException
        { }

        private class SomeServiceEventHandlerException
            : Exception, IEventHandlerServiceException
        { }

        private static Filler<EventCallV2> CreateEventCallV2Filler()
        {
            var filler = new Filler<EventCallV2>();

            filler.Setup()
                .OnProperty(call => call.FilterCriteria).IgnoreIt()
                .OnProperty(call => call.RequiredPromotedProperties).IgnoreIt()
                .OnProperty(call => call.PromotedProperties).IgnoreIt();

            return filler;
        }
    }
}
