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
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventFirings.V2
{
    public partial class EventFiringV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRecordErrorWhenPromotedPropertyKeySplitThrowsOnFireEventV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string promotedPropertiesCsv = GetRandomString();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;

            IQueryable<EventListenerV2> randomEventListenerV2s =
                CreateRandomEventListenerV2s(count: 1);

            EventListenerV2 retrievedEventListenerV2 =
                randomEventListenerV2s.Single();

            retrievedEventListenerV2.PromotedProperties = promotedPropertiesCsv;
            retrievedEventListenerV2.FilterCriteria = null;

            IQueryable<EventListenerV2> retrievedEventListenerV2s =
                new[] { retrievedEventListenerV2 }.AsQueryable();

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
                    UpdatedDate = randomDateTimeOffset
                };

            ListenerEventV2 addedListenerEventV2 =
                inputListenerEventV2.DeepClone();

            var splitException = new Exception(message: GetRandomString());

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
                    It.Is(SameListenerEventAs(inputListenerEventV2)),
                    randomCancellationToken))
                        .ReturnsAsync(addedListenerEventV2);

            this.eventCallV2ProcessingServiceMock.Setup(service =>
                service.SplitPromotedPropertyKeysAsync(
                    promotedPropertiesCsv,
                    randomCancellationToken))
                        .ThrowsAsync(splitException);

            ListenerEventV2 erroredListenerEventV2 = addedListenerEventV2.DeepClone();
            erroredListenerEventV2.Status = ListenerEventStatusV2.Error;
            erroredListenerEventV2.Response = splitException.Message;
            erroredListenerEventV2.UpdatedDate = randomDateTimeOffset;

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventV2Async(
                    It.Is(SameListenerEventAs(erroredListenerEventV2)),
                    randomCancellationToken))
                        .ReturnsAsync((ListenerEventV2 listenerEventV2, CancellationToken _) =>
                            listenerEventV2);

            // when
            EventV2 actualEventV2 =
                await this.eventFiringV2OrchestrationService
                    .FireEventV2Async(inputEventV2, randomCancellationToken);

            // then
            actualEventV2.ListenerEventV2s.Should().ContainSingle();

            actualEventV2.ListenerEventV2s.Single().Status
                .Should().Be(ListenerEventStatusV2.Error);

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.SplitPromotedPropertyKeysAsync(
                    promotedPropertiesCsv,
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.Is(SameListenerEventAs(erroredListenerEventV2)),
                    randomCancellationToken),
                        Times.Once);

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.PromotePropertiesAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(splitException),
                    Times.Never);
        }
    }
}
