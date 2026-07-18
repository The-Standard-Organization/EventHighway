// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ClientTests
    {
        [Fact]
        public async Task ShouldRedactSecretOnRetrieveAllEventParticipantSecretV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var inputEventParticipantSecretV2Query = new EventParticipantSecretV2Query();

            IReadOnlyList<EventParticipantSecretV2> storedEventParticipantSecretV2s =
                CreateRandomEventParticipantSecretV2s().ToList();

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    inputEventParticipantSecretV2Query, randomCancellationToken))
                        .ReturnsAsync(storedEventParticipantSecretV2s);

            // when
            IReadOnlyList<EventParticipantSecretV2> actualEventParticipantSecretV2s =
                await this.eventParticipantSecretV2Client
                    .RetrieveAllEventParticipantSecretV2sAsync(
                        inputEventParticipantSecretV2Query, randomCancellationToken);

            // then
            actualEventParticipantSecretV2s.Should()
                .OnlyContain(eventParticipantSecretV2 => eventParticipantSecretV2.Secret == null);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    inputEventParticipantSecretV2Query, randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRedactSecretOnRetrieveEventParticipantSecretV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventParticipantSecretV2Id = GetRandomId();

            EventParticipantSecretV2 storedEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2();

            EventParticipantSecretV2 expectedEventParticipantSecretV2 =
                storedEventParticipantSecretV2.DeepClone();

            expectedEventParticipantSecretV2.Secret = null;

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id, randomCancellationToken))
                        .ReturnsAsync(storedEventParticipantSecretV2);

            // when
            EventParticipantSecretV2 actualEventParticipantSecretV2 =
                await this.eventParticipantSecretV2Client
                    .RetrieveEventParticipantSecretV2ByIdAsync(
                        someEventParticipantSecretV2Id, randomCancellationToken);

            // then
            actualEventParticipantSecretV2.Secret.Should().BeNull();

            actualEventParticipantSecretV2.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id, randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRedactSecretOnModifyEventParticipantSecretV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantSecretV2 inputEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2();

            EventParticipantSecretV2 storedEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2();

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.ModifyEventParticipantSecretV2Async(
                    inputEventParticipantSecretV2, randomCancellationToken))
                        .ReturnsAsync(storedEventParticipantSecretV2);

            // when
            EventParticipantSecretV2 actualEventParticipantSecretV2 =
                await this.eventParticipantSecretV2Client
                    .ModifyEventParticipantSecretV2Async(
                        inputEventParticipantSecretV2, randomCancellationToken);

            // then
            actualEventParticipantSecretV2.Secret.Should().BeNull();

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.ModifyEventParticipantSecretV2Async(
                    inputEventParticipantSecretV2, randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRedactSecretOnRemoveEventParticipantSecretV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventParticipantSecretV2Id = GetRandomId();

            EventParticipantSecretV2 storedEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2();

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id, randomCancellationToken))
                        .ReturnsAsync(storedEventParticipantSecretV2);

            // when
            EventParticipantSecretV2 actualEventParticipantSecretV2 =
                await this.eventParticipantSecretV2Client
                    .RemoveEventParticipantSecretV2ByIdAsync(
                        someEventParticipantSecretV2Id, randomCancellationToken);

            // then
            actualEventParticipantSecretV2.Secret.Should().BeNull();

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RemoveEventParticipantSecretV2ByIdAsync(
                    someEventParticipantSecretV2Id, randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
