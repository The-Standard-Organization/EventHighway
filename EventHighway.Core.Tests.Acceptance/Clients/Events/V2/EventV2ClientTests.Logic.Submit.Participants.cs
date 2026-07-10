// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.Events.V2
{
    public partial class EventV2ClientTests
    {
        [Fact]
        public async Task ShouldSubmitEventV2WhenNoParticipantIdIsProvidedAsync()
        {
            // given
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            Guid inputEventAddressV2Id = randomEventAddressV2.Id;

            DateTimeOffset scheduledDate =
                TruncateToMicroseconds(DateTimeOffset.UtcNow)
                    .AddSeconds(GetRandomNumber());

            EventV2 inputEventV2 =
                CreateRandomEventV2(inputEventAddressV2Id, scheduledDate);

            inputEventV2.EventParticipantV2Id = null;
            inputEventV2.EventParticipantV2Secret = null;
            EventV2 expectedEventV2 = inputEventV2;

            // when
            EventV2 actualEventV2 =
                await this.clientBroker.SubmitEventV2Async(inputEventV2);

            // then
            actualEventV2.Should().BeEquivalentTo(expectedEventV2);

            await this.clientBroker.RemoveEventV2ByIdAsync(actualEventV2.Id);

            await this.clientBroker.RemoveEventAddressV2ByIdAsync(
                inputEventAddressV2Id);
        }

        [Fact]
        public async Task ShouldSubmitEventV2WhenParticipantIdIsProvidedAndParticipantExistsAsync()
        {
            // given
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            Guid inputEventAddressV2Id = randomEventAddressV2.Id;

            EventParticipantV2 addedEventParticipantV2 =
                await CreateRandomEventParticipantV2Async();

            DateTimeOffset scheduledDate =
                TruncateToMicroseconds(DateTimeOffset.UtcNow)
                    .AddSeconds(GetRandomNumber());

            EventV2 inputEventV2 =
                CreateRandomEventV2(inputEventAddressV2Id, scheduledDate);

            inputEventV2.EventParticipantV2Id = addedEventParticipantV2.Id;
            inputEventV2.EventParticipantV2Secret = null;
            EventV2 expectedEventV2 = inputEventV2;

            // when
            EventV2 actualEventV2 =
                await this.clientBroker.SubmitEventV2Async(inputEventV2);

            // then
            actualEventV2.Should().BeEquivalentTo(expectedEventV2);

            await this.clientBroker.RemoveEventV2ByIdAsync(actualEventV2.Id);

            await this.clientBroker.RemoveEventParticipantV2ByIdAsync(
                addedEventParticipantV2.Id);

            await this.clientBroker.RemoveEventAddressV2ByIdAsync(
                inputEventAddressV2Id);
        }

        [Fact]
        public async Task ShouldSubmitEventV2WhenParticipantIdAndSecretAreProvidedAndBothExistAsync()
        {
            // given
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            Guid inputEventAddressV2Id = randomEventAddressV2.Id;

            EventParticipantV2 addedEventParticipantV2 =
                await CreateRandomEventParticipantV2Async();

            EventParticipantSecretV2 addedEventParticipantSecretV2 =
                await CreateRandomEventParticipantSecretV2Async(
                    addedEventParticipantV2.Id);

            DateTimeOffset scheduledDate =
                TruncateToMicroseconds(DateTimeOffset.UtcNow)
                    .AddSeconds(GetRandomNumber());

            EventV2 inputEventV2 =
                CreateRandomEventV2(inputEventAddressV2Id, scheduledDate);

            inputEventV2.EventParticipantV2Id = addedEventParticipantV2.Id;
            inputEventV2.EventParticipantV2Secret = addedEventParticipantSecretV2.Secret;
            EventV2 expectedEventV2 = inputEventV2;

            // when
            EventV2 actualEventV2 =
                await this.clientBroker.SubmitEventV2Async(inputEventV2);

            // then
            actualEventV2.Should().BeEquivalentTo(expectedEventV2);

            await this.clientBroker.RemoveEventV2ByIdAsync(actualEventV2.Id);

            await this.clientBroker.RemoveEventParticipantSecretV2ByIdAsync(
                addedEventParticipantSecretV2.Id);

            await this.clientBroker.RemoveEventParticipantV2ByIdAsync(
                addedEventParticipantV2.Id);

            await this.clientBroker.RemoveEventAddressV2ByIdAsync(
                inputEventAddressV2Id);
        }
    }
}
