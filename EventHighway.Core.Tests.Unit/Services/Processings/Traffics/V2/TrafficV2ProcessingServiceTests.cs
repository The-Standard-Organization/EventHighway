// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Processings.Traffics.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Traffics.V2
{
    public partial class TrafficV2ProcessingServiceTests
    {
        private readonly Mock<IEventV2Service> eventV2ServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ITrafficV2ProcessingService trafficV2ProcessingService;

        public TrafficV2ProcessingServiceTests()
        {
            this.eventV2ServiceMock = new Mock<IEventV2Service>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.trafficV2ProcessingService = new TrafficV2ProcessingService(
                eventV2Service: this.eventV2ServiceMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            return new TheoryData<Xeption>
            {
                new EventV2DependencyException(
                    someMessage,
                    someInnerException),

                new EventV2ServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomPositiveNumber() =>
            new IntRange(min: 1, max: 9).GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static EventV2 CreateEventV2WithDate(
            DateTimeOffset createdDate,
            EventTypeV2 type)
        {
            return new EventV2
            {
                Id = Guid.NewGuid(),
                Type = type,
                Status = EventStatusV2.Active,
                Content = GetRandomString(),
                EventName = GetRandomString(),
                CreatedDate = createdDate,
                UpdatedDate = createdDate,
                EventAddressV2Id = Guid.NewGuid()
            };
        }

        private static ListenerEventV2 CreateListenerEventV2WithDate(
            DateTimeOffset createdDate,
            ListenerEventStatusV2 status)
        {
            return new ListenerEventV2
            {
                Id = Guid.NewGuid(),
                Status = status,
                CreatedDate = createdDate,
                UpdatedDate = createdDate,
                EventV2Id = Guid.NewGuid(),
                EventAddressV2Id = Guid.NewGuid(),
                EventListenerV2Id = Guid.NewGuid()
            };
        }
    }
}
