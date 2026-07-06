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
        public async Task ShouldRetrieveInfrastructureHealthV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int addressCount = GetRandomNumber();
            int participantCount = GetRandomNumber();
            int distinctHandlerCount = GetRandomNumber();

            Guid sharedHandlerId = GetRandomId();

            List<Guid> handlerIds = Enumerable.Range(start: 0, count: distinctHandlerCount)
                .Select(item => GetRandomId())
                .ToList();

            // A second listener re-uses the first handler id, so listener count > distinct handler count.
            handlerIds.Add(handlerIds.First());

            IQueryable<EventAddressV2> randomEventAddresses =
                CreateRandomEventAddressV2s(addressCount);

            IQueryable<EventParticipantV2> randomEventParticipants =
                CreateRandomEventParticipantV2s(participantCount);

            IQueryable<EventListenerV2> randomEventListeners =
                CreateEventListenerV2sWithHandlerIds(handlerIds);

            var expectedInfrastructureHealth = new InfrastructureHealthV2
            {
                TotalEventAddresses = addressCount,
                TotalEventListeners = handlerIds.Count,
                TotalParticipants = participantCount,
                RegisteredHandlers = distinctHandlerCount,
                HandlerStatus = HealthStatusV2.NA
            };

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventAddresses);

            this.eventListenerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventListeners);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventParticipants);

            // when
            InfrastructureHealthV2 actualInfrastructureHealth =
                await this.healthInfrastructureV2OrchestrationService
                    .RetrieveInfrastructureHealthV2Async(randomCancellationToken);

            // then
            actualInfrastructureHealth.Should()
                .BeEquivalentTo(expectedInfrastructureHealth);

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
