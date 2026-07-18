// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventFirings.V2
{
    public partial class EventFiringV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldContinueFiringRemainingListenersWhenOneListenerFailsAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;

            IQueryable<EventListenerV2> retrievedEventListenerV2s =
                CreateRandomEventListenerV2s(count: 2);

            EventListenerV2 failingEventListenerV2 = retrievedEventListenerV2s.First();
            EventListenerV2 succeedingEventListenerV2 = retrievedEventListenerV2s.Last();

            var addListenerEventV2Exception = new Exception(message: GetRandomString());

            this.eventListenerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    inputEventV2.EventAddressV2Id,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventListenerV2s);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.AddListenerEventV2Async(
                    It.Is<ListenerEventV2>(listenerEventV2 =>
                        listenerEventV2.EventListenerV2Id == failingEventListenerV2.Id),
                    randomCancellationToken))
                        .ThrowsAsync(addListenerEventV2Exception);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.AddListenerEventV2Async(
                    It.Is<ListenerEventV2>(listenerEventV2 =>
                        listenerEventV2.EventListenerV2Id == succeedingEventListenerV2.Id),
                    randomCancellationToken))
                        .ReturnsAsync((ListenerEventV2 listenerEventV2, CancellationToken _) =>
                            listenerEventV2);

            var ranEventCallV2 = new EventCallV2
            {
                IsSuccess = true,
                Response = GetRandomString(),
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString()
            };

            this.eventCallV2ProcessingServiceMock.Setup(service =>
                service.RunEventCallV2Async(
                    It.Is<EventCallV2>(eventCallV2 =>
                        eventCallV2.HandlerId == succeedingEventListenerV2.HandlerId),
                    randomCancellationToken))
                        .ReturnsAsync(ranEventCallV2);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventV2Async(
                    It.Is<ListenerEventV2>(listenerEventV2 =>
                        listenerEventV2.EventListenerV2Id == succeedingEventListenerV2.Id),
                    randomCancellationToken))
                        .ReturnsAsync((ListenerEventV2 listenerEventV2, CancellationToken _) =>
                            listenerEventV2);

            // when
            EventV2 actualEventV2 =
                await this.eventFiringV2OrchestrationService
                    .FireEventV2Async(inputEventV2, randomCancellationToken);

            // then
            actualEventV2.ListenerEventV2s.Should().ContainSingle();

            actualEventV2.ListenerEventV2s.Single().EventListenerV2Id
                .Should().Be(succeedingEventListenerV2.Id);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.AddListenerEventV2Async(
                    It.Is<ListenerEventV2>(listenerEventV2 =>
                        listenerEventV2.EventListenerV2Id == succeedingEventListenerV2.Id),
                    randomCancellationToken),
                        Times.Once);

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.Is<EventCallV2>(eventCallV2 =>
                        eventCallV2.HandlerId == succeedingEventListenerV2.HandlerId),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.Is<ListenerEventV2>(listenerEventV2 =>
                        listenerEventV2.EventListenerV2Id == succeedingEventListenerV2.Id),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(addListenerEventV2Exception),
                    Times.Once);
        }
    }
}
