// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveBatchOfDeadEventV2sWithTakeAsync()
        {
            // given
            int randomTake = GetRandomNumber();
            int inputTake = randomTake;

            List<EventV2> randomScheduledEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Scheduled)
                        .ToList();

            List<EventV2> randomImmediateEventV2sWithRetries =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Immediate)
                        .ToList();

            randomImmediateEventV2sWithRetries.ForEach(eventV2 =>
                eventV2.RemainingRetryAttempts = GetRandomNumber());

            List<EventV2> randomDeadEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Immediate)
                        .ToList();

            randomDeadEventV2s.ForEach(eventV2 =>
            {
                eventV2.RemainingRetryAttempts = 0;
                eventV2.ListenerEventV2s = new List<ListenerEventV2>();
            });

            IQueryable<EventV2> retrievedEventV2s =
                randomScheduledEventV2s
                    .Union(randomImmediateEventV2sWithRetries)
                    .Union(randomDeadEventV2s)
                        .AsQueryable();

            IEnumerable<EventV2> expectedEventV2s =
                randomDeadEventV2s.Take(inputTake);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync())
                    .ReturnsAsync(retrievedEventV2s);

            // when
            IEnumerable<EventV2> actualEventV2s =
                await this.eventV2ProcessingService
                    .RetrieveBatchOfDeadEventV2sAsync(inputTake);

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveBatchOfDeadEventV2sWithoutTakeAsync()
        {
            // given
            int inputTake = 0;

            List<EventV2> randomScheduledEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Scheduled)
                        .ToList();

            List<EventV2> randomImmediateEventV2sWithRetries =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Immediate)
                        .ToList();

            randomImmediateEventV2sWithRetries.ForEach(eventV2 =>
                eventV2.RemainingRetryAttempts = GetRandomNumber());

            List<EventV2> randomDeadEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Immediate)
                        .ToList();

            randomDeadEventV2s.ForEach(eventV2 =>
            {
                eventV2.RemainingRetryAttempts = 0;
                eventV2.ListenerEventV2s = new List<ListenerEventV2>();
            });

            IQueryable<EventV2> retrievedEventV2s =
                randomScheduledEventV2s
                    .Union(randomImmediateEventV2sWithRetries)
                    .Union(randomDeadEventV2s)
                        .AsQueryable();

            IEnumerable<EventV2> expectedEventV2s =
                randomDeadEventV2s.AsEnumerable();

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync())
                    .ReturnsAsync(retrievedEventV2s);

            // when
            IEnumerable<EventV2> actualEventV2s =
                await this.eventV2ProcessingService
                    .RetrieveBatchOfDeadEventV2sAsync(inputTake);

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
