// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V1
{
    public partial class EventV1ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveScheduledPendingEventV1sAsync()
        {
            // given
            int randomDaysAgo = GetNegativeRandomNumber();

            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            DateTimeOffset retrievedDateTimeOffset =
                randomDateTimeOffset;

            DateTimeOffset scheduledDateTimeOffset =
                retrievedDateTimeOffset.AddDays(randomDaysAgo);

            List<EventV1> randomEventV1s =
                CreateRandomEventV1s(
                    dates: scheduledDateTimeOffset,
                    eventV1Type: EventTypeV1.Scheduled)
                        .ToList();

            List<EventV1> randomOtherEventV1s =
                CreateRandomEventV1s().ToList();

            IQueryable<EventV1> retrievedEventV1s =
                randomEventV1s.Union(randomOtherEventV1s)
                    .AsQueryable();

            IQueryable<EventV1> expectedEventV1s =
                randomEventV1s.AsQueryable();

            this.eventV1ServiceMock.Setup(service =>
                service.RetrieveAllEventV1sAsync())
                    .ReturnsAsync(retrievedEventV1s);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(retrievedDateTimeOffset);

            // when
            IQueryable<EventV1> actualEventV1s =
                await this.eventV1ProcessingService
                    .RetrieveScheduledPendingEventV1sAsync();

            // then
            actualEventV1s.Should().BeEquivalentTo(expectedEventV1s);

            this.eventV1ServiceMock.Verify(service =>
                service.RetrieveAllEventV1sAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventV1ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
