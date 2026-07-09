// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
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

            IEnumerable<EventParticipantV2> randomEventParticipantV2s =
                CreateRandomEventParticipantV2s();

            IQueryable<EventParticipantV2> returnedEventParticipantV2s =
                randomEventParticipantV2s.AsQueryable();

            IEnumerable<EventParticipantV2> expectedEventParticipantV2s =
                returnedEventParticipantV2s.DeepClone();

            this.eventParticipantV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken))
                    .ReturnsAsync(returnedEventParticipantV2s);

            // when
            IEnumerable<EventParticipantV2> actualEventParticipantV2s =
                await this.eventParticipantV2Client
                    .RetrieveAllEventParticipantV2sAsync(randomCancellationToken);

            // then
            actualEventParticipantV2s.Should()
                .BeEquivalentTo(expectedEventParticipantV2s);

            this.eventParticipantV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventParticipantV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
