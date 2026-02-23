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
        public async Task ShouldRetrieveAllDeadEventV1sWithListenersAsync()
        {
            // given
            List<EventV1> randomScheduledEventV1s =
                CreateRandomEventV1s(
                    dates: GetRandomDateTimeOffset(),
                    eventV1Type: EventV1Type.Scheduled)
                        .ToList();

            List<EventV1> randomImmediateEventV1s =
                CreateRandomEventV1s(
                    dates: GetRandomDateTimeOffset(),
                    eventV1Type: EventV1Type.Immediate)
                        .ToList();

            IQueryable<EventV1> retrievedEventV1s =
                randomScheduledEventV1s.Union(
                    randomImmediateEventV1s)
                        .AsQueryable();

            IQueryable<EventV1> expectedEventV1s =
                randomImmediateEventV1s.AsQueryable();

            this.eventV1ServiceMock.Setup(service =>
                service.RetrieveAllEventV1sWithListenerEventV1sAsync())
                    .ReturnsAsync(retrievedEventV1s);

            // when
            IQueryable<EventV1> actualEventV1s =
                await this.eventV1ProcessingService
                    .RetrieveAllDeadEventV1sWithListenersAsync();

            // then
            actualEventV1s.Should().BeEquivalentTo(expectedEventV1s);

            this.eventV1ServiceMock.Verify(service =>
                service.RetrieveAllEventV1sWithListenerEventV1sAsync(),
                    Times.Once);

            this.eventV1ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
