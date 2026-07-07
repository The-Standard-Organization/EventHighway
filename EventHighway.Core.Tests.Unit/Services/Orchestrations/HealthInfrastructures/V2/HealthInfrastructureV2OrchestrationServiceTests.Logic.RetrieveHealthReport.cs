// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.HealthInfrastructures.V2
{
    public partial class HealthInfrastructureV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveHealthReportV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomEnum<TrafficPeriodV2>();
            DateTimeOffset inputWindowStart = GetRandomDateTimeOffset();

            List<EventAddressV2> randomEventAddresses =
                CreateRandomEventAddressV2s(count: GetRandomNumber());

            List<EventParticipantV2> randomEventParticipants =
                CreateRandomEventParticipantV2s(count: GetRandomNumber());

            int distinctHandlerCount = GetRandomNumber();

            List<Guid> handlerIds = Enumerable.Range(start: 0, count: distinctHandlerCount)
                .Select(item => GetRandomId())
                .ToList();

            // A second listener re-uses the first handler id, so listener count > distinct handler count.
            handlerIds.Add(handlerIds.First());

            List<EventListenerV2> randomEventListeners =
                CreateRandomEventListenerV2s(
                    handlerIds,
                    randomEventAddresses,
                    randomEventParticipants);

            var expectedHealthReport = new HealthReportV2
            {
                Period = inputPeriod,
                WindowStart = inputWindowStart,

                HealthCheckItems = new List<HealthCheckItemV2>
                {
                    new HealthCheckItemV2
                    {
                        Grouping = "Infrastructure",
                        Item = "Total Event Addresses",
                        Value = randomEventAddresses.Count.ToString(),
                        Description = "Total number of registered event addresses.",
                        StatusCode = (int)HealthStatusV2.NA,
                        Status = nameof(HealthStatusV2.NA)
                    },

                    new HealthCheckItemV2
                    {
                        Grouping = "Infrastructure",
                        Item = "Total Event Listeners",
                        Value = randomEventListeners.Count.ToString(),
                        Description = "Total number of registered event listeners.",
                        StatusCode = (int)HealthStatusV2.NA,
                        Status = nameof(HealthStatusV2.NA)
                    },

                    new HealthCheckItemV2
                    {
                        Grouping = "Infrastructure",
                        Item = "Total Participants",
                        Value = randomEventParticipants.Count.ToString(),
                        Description = "Total number of registered participants.",
                        StatusCode = (int)HealthStatusV2.NA,
                        Status = nameof(HealthStatusV2.NA)
                    },

                    new HealthCheckItemV2
                    {
                        Grouping = "Infrastructure",
                        Item = "Registered Handlers",
                        Value = distinctHandlerCount.ToString(),
                        Description = "Number of distinct registered event handlers.",
                        StatusCode = (int)HealthStatusV2.NA,
                        Status = nameof(HealthStatusV2.NA)
                    }
                },

                AddressUsage = randomEventAddresses
                    .Select(eventAddress => new EventAddressUsageV2
                    {
                        EventAddressV2Id = eventAddress.Id,
                        Name = eventAddress.Name,
                        Description = eventAddress.Description,

                        ActiveListeners = randomEventListeners
                            .Count(listener => listener.EventAddressV2Id == eventAddress.Id)
                    })
                    .ToList(),

                ParticipantUsage = randomEventParticipants
                    .Select(participant => new ParticipantUsageV2
                    {
                        EventParticipantV2Id = participant.Id,
                        Name = participant.Name,
                        ContactEmail = participant.ContactEmail,
                        ContactPhone = participant.ContactPhone,
                        IsActive = participant.IsActive,

                        OwnedListeners = randomEventListeners
                            .Count(listener => listener.EventParticipantV2Id == participant.Id)
                    })
                    .ToList()
            };

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventAddresses.AsQueryable());

            this.eventListenerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventListeners.AsQueryable());

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventParticipants.AsQueryable());

            // when
            HealthReportV2 actualHealthReport =
                await this.healthInfrastructureV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.Should().BeEquivalentTo(expectedHealthReport);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
