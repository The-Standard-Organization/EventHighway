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
        public static TheoryData<string> InvalidContents()
        {
            return new TheoryData<string>
            {
                null,
                "not valid json",
            };
        }

        [Theory]
        [MemberData(nameof(InvalidContents))]
        public async Task ShouldReturnEmptyPromotedPropertiesWhenContentIsNullOrInvalidJsonWhenFiringEventV2Async(
            string invalidContent)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var mockSequence = new MockSequence();

            string promotedPropertyKey = GetRandomString();
            string promotedPropertiesCsv = promotedPropertyKey;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.Content = invalidContent;

            IQueryable<EventListenerV2> randomEventListenerV2s =
                CreateRandomEventListenerV2s(count: 1);

            EventListenerV2 retrievedEventListenerV2 =
                randomEventListenerV2s.Single();

            retrievedEventListenerV2.PromotedProperties = promotedPropertiesCsv;
            retrievedEventListenerV2.FilterCriteria = null;

            IQueryable<EventListenerV2> retrievedEventListenerV2s =
                new[] { retrievedEventListenerV2 }.AsQueryable();

            EventCallV2 expectedInputCallEventV2 =
                new EventCallV2
                {
                    HandlerId = retrievedEventListenerV2.HandlerId,
                    HandlerName = retrievedEventListenerV2.HandlerName,
                    Content = invalidContent,
                    FilterCriteria = null,
                    RequiredPromotedProperties = new[] { promotedPropertyKey },
                    PromotedProperties = new List<PromotedProperty>(),
                };

            ListenerEventV2 inputListenerEventV2 =
                new ListenerEventV2
                {
                    EventListenerV2Id = retrievedEventListenerV2.Id,
                    EventParticipantV2Id = retrievedEventListenerV2.EventParticipantV2Id,
                    EventV2Id = inputEventV2.Id,
                    Status = ListenerEventStatusV2.Pending,
                    EventAddressV2Id = inputEventV2.EventAddressV2Id,
                    RemainingRetryAttempts = this.retryConfiguration.RetryAttemptsAllowed,
                    RetryAttemptsAllowed = this.retryConfiguration.RetryAttemptsAllowed,
                    NextRetryAttemptNotBefore = null,
                    DispatchedDate = randomDateTimeOffset,
                    CreatedDate = randomDateTimeOffset,
                    UpdatedDate = randomDateTimeOffset,
                };

            ListenerEventV2 addedListenerEventV2 =
                inputListenerEventV2.DeepClone();

            var ranEventCall = new EventCallV2
            {
                HandlerName = expectedInputCallEventV2.HandlerName,
                Content = expectedInputCallEventV2.Content,
                Response = GetRandomString(),
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString(),
                IsSuccess = true
            };

            this.eventListenerV2ProcessingServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveEventListenerV2sByEventAddressIdAsync(
                        inputEventV2.EventAddressV2Id,
                        randomCancellationToken))
                            .ReturnsAsync(retrievedEventListenerV2s);

            this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.listenerEventV2ProcessingServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.AddListenerEventV2Async(
                        It.Is(SameListenerEventAs(inputListenerEventV2)),
                        randomCancellationToken))
                            .ReturnsAsync(addedListenerEventV2);

            this.eventCallV2ProcessingServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.SplitPromotedPropertyKeysAsync(
                        promotedPropertiesCsv,
                        randomCancellationToken))
                            .ReturnsAsync(new[] { promotedPropertyKey });

            if (invalidContent != null)
            {
                this.eventCallV2ProcessingServiceMock
                    .InSequence(mockSequence).Setup(service =>
                        service.PromotePropertiesAsync(
                            invalidContent,
                            promotedPropertiesCsv,
                            randomCancellationToken))
                                .ReturnsAsync(new List<PromotedProperty>());
            }

            this.eventCallV2ProcessingServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RunEventCallV2Async(
                        It.Is(SameEventCallAs(expectedInputCallEventV2)),
                        randomCancellationToken))
                            .ReturnsAsync(ranEventCall);

            this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            addedListenerEventV2.UpdatedDate = randomDateTimeOffset;
            addedListenerEventV2.Status = ListenerEventStatusV2.Success;
            addedListenerEventV2.Response = ranEventCall.Response;
            addedListenerEventV2.ResponseCode = ranEventCall.ResponseCode;
            addedListenerEventV2.ResponseMessage = ranEventCall.ResponseMessage;

            ListenerEventV2 modifiedListenerEventV2 =
                addedListenerEventV2.DeepClone();

            this.listenerEventV2ProcessingServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.ModifyListenerEventV2Async(
                        It.Is(SameListenerEventAs(addedListenerEventV2)),
                        randomCancellationToken))
                            .ReturnsAsync(modifiedListenerEventV2);

            EventV2 expectedEventV2 = inputEventV2.DeepClone();

            expectedEventV2.ListenerEventV2s =
                new List<ListenerEventV2> { modifiedListenerEventV2 };

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
                    Times.Exactly(callCount: 2));

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.AddListenerEventV2Async(
                    It.Is(SameListenerEventAs(inputListenerEventV2)),
                    randomCancellationToken),
                        Times.Once);

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.SplitPromotedPropertyKeysAsync(
                    promotedPropertiesCsv,
                    randomCancellationToken),
                        Times.Once);

            if (invalidContent != null)
            {
                this.eventCallV2ProcessingServiceMock.Verify(service =>
                    service.PromotePropertiesAsync(
                        invalidContent,
                        promotedPropertiesCsv,
                        randomCancellationToken),
                            Times.Once);
            }

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.Is(SameEventCallAs(expectedInputCallEventV2)),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.Is(SameListenerEventAs(addedListenerEventV2)),
                    randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
