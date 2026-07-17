// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V2
{
    public partial class EventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEventV2sByQueryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid targetEventAddressV2Id = GetRandomId();
            DateTimeOffset baseDateTimeOffset = GetRandomDateTimeOffset();

            List<EventV2> matchingEventV2s =
                Enumerable.Range(start: 0, count: 4).Select(index =>
                {
                    EventV2 matchingEventV2 = CreateRandomEventV2();
                    matchingEventV2.EventAddressV2Id = targetEventAddressV2Id;
                    matchingEventV2.Status = EventStatusV2.Active;
                    matchingEventV2.CreatedDate = baseDateTimeOffset.AddMinutes(-index);

                    return matchingEventV2;
                }).ToList();

            EventV2 differentAddressEventV2 = CreateRandomEventV2();

            EventV2 quarantinedEventV2 = CreateRandomEventV2();
            quarantinedEventV2.EventAddressV2Id = targetEventAddressV2Id;
            quarantinedEventV2.Status = EventStatusV2.Quarantined;

            IQueryable<EventV2> allEventV2s = matchingEventV2s
                .Append(differentAddressEventV2)
                .Append(quarantinedEventV2)
                .AsQueryable();

            var inputEventV2Query = new EventV2Query
            {
                EventAddressV2Id = targetEventAddressV2Id,
                Status = EventStatusV2.Active,
                Skip = 1,
                Take = 2
            };

            List<EventV2> expectedEventV2s = matchingEventV2s
                .OrderByDescending(eventV2 => eventV2.CreatedDate)
                .ThenBy(eventV2 => eventV2.Id)
                .Skip(1)
                .Take(2)
                .ToList();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(allEventV2s);

            // when
            IQueryable<EventV2> actualEventV2sQuery =
                await this.eventV2CoordinationService.RetrieveEventV2sByQueryAsync(
                    inputEventV2Query, randomCancellationToken);

            List<EventV2> actualEventV2s = actualEventV2sQuery.ToList();

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s, options =>
                options.WithStrictOrdering());

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveEventV2sByQueryWithRemainingCriteriaAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string targetEventName = GetRandomString();
            Guid targetEventParticipantV2Id = GetRandomId();
            DateTimeOffset baseDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset createdFrom = baseDateTimeOffset;
            DateTimeOffset createdTo = baseDateTimeOffset.AddHours(1);
            DateTimeOffset scheduledFrom = baseDateTimeOffset.AddHours(2);
            DateTimeOffset scheduledTo = baseDateTimeOffset.AddHours(3);

            EventV2 CreateMatchingEventV2(int minutesIntoCreatedWindow)
            {
                EventV2 matchingEventV2 = CreateRandomEventV2();
                matchingEventV2.EventName = targetEventName;
                matchingEventV2.EventParticipantV2Id = targetEventParticipantV2Id;
                matchingEventV2.Type = EventTypeV2.Scheduled;
                matchingEventV2.CreatedDate = createdFrom.AddMinutes(minutesIntoCreatedWindow);
                matchingEventV2.ScheduledDate = scheduledFrom.AddMinutes(30);

                return matchingEventV2;
            }

            List<EventV2> matchingEventV2s = new List<EventV2>
            {
                CreateMatchingEventV2(minutesIntoCreatedWindow: 10),
                CreateMatchingEventV2(minutesIntoCreatedWindow: 20)
            };

            EventV2 wrongNameEventV2 = CreateMatchingEventV2(minutesIntoCreatedWindow: 30);
            wrongNameEventV2.EventName = GetRandomString();

            EventV2 wrongParticipantEventV2 = CreateMatchingEventV2(minutesIntoCreatedWindow: 30);
            wrongParticipantEventV2.EventParticipantV2Id = GetRandomId();

            EventV2 wrongTypeEventV2 = CreateMatchingEventV2(minutesIntoCreatedWindow: 30);
            wrongTypeEventV2.Type = EventTypeV2.Immediate;

            EventV2 createdBeforeWindowEventV2 = CreateMatchingEventV2(minutesIntoCreatedWindow: 30);
            createdBeforeWindowEventV2.CreatedDate = createdFrom.AddMinutes(-5);

            EventV2 createdAfterWindowEventV2 = CreateMatchingEventV2(minutesIntoCreatedWindow: 30);
            createdAfterWindowEventV2.CreatedDate = createdTo.AddMinutes(5);

            EventV2 scheduledBeforeWindowEventV2 = CreateMatchingEventV2(minutesIntoCreatedWindow: 30);
            scheduledBeforeWindowEventV2.ScheduledDate = scheduledFrom.AddMinutes(-5);

            EventV2 scheduledAfterWindowEventV2 = CreateMatchingEventV2(minutesIntoCreatedWindow: 30);
            scheduledAfterWindowEventV2.ScheduledDate = scheduledTo.AddMinutes(5);

            EventV2 unscheduledEventV2 = CreateMatchingEventV2(minutesIntoCreatedWindow: 30);
            unscheduledEventV2.ScheduledDate = null;

            IQueryable<EventV2> allEventV2s = matchingEventV2s
                .Append(wrongNameEventV2)
                .Append(wrongParticipantEventV2)
                .Append(wrongTypeEventV2)
                .Append(createdBeforeWindowEventV2)
                .Append(createdAfterWindowEventV2)
                .Append(scheduledBeforeWindowEventV2)
                .Append(scheduledAfterWindowEventV2)
                .Append(unscheduledEventV2)
                .AsQueryable();

            var inputEventV2Query = new EventV2Query
            {
                EventName = targetEventName,
                EventParticipantV2Id = targetEventParticipantV2Id,
                Type = EventTypeV2.Scheduled,
                CreatedFrom = createdFrom,
                CreatedTo = createdTo,
                ScheduledFrom = scheduledFrom,
                ScheduledTo = scheduledTo
            };

            List<EventV2> expectedEventV2s = matchingEventV2s
                .OrderByDescending(eventV2 => eventV2.CreatedDate)
                .ThenBy(eventV2 => eventV2.Id)
                .ToList();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(allEventV2s);

            // when
            IQueryable<EventV2> actualEventV2sQuery =
                await this.eventV2CoordinationService.RetrieveEventV2sByQueryAsync(
                    inputEventV2Query, randomCancellationToken);

            List<EventV2> actualEventV2s = actualEventV2sQuery.ToList();

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s, options =>
                options.WithStrictOrdering());

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveEventV2sWithEventAddressV2ByQueryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid targetEventAddressV2Id = GetRandomId();
            DateTimeOffset baseDateTimeOffset = GetRandomDateTimeOffset();

            List<EventV2> matchingEventV2s =
                Enumerable.Range(start: 0, count: 4).Select(index =>
                {
                    EventV2 matchingEventV2 = CreateRandomEventV2();
                    matchingEventV2.EventAddressV2Id = targetEventAddressV2Id;
                    matchingEventV2.Status = EventStatusV2.Active;
                    matchingEventV2.CreatedDate = baseDateTimeOffset.AddMinutes(-index);

                    return matchingEventV2;
                }).ToList();

            EventV2 differentAddressEventV2 = CreateRandomEventV2();

            EventV2 quarantinedEventV2 = CreateRandomEventV2();
            quarantinedEventV2.EventAddressV2Id = targetEventAddressV2Id;
            quarantinedEventV2.Status = EventStatusV2.Quarantined;

            IQueryable<EventV2> allEventV2s = matchingEventV2s
                .Append(differentAddressEventV2)
                .Append(quarantinedEventV2)
                .AsQueryable();

            var inputEventV2Query = new EventV2Query
            {
                EventAddressV2Id = targetEventAddressV2Id,
                Status = EventStatusV2.Active,
                Skip = 1,
                Take = 2
            };

            List<EventV2> expectedEventV2s = matchingEventV2s
                .OrderByDescending(eventV2 => eventV2.CreatedDate)
                .ThenBy(eventV2 => eventV2.Id)
                .Skip(1)
                .Take(2)
                .ToList();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sWithEventAddressV2Async(randomCancellationToken))
                    .ReturnsAsync(allEventV2s);

            // when
            IQueryable<EventV2> actualEventV2sQuery =
                await this.eventV2CoordinationService.RetrieveEventV2sWithEventAddressV2ByQueryAsync(
                    inputEventV2Query, randomCancellationToken);

            List<EventV2> actualEventV2s = actualEventV2sQuery.ToList();

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s, options =>
                options.WithStrictOrdering());

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sWithEventAddressV2Async(randomCancellationToken),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
