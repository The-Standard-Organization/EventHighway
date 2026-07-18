// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ClientTests
    {
        [Fact]
        public async Task ShouldResolveServiceInNewScopePerOperationAsync()
        {
            // given
            var inputEventParticipantSecretV2Query = new EventParticipantSecretV2Query();
            int expectedResolutionCount = 2;

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    inputEventParticipantSecretV2Query, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<EventParticipantSecretV2>());

            // when
            await this.eventParticipantSecretV2Client.RetrieveAllEventParticipantSecretV2sAsync(
                inputEventParticipantSecretV2Query);

            await this.eventParticipantSecretV2Client.RetrieveAllEventParticipantSecretV2sAsync(
                inputEventParticipantSecretV2Query);

            // then
            this.eventParticipantSecretServiceResolutionCount.Should()
                .Be(expectedResolutionCount);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    inputEventParticipantSecretV2Query, It.IsAny<CancellationToken>()),
                        Times.Exactly(2));

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
