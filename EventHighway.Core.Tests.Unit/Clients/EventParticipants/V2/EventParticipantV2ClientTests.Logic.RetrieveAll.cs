// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventParticipants.V2
{
    public partial class EventParticipantV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventParticipantV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IReadOnlyList<EventParticipantV2> returnedEventParticipantV2s =
                CreateRandomEventParticipantV2s().ToList();

            IReadOnlyList<EventParticipantV2> expectedEventParticipantV2s =
                returnedEventParticipantV2s.DeepClone();

            var inputEventParticipantV2Query = new EventParticipantV2Query();

            this.eventParticipantV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2sByQueryAsync(
                    inputEventParticipantV2Query, randomCancellationToken))
                        .ReturnsAsync(returnedEventParticipantV2s);

            // when
            IReadOnlyList<EventParticipantV2> actualEventParticipantV2s =
                await this.eventParticipantV2Client.RetrieveAllEventParticipantV2sAsync(
                    inputEventParticipantV2Query, randomCancellationToken);

            // then
            actualEventParticipantV2s.Should()
                .BeEquivalentTo(expectedEventParticipantV2s);

            this.eventParticipantV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2sByQueryAsync(
                    inputEventParticipantV2Query, randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
