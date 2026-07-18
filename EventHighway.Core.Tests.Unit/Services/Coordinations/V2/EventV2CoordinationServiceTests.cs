// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventFirings.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.EventParticipants.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.Events.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.Events.V2;
using EventHighway.Core.Services.Orchestrations.EventFirings.V2;
using EventHighway.Core.Services.Orchestrations.EventParticipants.V2;
using EventHighway.Core.Services.Orchestrations.Events.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V2
{
    public partial class EventV2CoordinationServiceTests
    {
        private readonly Mock<IEventV2OrchestrationService> eventV2OrchestrationServiceMock;
        private readonly Mock<IEventFiringV2OrchestrationService> eventFiringV2OrchestrationServiceMock;
        private readonly Mock<IEventParticipantV2OrchestrationService> eventParticipantV2OrchestrationServiceMock;
        private readonly Mock<IConfigurationBroker> configurationBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventV2CoordinationService eventV2CoordinationService;

        public EventV2CoordinationServiceTests()
        {
            this.eventV2OrchestrationServiceMock =
                new Mock<IEventV2OrchestrationService>();

            this.eventFiringV2OrchestrationServiceMock =
                new Mock<IEventFiringV2OrchestrationService>(
                    behavior: MockBehavior.Strict);

            this.eventParticipantV2OrchestrationServiceMock =
                new Mock<IEventParticipantV2OrchestrationService>(
                    behavior: MockBehavior.Strict);

            this.configurationBrokerMock = new Mock<IConfigurationBroker>();

            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>(
                behavior: MockBehavior.Strict);

            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(new BatchConfiguration { BatchSizeForBulkProcessing = 1000 });

            this.eventV2CoordinationService =
                new EventV2CoordinationService(
                    eventV2OrchestrationService: this.eventV2OrchestrationServiceMock.Object,
                    eventFiringV2OrchestrationService: this.eventFiringV2OrchestrationServiceMock.Object,
                    eventParticipantV2OrchestrationService: this.eventParticipantV2OrchestrationServiceMock.Object,
                    configurationBroker: this.configurationBrokerMock.Object,
                    dateTimeBroker: this.dateTimeBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventV2OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new EventV2OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException),

                new EventFiringV2OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new EventFiringV2OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException),

                new EventParticipantV2OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new EventParticipantV2OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException)
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new EventV2OrchestrationServiceException(
                    someMessage,
                    someInnerException),

                new EventFiringV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new EventFiringV2OrchestrationServiceException(
                    someMessage,
                    someInnerException),

                new EventParticipantV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new EventParticipantV2OrchestrationServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Exception> PlainException()
        {
            return new TheoryData<Exception>
            {
                new Exception()
            };
        }

        public static TheoryData<DateTimeOffset, DateTimeOffset?> ScheduledDates()
        {
            DateTimeOffset fixedDateTimeOffset =
                new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);

            return new TheoryData<DateTimeOffset, DateTimeOffset?>
            {
                {
                    fixedDateTimeOffset,
                    fixedDateTimeOffset.AddDays(-2)
                },
                {
                    fixedDateTimeOffset,
                    null
                }
            };
        }

        private void SetupValidateEventParticipantsSucceeds() =>
            this.eventParticipantV2OrchestrationServiceMock.Setup(service =>
                service.ValidateEventParticipantsAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()))
                        .Returns(ValueTask.CompletedTask);

        private void SetupValidateEventParticipantsInSequence(MockSequence mockSequence) =>
            this.eventParticipantV2OrchestrationServiceMock.InSequence(mockSequence).Setup(service =>
                service.ValidateEventParticipantsAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()))
                        .Returns(ValueTask.CompletedTask);

        private void VerifyValidateEventParticipantsCalledOnce() =>
            this.eventParticipantV2OrchestrationServiceMock.Verify(service =>
                service.ValidateEventParticipantsAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static IQueryable<EventV2> CreateRandomEventV2s()
        {
            return CreateEventV2Filler()
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static EventV2 CreateRandomEventV2() =>
            CreateEventV2Filler().Create();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Filler<EventV2> CreateEventV2Filler()
        {
            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset())

                .OnProperty(eventV2 =>
                    eventV2.EventAddressV2).IgnoreIt()

                .OnProperty(eventV2 =>
                    eventV2.ListenerEventV2s).IgnoreIt()

                .OnType<EventAddressV2>().IgnoreIt()
                .OnType<ListenerEventV2>().IgnoreIt()
                .OnType<EventParticipantV2>().IgnoreIt()

                .OnProperty(eventV2 =>
                    eventV2.Status).Use(EventStatusV2.Active);

            return filler;
        }
    }
}
