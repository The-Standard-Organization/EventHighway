// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
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

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyValidationError" });

            return new TheoryData<Xeption>
            {
                new EventAddressV2ValidationException(someMessage, someInnerException),
                new EventAddressV2DependencyValidationException(someMessage, someInnerException),
                new EventListenerV2ValidationException(someMessage, someInnerException),
                new EventListenerV2DependencyValidationException(someMessage, someInnerException),
                new EventParticipantV2ValidationException(someMessage, someInnerException),
                new EventParticipantV2DependencyValidationException(someMessage, someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new EventAddressV2DependencyException(someMessage, someInnerException),
                new EventAddressV2ServiceException(someMessage, someInnerException),
                new EventListenerV2DependencyException(someMessage, someInnerException),
                new EventListenerV2ServiceException(someMessage, someInnerException),
                new EventParticipantV2DependencyException(someMessage, someInnerException),
                new EventParticipantV2ServiceException(someMessage, someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static bool GetRandomBoolean() =>
            GetRandomNumber() % 2 == 0;

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static T GetRandomEnum<T>() where T : struct, Enum
        {
            T[] enumValues = Enum.GetValues<T>();

            return enumValues[new IntRange(min: 0, max: enumValues.Length - 1).GetValue()];
        }

        private static List<EventAddressV2> CreateRandomEventAddressV2s(int count) =>
            Enumerable.Range(start: 0, count: count)
                .Select(item => new EventAddressV2
                {
                    Id = GetRandomId(),
                    Name = GetRandomString(),
                    Description = GetRandomString()
                })
                .ToList();

        private static List<EventParticipantV2> CreateRandomEventParticipantV2s(int count) =>
            Enumerable.Range(start: 0, count: count)
                .Select(item => new EventParticipantV2
                {
                    Id = GetRandomId(),
                    Name = GetRandomString(),
                    ContactEmail = GetRandomString(),
                    ContactPhone = GetRandomString(),
                    IsActive = GetRandomBoolean()
                })
                .ToList();

        private static List<EventListenerV2> CreateRandomEventListenerV2s(
            IReadOnlyList<Guid> handlerIds,
            IReadOnlyList<EventAddressV2> eventAddresses,
            IReadOnlyList<EventParticipantV2> eventParticipants) =>
            handlerIds
                .Select((handlerId, index) => new EventListenerV2
                {
                    Id = GetRandomId(),
                    HandlerId = handlerId,
                    EventAddressV2Id = eventAddresses[index % eventAddresses.Count].Id,
                    EventParticipantV2Id = eventParticipants[index % eventParticipants.Count].Id
                })
                .ToList();
    }
}
