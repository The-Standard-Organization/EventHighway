// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using Moq;
using Xunit;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventParticipants.V2
{
    public partial class EventParticipantV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldValidateEventParticipantsWhenParticipantIdAndSecretAreNotProvidedAsync()
        {
            // given
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = null;
            inputEventV2.EventParticipantV2Secret = null;

            // when
            await this.eventParticipantV2OrchestrationService
                .ValidateEventParticipantsAsync(
                    inputEventV2,
                    TestContext.Current.CancellationToken);

            // then
            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldValidateEventParticipantsWhenParticipantIsFoundAndActiveAndNoSecretAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventParticipantV2 randomEventParticipantV2 = CreateRandomEventParticipantV2();
            EventParticipantV2 activeEventParticipantV2 = randomEventParticipantV2;
            activeEventParticipantV2.IsActive = true;
            activeEventParticipantV2.ActiveFrom = null;
            activeEventParticipantV2.ActiveTo = null;

            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = activeEventParticipantV2.Id;
            inputEventV2.EventParticipantV2Secret = null;

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventV2.EventParticipantV2Id.Value,
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(activeEventParticipantV2);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            await this.eventParticipantV2OrchestrationService
                .ValidateEventParticipantsAsync(
                    inputEventV2,
                    TestContext.Current.CancellationToken);

            // then
            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventV2.EventParticipantV2Id.Value,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldValidateEventParticipantsWhenParticipantAndSecretAreFoundAndActiveAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventParticipantV2 randomEventParticipantV2 = CreateRandomEventParticipantV2();
            EventParticipantV2 activeEventParticipantV2 = randomEventParticipantV2;
            activeEventParticipantV2.IsActive = true;
            activeEventParticipantV2.ActiveFrom = null;
            activeEventParticipantV2.ActiveTo = null;

            EventParticipantSecretV2 randomEventParticipantSecretV2 = CreateRandomEventParticipantSecretV2();
            EventParticipantSecretV2 activeEventParticipantSecretV2 = randomEventParticipantSecretV2;
            activeEventParticipantSecretV2.IsActive = true;
            activeEventParticipantSecretV2.ActiveFrom = null;
            activeEventParticipantSecretV2.ActiveTo = null;
            activeEventParticipantSecretV2.EventParticipantV2Id = activeEventParticipantV2.Id;

            IQueryable<EventParticipantSecretV2> eventParticipantSecretV2s =
                new List<EventParticipantSecretV2> { activeEventParticipantSecretV2 }.AsQueryable();

            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = activeEventParticipantV2.Id;
            string inputSecretValue = GetRandomString();
            inputEventV2.EventParticipantV2Secret = inputSecretValue;

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventV2.EventParticipantV2Id.Value,
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(activeEventParticipantV2);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.hashBrokerMock.Setup(broker =>
                broker.GenerateSha256Hash(inputSecretValue))
                    .Returns(activeEventParticipantSecretV2.Secret);

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RetrieveAllEventParticipantSecretV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(eventParticipantSecretV2s);

            // when
            await this.eventParticipantV2OrchestrationService
                .ValidateEventParticipantsAsync(
                    inputEventV2,
                    TestContext.Current.CancellationToken);

            // then
            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventV2.EventParticipantV2Id.Value,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.hashBrokerMock.Verify(broker =>
                broker.GenerateSha256Hash(inputSecretValue),
                    Times.Once);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantSecretV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldValidateEventParticipantsWhenParticipantAndSecretAreWithinActiveWindowAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            EventParticipantV2 randomEventParticipantV2 = CreateRandomEventParticipantV2();
            EventParticipantV2 activeEventParticipantV2 = randomEventParticipantV2;
            activeEventParticipantV2.IsActive = true;
            activeEventParticipantV2.ActiveFrom = randomDateTimeOffset.AddMinutes(-1);
            activeEventParticipantV2.ActiveTo = randomDateTimeOffset.AddMinutes(1);

            EventParticipantSecretV2 randomEventParticipantSecretV2 = CreateRandomEventParticipantSecretV2();
            EventParticipantSecretV2 activeEventParticipantSecretV2 = randomEventParticipantSecretV2;
            activeEventParticipantSecretV2.IsActive = true;
            activeEventParticipantSecretV2.ActiveFrom = randomDateTimeOffset.AddMinutes(-1);
            activeEventParticipantSecretV2.ActiveTo = randomDateTimeOffset.AddMinutes(1);
            activeEventParticipantSecretV2.EventParticipantV2Id = activeEventParticipantV2.Id;

            IQueryable<EventParticipantSecretV2> eventParticipantSecretV2s =
                new List<EventParticipantSecretV2> { activeEventParticipantSecretV2 }.AsQueryable();

            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = activeEventParticipantV2.Id;
            string inputSecretValue = GetRandomString();
            inputEventV2.EventParticipantV2Secret = inputSecretValue;

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventV2.EventParticipantV2Id.Value,
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(activeEventParticipantV2);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.hashBrokerMock.Setup(broker =>
                broker.GenerateSha256Hash(inputSecretValue))
                    .Returns(activeEventParticipantSecretV2.Secret);

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RetrieveAllEventParticipantSecretV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(eventParticipantSecretV2s);

            // when
            await this.eventParticipantV2OrchestrationService
                .ValidateEventParticipantsAsync(
                    inputEventV2,
                    TestContext.Current.CancellationToken);

            // then
            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    inputEventV2.EventParticipantV2Id.Value,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.hashBrokerMock.Verify(broker =>
                broker.GenerateSha256Hash(inputSecretValue),
                    Times.Once);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantSecretV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
