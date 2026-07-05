// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.Retries;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllDeadEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomNow = GetRandomDateTimeOffset();
            RetryConfiguration retryConfiguration = CreateRandomRetryConfiguration();

            DateTimeOffset cutoff =
                randomNow.AddMinutes(-retryConfiguration.DeadAfterMinutes);

            DateTimeOffset pastGraceDate = cutoff.AddMinutes(-GetRandomNumber());

            List<EventV2> randomScheduledEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Scheduled)
                        .ToList();

            List<EventV2> randomDeadEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Immediate)
                        .ToList();

            randomDeadEventV2s.ForEach(eventV2 =>
                eventV2.ListenerEventV2s = new List<ListenerEventV2>
                {
                    new ListenerEventV2
                    {
                        Status = ListenerEventStatusV2.Error,
                        RemainingRetryAttempts = 0,
                        DispatchedDate = pastGraceDate
                    }
                });

            List<EventV2> randomRetriableEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Immediate)
                        .ToList();

            randomRetriableEventV2s.ForEach(eventV2 =>
                eventV2.ListenerEventV2s = new List<ListenerEventV2>
                {
                    new ListenerEventV2
                    {
                        Status = ListenerEventStatusV2.Error,
                        RemainingRetryAttempts = GetRandomNumber(),
                        DispatchedDate = pastGraceDate
                    }
                });

            IQueryable<EventV2> retrievedEventV2s =
                randomScheduledEventV2s
                    .Union(randomDeadEventV2s)
                    .Union(randomRetriableEventV2s)
                        .AsQueryable();

            IQueryable<EventV2> expectedEventV2s =
                randomDeadEventV2s.AsQueryable();

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventV2s);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomNow);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetRetryConfiguration())
                    .Returns(retryConfiguration);

            // when
            IQueryable<EventV2> actualEventV2s =
                await this.eventV2ProcessingService
                    .RetrieveAllDeadEventV2sAsync(randomCancellationToken);

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetRetryConfiguration(),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldIncludeAllSuccessEventV2sInDeadEventV2sWithListenersAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomNow = GetRandomDateTimeOffset();
            RetryConfiguration retryConfiguration = CreateRandomRetryConfiguration();

            List<EventV2> randomSuccessfulEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Immediate)
                        .ToList();

            randomSuccessfulEventV2s.ForEach(eventV2 =>
                eventV2.ListenerEventV2s = new List<ListenerEventV2>
                {
                    new ListenerEventV2
                    {
                        Status = ListenerEventStatusV2.Success,
                        RemainingRetryAttempts = GetRandomNumber()
                    }
                });

            List<EventV2> randomFailedEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Immediate)
                        .ToList();

            randomFailedEventV2s.ForEach(eventV2 =>
                eventV2.ListenerEventV2s = new List<ListenerEventV2>
                {
                    new ListenerEventV2
                    {
                        Status = ListenerEventStatusV2.Error,
                        RemainingRetryAttempts = GetRandomNumber()
                    }
                });

            IQueryable<EventV2> retrievedEventV2s =
                randomSuccessfulEventV2s
                    .Union(randomFailedEventV2s)
                        .AsQueryable();

            IQueryable<EventV2> expectedEventV2s =
                randomSuccessfulEventV2s.AsQueryable();

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventV2s);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomNow);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetRetryConfiguration())
                    .Returns(retryConfiguration);

            // when
            IQueryable<EventV2> actualEventV2s =
                await this.eventV2ProcessingService
                    .RetrieveAllDeadEventV2sAsync(randomCancellationToken);

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetRetryConfiguration(),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExcludeReplayEventV2sFromDeadEventV2sWithListenersAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomNow = GetRandomDateTimeOffset();
            RetryConfiguration retryConfiguration = CreateRandomRetryConfiguration();

            DateTimeOffset cutoff =
                randomNow.AddMinutes(-retryConfiguration.DeadAfterMinutes);

            DateTimeOffset pastGraceDate = cutoff.AddMinutes(-GetRandomNumber());

            List<EventV2> randomDeadEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Immediate)
                        .ToList();

            randomDeadEventV2s.ForEach(eventV2 =>
                eventV2.ListenerEventV2s = new List<ListenerEventV2>
                {
                    new ListenerEventV2
                    {
                        Status = ListenerEventStatusV2.Error,
                        RemainingRetryAttempts = 0,
                        DispatchedDate = pastGraceDate
                    }
                });

            List<EventV2> randomReplayEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Immediate)
                        .ToList();

            randomReplayEventV2s.ForEach(eventV2 =>
                eventV2.ListenerEventV2s = new List<ListenerEventV2>
                {
                    new ListenerEventV2
                    {
                        Status = ListenerEventStatusV2.Replay,
                        RemainingRetryAttempts = 0,
                        DispatchedDate = pastGraceDate
                    }
                });

            IQueryable<EventV2> retrievedEventV2s =
                randomDeadEventV2s
                    .Union(randomReplayEventV2s)
                        .AsQueryable();

            IQueryable<EventV2> expectedEventV2s =
                randomDeadEventV2s.AsQueryable();

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventV2s);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomNow);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetRetryConfiguration())
                    .Returns(retryConfiguration);

            // when
            IQueryable<EventV2> actualEventV2s =
                await this.eventV2ProcessingService
                    .RetrieveAllDeadEventV2sAsync(randomCancellationToken);

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetRetryConfiguration(),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldExcludeStillRetriableOrInGraceEventV2sFromDeadEventV2sWithListenersAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomNow = GetRandomDateTimeOffset();
            RetryConfiguration retryConfiguration = CreateRandomRetryConfiguration();

            DateTimeOffset cutoff =
                randomNow.AddMinutes(-retryConfiguration.DeadAfterMinutes);

            DateTimeOffset pastGraceDate = cutoff.AddMinutes(-GetRandomNumber());

            List<EventV2> randomDeadEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Immediate)
                        .ToList();

            randomDeadEventV2s.ForEach(eventV2 =>
                eventV2.ListenerEventV2s = new List<ListenerEventV2>
                {
                    new ListenerEventV2
                    {
                        Status = ListenerEventStatusV2.Error,
                        RemainingRetryAttempts = 0,
                        DispatchedDate = pastGraceDate
                    }
                });

            List<EventV2> randomStillRetriableEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Immediate)
                        .ToList();

            randomStillRetriableEventV2s.ForEach(eventV2 =>
                eventV2.ListenerEventV2s = new List<ListenerEventV2>
                {
                    new ListenerEventV2
                    {
                        Status = ListenerEventStatusV2.Error,
                        RemainingRetryAttempts = GetRandomNumber(),
                        DispatchedDate = pastGraceDate
                    }
                });

            List<EventV2> randomInGraceEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Immediate)
                        .ToList();

            randomInGraceEventV2s.ForEach(eventV2 =>
                eventV2.ListenerEventV2s = new List<ListenerEventV2>
                {
                    new ListenerEventV2
                    {
                        Status = ListenerEventStatusV2.Error,
                        RemainingRetryAttempts = 0,
                        DispatchedDate = randomNow
                    }
                });

            IQueryable<EventV2> retrievedEventV2s =
                randomDeadEventV2s
                    .Union(randomStillRetriableEventV2s)
                    .Union(randomInGraceEventV2s)
                        .AsQueryable();

            IQueryable<EventV2> expectedEventV2s =
                randomDeadEventV2s.AsQueryable();

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventV2s);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomNow);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetRetryConfiguration())
                    .Returns(retryConfiguration);

            // when
            IQueryable<EventV2> actualEventV2s =
                await this.eventV2ProcessingService
                    .RetrieveAllDeadEventV2sAsync(randomCancellationToken);

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetRetryConfiguration(),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
