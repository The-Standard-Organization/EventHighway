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
using EventHighway.Core.Models.Services.Foundations.PromotedProperties;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventFirings.V2
{
    public partial class EventFiringV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldFireEventV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var mockSequence = new MockSequence();

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;

            IQueryable<EventListenerV2> randomEventListenerV2s =
                CreateRandomEventListenerV2s();

            IQueryable<EventListenerV2> retrievedEventListenerV2s =
                randomEventListenerV2s;

            List<ListenerEventV2> inputListenerEventV2s =
                retrievedEventListenerV2s.Select(eventListenerV2 =>
                    new ListenerEventV2
                    {
                        EventListenerV2Id = eventListenerV2.Id,
                        EventV2Id = inputEventV2.Id,
                        Status = ListenerEventStatusV2.Pending,
                        EventAddressV2Id = inputEventV2.EventAddressV2Id,
                        EventParticipantV2Id = eventListenerV2.EventParticipantV2Id,
                        RemainingRetryAttempts = this.retryConfiguration.RetryAttemptsAllowed,
                        RetryAttemptsAllowed = this.retryConfiguration.RetryAttemptsAllowed,
                        NextRetryAttemptNotBefore = null,
                        DispatchedDate = randomDateTimeOffset,
                        CreatedDate = randomDateTimeOffset,
                        UpdatedDate = randomDateTimeOffset
                    }).ToList();

            List<ListenerEventV2> addedListenerEventV2s =
                inputListenerEventV2s.DeepClone();

            List<EventCallV2> expectedInputCallEventV2s =
                retrievedEventListenerV2s.AsEnumerable().Select(
                    retrievedEventListenerV2 =>
                        new EventCallV2
                        {
                            HandlerId = retrievedEventListenerV2.HandlerId,
                            HandlerName = retrievedEventListenerV2.HandlerName,
                            Content = inputEventV2.Content,
                            RequiredPromotedProperties =
                                SplitPromotedPropertyKeys(retrievedEventListenerV2.PromotedProperties),
                            PromotedProperties = new List<PromotedProperty>(),
                        }).ToList();

            int expectedDateTimeBrokerCalls =
                inputListenerEventV2s.Count * 2;

            var ranEventCallV2s = new List<EventCallV2>();
            var expectedListenerEventV2s = new List<ListenerEventV2>();

            this.eventListenerV2ProcessingServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveEventListenerV2sByEventAddressIdAsync(
                        inputEventV2.EventAddressV2Id,
                        randomCancellationToken))
                            .ReturnsAsync(retrievedEventListenerV2s);

            for (int index = 0; index < inputListenerEventV2s.Count; index++)
            {
                this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                    broker.GetDateTimeOffsetAsync())
                        .ReturnsAsync(randomDateTimeOffset);

                this.listenerEventV2ProcessingServiceMock
                    .InSequence(mockSequence).Setup(service =>
                        service.AddListenerEventV2Async(
                            It.Is(SameListenerEventAs(inputListenerEventV2s[index])),
                            randomCancellationToken))
                                .ReturnsAsync(addedListenerEventV2s[index]);

                var ranEventCall = new EventCallV2
                {
                    HandlerName = expectedInputCallEventV2s[index].HandlerName,
                    Content = expectedInputCallEventV2s[index].Content,
                    Response = GetRandomString(),
                    ResponseCode = GetRandomString(),
                    ResponseMessage = GetRandomString(),
                    IsSuccess = true
                };

                this.eventCallV2ProcessingServiceMock
                    .InSequence(mockSequence).Setup(service =>
                        service.RunEventCallV2Async(
                            It.Is(SameEventCallAs(expectedInputCallEventV2s[index])),
                            randomCancellationToken))
                                .ReturnsAsync(ranEventCall);

                ranEventCallV2s.Add(item: ranEventCall);

                this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                    broker.GetDateTimeOffsetAsync())
                        .ReturnsAsync(randomDateTimeOffset);

                addedListenerEventV2s[index].UpdatedDate = randomDateTimeOffset;
                addedListenerEventV2s[index].Status = ListenerEventStatusV2.Success;
                addedListenerEventV2s[index].Response = ranEventCallV2s[index].Response;
                addedListenerEventV2s[index].ResponseCode = ranEventCallV2s[index].ResponseCode;
                addedListenerEventV2s[index].ResponseMessage = ranEventCallV2s[index].ResponseMessage;

                ListenerEventV2 modifiedListenerEventV2 =
                    addedListenerEventV2s[index].DeepClone();

                this.listenerEventV2ProcessingServiceMock
                    .InSequence(mockSequence).Setup(service =>
                        service.ModifyListenerEventV2Async(
                            It.Is(SameListenerEventAs(addedListenerEventV2s[index])),
                            randomCancellationToken))
                                .ReturnsAsync(modifiedListenerEventV2);

                expectedListenerEventV2s.Add(item: modifiedListenerEventV2);
            }

            EventV2 expectedEventV2 = inputEventV2.DeepClone();
            expectedEventV2.ListenerEventV2s = expectedListenerEventV2s;

            // when
            EventV2 actualEventV2 =
                await this.eventFiringV2OrchestrationService
                    .FireEventV2Async(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualEventV2.Should().BeEquivalentTo(expectedEventV2);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    inputEventV2.EventAddressV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(callCount: expectedDateTimeBrokerCalls));

            this.configurationBrokerMock.Verify(broker =>
                broker.GetRetryConfiguration(),
                    Times.Exactly(callCount: inputListenerEventV2s.Count));

            for (int index = 0; index < inputListenerEventV2s.Count; index++)
            {
                this.listenerEventV2ProcessingServiceMock.Verify(service =>
                    service.AddListenerEventV2Async(
                        It.Is(SameListenerEventAs(inputListenerEventV2s[index])),
                        randomCancellationToken),
                            Times.Once);

                this.eventCallV2ProcessingServiceMock.Verify(service =>
                    service.RunEventCallV2Async(
                        It.Is(SameEventCallAs(expectedInputCallEventV2s[index])),
                        randomCancellationToken),
                            Times.Once);

                this.listenerEventV2ProcessingServiceMock.Verify(service =>
                    service.ModifyListenerEventV2Async(
                        It.Is(SameListenerEventAs(addedListenerEventV2s[index])),
                        randomCancellationToken),
                            Times.Once);
            }

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
