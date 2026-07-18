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
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEventParticipantSecretV2sByQueryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid targetEventParticipantV2Id = GetRandomId();
            DateTimeOffset baseDateTimeOffset = GetRandomDateTimeOffset();

            List<EventParticipantSecretV2> matchingEventParticipantSecretV2s =
                Enumerable.Range(start: 0, count: 4).Select(index =>
                {
                    EventParticipantSecretV2 matchingEventParticipantSecretV2 =
                        CreateRandomEventParticipantSecretV2();

                    matchingEventParticipantSecretV2.EventParticipantV2Id = targetEventParticipantV2Id;
                    matchingEventParticipantSecretV2.IsActive = true;

                    matchingEventParticipantSecretV2.CreatedDate =
                        baseDateTimeOffset.AddMinutes(-index);

                    return matchingEventParticipantSecretV2;
                }).ToList();

            EventParticipantSecretV2 differentParticipantEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2();

            EventParticipantSecretV2 inactiveEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2();

            inactiveEventParticipantSecretV2.EventParticipantV2Id = targetEventParticipantV2Id;
            inactiveEventParticipantSecretV2.IsActive = false;

            IQueryable<EventParticipantSecretV2> allEventParticipantSecretV2s =
                matchingEventParticipantSecretV2s
                    .Append(differentParticipantEventParticipantSecretV2)
                    .Append(inactiveEventParticipantSecretV2)
                    .AsQueryable();

            var inputEventParticipantSecretV2Query = new EventParticipantSecretV2Query
            {
                EventParticipantV2Id = targetEventParticipantV2Id,
                IsActive = true,
                Skip = 1,
                Take = 2
            };

            List<EventParticipantSecretV2> expectedEventParticipantSecretV2s =
                matchingEventParticipantSecretV2s
                    .OrderByDescending(secret => secret.CreatedDate)
                    .ThenBy(secret => secret.Id)
                    .Skip(1)
                    .Take(2)
                    .ToList();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventParticipantSecretV2sAsync(randomCancellationToken))
                    .ReturnsAsync(allEventParticipantSecretV2s);

            // when
            IReadOnlyList<EventParticipantSecretV2> actualEventParticipantSecretV2s =
                await this.eventParticipantSecretV2Service
                    .RetrieveEventParticipantSecretV2sByQueryAsync(
                        inputEventParticipantSecretV2Query, randomCancellationToken);

            // then
            actualEventParticipantSecretV2s.Should().BeEquivalentTo(
                expectedEventParticipantSecretV2s, options => options.WithStrictOrdering());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventParticipantSecretV2sAsync(randomCancellationToken),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
