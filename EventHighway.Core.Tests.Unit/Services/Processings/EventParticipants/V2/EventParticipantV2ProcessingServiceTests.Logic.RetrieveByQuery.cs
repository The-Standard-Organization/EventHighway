// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventParticipants.V2
{
    public partial class EventParticipantV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEventParticipantV2sByQueryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string targetName = GetRandomString();
            DateTimeOffset baseDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset createdFrom = baseDateTimeOffset;
            DateTimeOffset createdTo = baseDateTimeOffset.AddHours(1);

            List<EventParticipantV2> matchingEventParticipantV2s =
                Enumerable.Range(start: 0, count: 4).Select(index =>
                {
                    EventParticipantV2 matchingEventParticipantV2 =
                        CreateRandomEventParticipantV2();

                    matchingEventParticipantV2.Name = targetName;
                    matchingEventParticipantV2.IsActive = true;
                    matchingEventParticipantV2.IsSecretRequired = true;
                    matchingEventParticipantV2.CreatedDate = createdFrom.AddMinutes(index);

                    return matchingEventParticipantV2;
                }).ToList();

            EventParticipantV2 differentNameEventParticipantV2 =
                CreateRandomEventParticipantV2();

            differentNameEventParticipantV2.IsActive = true;
            differentNameEventParticipantV2.IsSecretRequired = true;
            differentNameEventParticipantV2.CreatedDate = createdFrom.AddMinutes(5);

            EventParticipantV2 inactiveEventParticipantV2 =
                CreateRandomEventParticipantV2();

            inactiveEventParticipantV2.Name = targetName;
            inactiveEventParticipantV2.IsActive = false;
            inactiveEventParticipantV2.IsSecretRequired = true;
            inactiveEventParticipantV2.CreatedDate = createdFrom.AddMinutes(5);

            EventParticipantV2 secretOptionalEventParticipantV2 =
                CreateRandomEventParticipantV2();

            secretOptionalEventParticipantV2.Name = targetName;
            secretOptionalEventParticipantV2.IsActive = true;
            secretOptionalEventParticipantV2.IsSecretRequired = false;
            secretOptionalEventParticipantV2.CreatedDate = createdFrom.AddMinutes(5);

            EventParticipantV2 createdBeforeWindowEventParticipantV2 =
                CreateRandomEventParticipantV2();

            createdBeforeWindowEventParticipantV2.Name = targetName;
            createdBeforeWindowEventParticipantV2.IsActive = true;
            createdBeforeWindowEventParticipantV2.IsSecretRequired = true;
            createdBeforeWindowEventParticipantV2.CreatedDate = createdFrom.AddMinutes(-5);

            EventParticipantV2 createdAfterWindowEventParticipantV2 =
                CreateRandomEventParticipantV2();

            createdAfterWindowEventParticipantV2.Name = targetName;
            createdAfterWindowEventParticipantV2.IsActive = true;
            createdAfterWindowEventParticipantV2.IsSecretRequired = true;
            createdAfterWindowEventParticipantV2.CreatedDate = createdTo.AddMinutes(5);

            IQueryable<EventParticipantV2> allEventParticipantV2s = matchingEventParticipantV2s
                .Append(differentNameEventParticipantV2)
                .Append(inactiveEventParticipantV2)
                .Append(secretOptionalEventParticipantV2)
                .Append(createdBeforeWindowEventParticipantV2)
                .Append(createdAfterWindowEventParticipantV2)
                .AsQueryable();

            var inputEventParticipantV2Query = new EventParticipantV2Query
            {
                Name = targetName,
                IsActive = true,
                IsSecretRequired = true,
                CreatedFrom = createdFrom,
                CreatedTo = createdTo,
                Skip = 1,
                Take = 2
            };

            List<EventParticipantV2> expectedEventParticipantV2s = matchingEventParticipantV2s
                .OrderByDescending(eventParticipantV2 => eventParticipantV2.CreatedDate)
                .ThenBy(eventParticipantV2 => eventParticipantV2.Id)
                .Skip(1)
                .Take(2)
                .ToList();

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken))
                    .ReturnsAsync(allEventParticipantV2s);

            // when
            IQueryable<EventParticipantV2> actualEventParticipantV2sQuery =
                await this.eventParticipantV2ProcessingService
                    .RetrieveEventParticipantV2sByQueryAsync(
                        inputEventParticipantV2Query, randomCancellationToken);

            List<EventParticipantV2> actualEventParticipantV2s =
                actualEventParticipantV2sQuery.ToList();

            // then
            actualEventParticipantV2s.Should().BeEquivalentTo(
                expectedEventParticipantV2s, options =>
                    options.WithStrictOrdering());

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
