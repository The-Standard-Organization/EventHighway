// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Models.Clients.EventHandlers.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventHandlers;
using EventHighway.Portal.Web.Services.Views.Foundations.EventHandlers;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventHandlers
{
    public partial class EventHandlersViewServiceTests
    {
        private readonly Mock<IEventHighwayBroker> eventHighwayBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventHandlersViewService eventHandlersViewService;

        public EventHandlersViewServiceTests()
        {
            this.eventHighwayBrokerMock = new Mock<IEventHighwayBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventHandlersViewService = new EventHandlersViewService(
                eventHighwayBroker: this.eventHighwayBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            var someInnerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new EventHandlerV2ClientValidationException(
                    message: GetRandomString(),
                    innerException: someInnerException,
                    data: new Hashtable()),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            var someInnerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new EventHandlerV2ClientDependencyException(
                    message: GetRandomString(),
                    innerException: someInnerException,
                    data: new Hashtable()),

                new EventHandlerV2ClientServiceException(
                    message: GetRandomString(),
                    innerException: someInnerException,
                    data: new Hashtable()),
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static List<EventHandlerV2> CreateRandomEventHandlerV2s() =>
            Enumerable.Range(0, 3).Select(_ => new EventHandlerV2
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString()
            }).ToList();

        private static List<EventHandlerView> MapToViews(
            IEnumerable<EventHandlerV2> eventHandlerV2s) =>
            eventHandlerV2s.Select(eventHandlerV2 => new EventHandlerView
            {
                Id = eventHandlerV2.Id,
                Name = eventHandlerV2.Name
            }).ToList();
    }
}
