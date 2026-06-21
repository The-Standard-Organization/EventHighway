// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V2
{
    public partial class EventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldSubmitScheduleEventV2WhenScheduledDateIsInFutureAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomDays = GetRandomNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset retrievedDateTimeOffset = randomDateTimeOffset;
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;

            inputEventV2.ScheduledDate =
                retrievedDateTimeOffset.AddDays(randomDays);

            EventV2 inputScheduledEventV2 = inputEventV2;
            inputScheduledEventV2.Type = EventTypeV2.Scheduled;
            EventV2 submittedEventV2 = inputScheduledEventV2;
            EventV2 expectedEventV2 = submittedEventV2.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(retrievedDateTimeOffset);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.SubmitEventV2Async(
                    inputScheduledEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(submittedEventV2);

            // when
            EventV2 actualEventV2 =
                await this.eventV2CoordinationService
                    .SubmitEventV2Async(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualEventV2.Should().BeEquivalentTo(expectedEventV2);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV2Async(
                    inputScheduledEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.AddListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ScheduledDates))]
        public async Task ShouldSubmitImmediateEventV2WhenScheduledDateIsNullOrInPastAsync(
            DateTimeOffset randomDateTimeOffset,
            DateTimeOffset? scheduledDate)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var mockSequence = new MockSequence();
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.ScheduledDate = scheduledDate;
            EventV2 inputImmediateEventV2 = inputEventV2;
            inputImmediateEventV2.Type = EventTypeV2.Immediate;
            EventV2 submittedEventV2 = inputImmediateEventV2;
            EventV2 expectedEventV2 = submittedEventV2.DeepClone();

            IQueryable<EventListenerV2> randomEventListenerV2s =
                CreateRandomEventListenerV2s();

            IQueryable<EventListenerV2> retrievedEventListenerV2s =
                randomEventListenerV2s;

            List<ListenerEventV2> inputListenerEventV2s =
                retrievedEventListenerV2s.Select(eventListenerV2 =>
                    new ListenerEventV2
                    {
                        EventListenerId = eventListenerV2.Id,
                        EventId = inputImmediateEventV2.Id,
                        Status = ListenerEventStatusV2.Pending,
                        EventAddressId = inputImmediateEventV2.EventAddressId,
                        CreatedDate = randomDateTimeOffset,
                        UpdatedDate = randomDateTimeOffset
                    }).ToList();

            List<ListenerEventV2> addedListenerEventV2s =
                inputListenerEventV2s.DeepClone();

            List<ListenerEventV2> modifiedListenerEventV2s =
                addedListenerEventV2s;

            List<ListenerEventV2> expectedListenerEventV2s =
                modifiedListenerEventV2s.DeepClone();

            List<EventCallV2> expectedInputCallEventV2s =
                retrievedEventListenerV2s.AsEnumerable().Select(
                    retrievedEventListenerV2 =>
                        new EventCallV2
                        {
                            HandlerId = retrievedEventListenerV2.HandlerId,
                            HandlerName = retrievedEventListenerV2.HandlerName,
                            Content = inputImmediateEventV2.Content,
                            RequiredPromotedProperties =
                                SplitPromotedPropertyKeys(retrievedEventListenerV2.PromotedProperties),
                        }).ToList();

            int expectedDateTimeBrokerCalls =
                inputListenerEventV2s.Count +
                    modifiedListenerEventV2s.Count + 1;

            var ranEventCallV2s = new List<EventCallV2>();

            this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.eventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.SubmitEventV2Async(
                        inputImmediateEventV2,
                        randomCancellationToken))
                            .ReturnsAsync(submittedEventV2);

            this.eventListenerV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveEventListenerV2sByEventAddressIdAsync(
                        inputImmediateEventV2.EventAddressId,
                        randomCancellationToken))
                            .ReturnsAsync(retrievedEventListenerV2s);

            for (int index = 0; index < inputListenerEventV2s.Count; index++)
            {
                this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                    broker.GetDateTimeOffsetAsync())
                        .ReturnsAsync(randomDateTimeOffset);

                this.eventListenerV2OrchestrationServiceMock
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

                this.eventV2OrchestrationServiceMock
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

                this.eventListenerV2OrchestrationServiceMock
                    .InSequence(mockSequence).Setup(service =>
                        service.ModifyListenerEventV2Async(
                            It.Is(SameListenerEventAs(addedListenerEventV2s[index])),
                            randomCancellationToken))
                                .ReturnsAsync(modifiedListenerEventV2s[index]);
            }

            // when
            EventV2 actualEventV2 =
                await this.eventV2CoordinationService
                    .SubmitEventV2Async(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualEventV2.Should().BeEquivalentTo(expectedEventV2);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(callCount: expectedDateTimeBrokerCalls));

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV2Async(
                    inputImmediateEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    inputImmediateEventV2.EventAddressId,
                    randomCancellationToken),
                        Times.Once);

            for (int index = 0; index < inputListenerEventV2s.Count; index++)
            {
                this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                    service.AddListenerEventV2Async(
                        It.Is(SameListenerEventAs(inputListenerEventV2s[index])),
                        randomCancellationToken),
                            Times.Once);

                this.eventV2OrchestrationServiceMock.Verify(service =>
                    service.RunEventCallV2Async(
                        It.Is(SameEventCallAs(expectedInputCallEventV2s[index])),
                        randomCancellationToken),
                            Times.Once);

                this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                    service.ModifyListenerEventV2Async(
                        It.Is(SameListenerEventAs(addedListenerEventV2s[index])),
                        randomCancellationToken),
                            Times.Once);
            }

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ScheduledDates))]
        public async Task ShouldPromotePropertiesWhenSubmittingImmediateEventV2Async(
            DateTimeOffset randomDateTimeOffset,
            DateTimeOffset? scheduledDate)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var mockSequence = new MockSequence();
            string promotedPropertyKey1 = GetRandomString();
            string promotedPropertyKey2 = GetRandomString();
            string promotedPropertyValue1 = GetRandomString();
            string promotedPropertyValue2 = GetRandomString();
            string promotedPropertiesCsv = $"{promotedPropertyKey1},{promotedPropertyKey2}";

            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.ScheduledDate = scheduledDate;
            EventV2 inputImmediateEventV2 = inputEventV2;
            inputImmediateEventV2.Type = EventTypeV2.Immediate;
            EventV2 submittedEventV2 = inputImmediateEventV2;
            EventV2 expectedEventV2 = submittedEventV2.DeepClone();

            IQueryable<EventListenerV2> randomEventListenerV2s =
                CreateRandomEventListenerV2s(count: 1);

            EventListenerV2 retrievedEventListenerV2 =
                randomEventListenerV2s.Single();

            retrievedEventListenerV2.PromotedProperties = promotedPropertiesCsv;
            retrievedEventListenerV2.FilterCriteria = null;

            IQueryable<EventListenerV2> retrievedEventListenerV2s =
                new[] { retrievedEventListenerV2 }.AsQueryable();

            var expectedPromotedProperties = new List<PromotedProperty>
            {
                new PromotedProperty { Name = promotedPropertyKey1, Value = promotedPropertyValue1 },
                new PromotedProperty { Name = promotedPropertyKey2, Value = promotedPropertyValue2 },
            };

            ListenerEventV2 inputListenerEventV2 =
                new ListenerEventV2
                {
                    EventListenerId = retrievedEventListenerV2.Id,
                    EventId = inputImmediateEventV2.Id,
                    Status = ListenerEventStatusV2.Pending,
                    EventAddressId = inputImmediateEventV2.EventAddressId,
                    CreatedDate = randomDateTimeOffset,
                    UpdatedDate = randomDateTimeOffset
                };

            ListenerEventV2 addedListenerEventV2 =
                inputListenerEventV2.DeepClone();

            EventCallV2 expectedInputCallEventV2 =
                new EventCallV2
                {
                    HandlerId = retrievedEventListenerV2.HandlerId,
                    HandlerName = retrievedEventListenerV2.HandlerName,
                    Content = inputImmediateEventV2.Content,
                    FilterCriteria = null,
                    RequiredPromotedProperties = new[] { promotedPropertyKey1, promotedPropertyKey2 },
                    PromotedProperties = expectedPromotedProperties,
                };

            var ranEventCall = new EventCallV2
            {
                HandlerName = expectedInputCallEventV2.HandlerName,
                Content = expectedInputCallEventV2.Content,
                Response = GetRandomString(),
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString(),
                IsSuccess = true
            };

            this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.eventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.SubmitEventV2Async(
                        inputImmediateEventV2,
                        randomCancellationToken))
                            .ReturnsAsync(submittedEventV2);

            this.eventListenerV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveEventListenerV2sByEventAddressIdAsync(
                        inputImmediateEventV2.EventAddressId,
                        randomCancellationToken))
                            .ReturnsAsync(retrievedEventListenerV2s);

            this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.eventListenerV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.AddListenerEventV2Async(
                        It.Is(SameListenerEventAs(inputListenerEventV2)),
                        randomCancellationToken))
                            .ReturnsAsync(addedListenerEventV2);

            this.jsonBrokerMock.Setup(broker =>
                broker.CheckIfPropertyExist(
                    inputImmediateEventV2.Content,
                    promotedPropertyKey1))
                        .Returns(true);

            this.jsonBrokerMock.Setup(broker =>
                broker.GetJsonPropertyValue(
                    inputImmediateEventV2.Content,
                    promotedPropertyKey1))
                        .Returns(promotedPropertyValue1);

            this.jsonBrokerMock.Setup(broker =>
                broker.CheckIfPropertyExist(
                    inputImmediateEventV2.Content,
                    promotedPropertyKey2))
                        .Returns(true);

            this.jsonBrokerMock.Setup(broker =>
                broker.GetJsonPropertyValue(
                    inputImmediateEventV2.Content,
                    promotedPropertyKey2))
                        .Returns(promotedPropertyValue2);

            this.eventV2OrchestrationServiceMock
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

            this.eventListenerV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.ModifyListenerEventV2Async(
                        It.Is(SameListenerEventAs(addedListenerEventV2)),
                        randomCancellationToken))
                            .ReturnsAsync(addedListenerEventV2);

            // when
            EventV2 actualEventV2 =
                await this.eventV2CoordinationService
                    .SubmitEventV2Async(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualEventV2.Should().BeEquivalentTo(expectedEventV2);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(callCount: 3));

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV2Async(
                    inputImmediateEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    inputImmediateEventV2.EventAddressId,
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.AddListenerEventV2Async(
                    It.Is(SameListenerEventAs(inputListenerEventV2)),
                    randomCancellationToken),
                        Times.Once);

            this.jsonBrokerMock.Verify(broker =>
                broker.CheckIfPropertyExist(
                    inputImmediateEventV2.Content,
                    promotedPropertyKey1),
                        Times.Once);

            this.jsonBrokerMock.Verify(broker =>
                broker.GetJsonPropertyValue(
                    inputImmediateEventV2.Content,
                    promotedPropertyKey1),
                        Times.Once);

            this.jsonBrokerMock.Verify(broker =>
                broker.CheckIfPropertyExist(
                    inputImmediateEventV2.Content,
                    promotedPropertyKey2),
                        Times.Once);

            this.jsonBrokerMock.Verify(broker =>
                broker.GetJsonPropertyValue(
                    inputImmediateEventV2.Content,
                    promotedPropertyKey2),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.Is(SameEventCallAs(expectedInputCallEventV2)),
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.Is(SameListenerEventAs(addedListenerEventV2)),
                    randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
        [Theory]
        [MemberData(nameof(ScheduledDates))]
        public async Task ShouldSkipPromotedPropertyWhenPropertyNotFoundInContentAsync(
            DateTimeOffset randomDateTimeOffset,
            DateTimeOffset? scheduledDate)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var mockSequence = new MockSequence();
            string promotedPropertyKey1 = GetRandomString();
            string promotedPropertyKey2 = GetRandomString();
            string promotedPropertyValue1 = GetRandomString();
            string promotedPropertiesCsv = $"{promotedPropertyKey1},{promotedPropertyKey2}";

            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.ScheduledDate = scheduledDate;
            EventV2 inputImmediateEventV2 = inputEventV2;
            inputImmediateEventV2.Type = EventTypeV2.Immediate;
            EventV2 submittedEventV2 = inputImmediateEventV2;
            EventV2 expectedEventV2 = submittedEventV2.DeepClone();

            IQueryable<EventListenerV2> randomEventListenerV2s =
                CreateRandomEventListenerV2s(count: 1);

            EventListenerV2 retrievedEventListenerV2 =
                randomEventListenerV2s.Single();

            retrievedEventListenerV2.PromotedProperties = promotedPropertiesCsv;
            retrievedEventListenerV2.FilterCriteria = null;

            IQueryable<EventListenerV2> retrievedEventListenerV2s =
                new[] { retrievedEventListenerV2 }.AsQueryable();

            var expectedPromotedProperties = new List<PromotedProperty>
            {
                new PromotedProperty { Name = promotedPropertyKey1, Value = promotedPropertyValue1 },
            };

            ListenerEventV2 inputListenerEventV2 =
                new ListenerEventV2
                {
                    EventListenerId = retrievedEventListenerV2.Id,
                    EventId = inputImmediateEventV2.Id,
                    Status = ListenerEventStatusV2.Pending,
                    EventAddressId = inputImmediateEventV2.EventAddressId,
                    CreatedDate = randomDateTimeOffset,
                    UpdatedDate = randomDateTimeOffset
                };

            ListenerEventV2 addedListenerEventV2 =
                inputListenerEventV2.DeepClone();

            EventCallV2 expectedInputCallEventV2 =
                new EventCallV2
                {
                    HandlerId = retrievedEventListenerV2.HandlerId,
                    HandlerName = retrievedEventListenerV2.HandlerName,
                    Content = inputImmediateEventV2.Content,
                    FilterCriteria = null,
                    RequiredPromotedProperties = new[] { promotedPropertyKey1, promotedPropertyKey2 },
                    PromotedProperties = expectedPromotedProperties,
                };

            var ranEventCall = new EventCallV2
            {
                HandlerName = expectedInputCallEventV2.HandlerName,
                Content = expectedInputCallEventV2.Content,
                Response = GetRandomString(),
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString(),
                IsSuccess = true
            };

            this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.eventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.SubmitEventV2Async(
                        inputImmediateEventV2,
                        randomCancellationToken))
                            .ReturnsAsync(submittedEventV2);

            this.eventListenerV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveEventListenerV2sByEventAddressIdAsync(
                        inputImmediateEventV2.EventAddressId,
                        randomCancellationToken))
                            .ReturnsAsync(retrievedEventListenerV2s);

            this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.eventListenerV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.AddListenerEventV2Async(
                        It.Is(SameListenerEventAs(inputListenerEventV2)),
                        randomCancellationToken))
                            .ReturnsAsync(addedListenerEventV2);

            this.jsonBrokerMock.Setup(broker =>
                broker.CheckIfPropertyExist(
                    inputImmediateEventV2.Content,
                    promotedPropertyKey1))
                        .Returns(true);

            this.jsonBrokerMock.Setup(broker =>
                broker.GetJsonPropertyValue(
                    inputImmediateEventV2.Content,
                    promotedPropertyKey1))
                        .Returns(promotedPropertyValue1);

            this.jsonBrokerMock.Setup(broker =>
                broker.CheckIfPropertyExist(
                    inputImmediateEventV2.Content,
                    promotedPropertyKey2))
                        .Returns(false);

            this.eventV2OrchestrationServiceMock
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

            this.eventListenerV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.ModifyListenerEventV2Async(
                        It.Is(SameListenerEventAs(addedListenerEventV2)),
                        randomCancellationToken))
                            .ReturnsAsync(addedListenerEventV2);

            // when
            EventV2 actualEventV2 =
                await this.eventV2CoordinationService
                    .SubmitEventV2Async(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualEventV2.Should().BeEquivalentTo(expectedEventV2);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(callCount: 3));

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV2Async(
                    inputImmediateEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    inputImmediateEventV2.EventAddressId,
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.AddListenerEventV2Async(
                    It.Is(SameListenerEventAs(inputListenerEventV2)),
                    randomCancellationToken),
                        Times.Once);

            this.jsonBrokerMock.Verify(broker =>
                broker.CheckIfPropertyExist(
                    inputImmediateEventV2.Content,
                    promotedPropertyKey1),
                        Times.Once);

            this.jsonBrokerMock.Verify(broker =>
                broker.GetJsonPropertyValue(
                    inputImmediateEventV2.Content,
                    promotedPropertyKey1),
                        Times.Once);

            this.jsonBrokerMock.Verify(broker =>
                broker.CheckIfPropertyExist(
                    inputImmediateEventV2.Content,
                    promotedPropertyKey2),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.Is(SameEventCallAs(expectedInputCallEventV2)),
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.Is(SameListenerEventAs(addedListenerEventV2)),
                    randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidContentWithScheduledDates))]
        public async Task ShouldReturnEmptyPromotedPropertiesWhenContentIsNullOrInvalidJsonAsync(
            DateTimeOffset randomDateTimeOffset,
            DateTimeOffset? scheduledDate,
            string invalidContent)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var mockSequence = new MockSequence();
            string promotedPropertyKey = GetRandomString();
            string promotedPropertiesCsv = promotedPropertyKey;

            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.ScheduledDate = scheduledDate;
            inputEventV2.Content = invalidContent;
            EventV2 inputImmediateEventV2 = inputEventV2;
            inputImmediateEventV2.Type = EventTypeV2.Immediate;
            EventV2 submittedEventV2 = inputImmediateEventV2;
            EventV2 expectedEventV2 = submittedEventV2.DeepClone();

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
                    EventListenerId = retrievedEventListenerV2.Id,
                    EventId = inputImmediateEventV2.Id,
                    Status = ListenerEventStatusV2.Pending,
                    EventAddressId = inputImmediateEventV2.EventAddressId,
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

            this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.eventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.SubmitEventV2Async(
                        inputImmediateEventV2,
                        randomCancellationToken))
                            .ReturnsAsync(submittedEventV2);

            this.eventListenerV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveEventListenerV2sByEventAddressIdAsync(
                        inputImmediateEventV2.EventAddressId,
                        randomCancellationToken))
                            .ReturnsAsync(retrievedEventListenerV2s);

            this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.eventListenerV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.AddListenerEventV2Async(
                        It.Is(SameListenerEventAs(inputListenerEventV2)),
                        randomCancellationToken))
                            .ReturnsAsync(addedListenerEventV2);

            if (invalidContent != null)
            {
                this.jsonBrokerMock.Setup(broker =>
                    broker.CheckIfPropertyExist(invalidContent, It.IsAny<string>()))
                        .Throws<JsonException>();
            }

            this.eventV2OrchestrationServiceMock
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

            this.eventListenerV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.ModifyListenerEventV2Async(
                        It.Is(SameListenerEventAs(addedListenerEventV2)),
                        randomCancellationToken))
                            .ReturnsAsync(addedListenerEventV2);

            // when
            EventV2 actualEventV2 =
                await this.eventV2CoordinationService
                    .SubmitEventV2Async(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualEventV2.Should().BeEquivalentTo(expectedEventV2);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(callCount: 3));

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV2Async(
                    inputImmediateEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    inputImmediateEventV2.EventAddressId,
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.AddListenerEventV2Async(
                    It.Is(SameListenerEventAs(inputListenerEventV2)),
                    randomCancellationToken),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.Is(SameEventCallAs(expectedInputCallEventV2)),
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.Is(SameListenerEventAs(addedListenerEventV2)),
                    randomCancellationToken),
                        Times.Once);

            if (invalidContent != null)
            {
                this.jsonBrokerMock.Verify(broker =>
                    broker.CheckIfPropertyExist(invalidContent, promotedPropertyKey),
                        Times.Once);
            }

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
