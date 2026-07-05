// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Orchestrations.DuplicateSummaries.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.DuplicateSummaries.V2
{
    public partial class DuplicateSummaryV2OrchestrationServiceTests
    {
        private readonly Mock<IEventAddressV2Service> eventAddressV2ServiceMock;
        private readonly Mock<IEventV2Service> eventV2ServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IDuplicateSummaryV2OrchestrationService duplicateSummaryV2OrchestrationService;

        public DuplicateSummaryV2OrchestrationServiceTests()
        {
            this.eventAddressV2ServiceMock = new Mock<IEventAddressV2Service>();
            this.eventV2ServiceMock = new Mock<IEventV2Service>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.duplicateSummaryV2OrchestrationService =
                new DuplicateSummaryV2OrchestrationService(
                    eventAddressV2Service: this.eventAddressV2ServiceMock.Object,
                    eventV2Service: this.eventV2ServiceMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
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
                new EventV2DependencyException(someMessage, someInnerException),
                new EventV2ServiceException(someMessage, someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private void SetupDuplicateFoundationMocks(
            CancellationToken cancellationToken,
            IQueryable<EventAddressV2> addresses,
            IQueryable<EventV2> events)
        {
            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(cancellationToken))
                    .ReturnsAsync(addresses);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(cancellationToken))
                    .ReturnsAsync(events);
        }

        private void VerifyDuplicateFoundationMocksOnce(CancellationToken cancellationToken)
        {
            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(cancellationToken),
                    Times.Once);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(cancellationToken),
                    Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomPositiveNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static EventAddressV2 CreateEventAddressV2(
            Guid id,
            string name,
            string description)
        {
            return new EventAddressV2
            {
                Id = id,
                Name = name,
                Description = description,
                CreatedDate = GetRandomDateTimeOffset(),
                UpdatedDate = GetRandomDateTimeOffset()
            };
        }

        private static EventParticipantV2 CreateEventParticipantV2(
            Guid id,
            string name,
            string contactEmail,
            string contactPhone,
            bool isActive)
        {
            return new EventParticipantV2
            {
                Id = id,
                Name = name,
                Description = GetRandomString(),
                ContactEmail = contactEmail,
                ContactPhone = contactPhone,
                IsActive = isActive,
                CreatedDate = GetRandomDateTimeOffset(),
                UpdatedDate = GetRandomDateTimeOffset()
            };
        }

        private static EventV2 CreateEventV2ForAddress(
            Guid eventAddressId,
            DateTimeOffset createdDate,
            string contentHash,
            int remainingRetryAttempts,
            EventStatusV2 status)
        {
            return new EventV2
            {
                Id = Guid.NewGuid(),
                Type = EventTypeV2.Immediate,
                Status = status,
                ContentHash = contentHash,
                Content = GetRandomString(),
                EventName = GetRandomString(),
                CreatedDate = createdDate,
                UpdatedDate = createdDate,
                EventAddressV2Id = eventAddressId
            };
        }

        private static EventV2 CreateEventV2ForParticipant(
            Guid id,
            Guid eventAddressId,
            Guid? participantId,
            EventParticipantV2 participant,
            DateTimeOffset createdDate,
            string contentHash,
            EventStatusV2 status)
        {
            return new EventV2
            {
                Id = id,
                Type = EventTypeV2.Immediate,
                Status = status,
                ContentHash = contentHash,
                Content = GetRandomString(),
                EventName = GetRandomString(),
                CreatedDate = createdDate,
                UpdatedDate = createdDate,
                EventAddressV2Id = eventAddressId,
                EventParticipantV2Id = participantId,
                EventParticipantV2 = participant
            };
        }
    }
}
