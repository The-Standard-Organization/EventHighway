// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ListenerEvents.V2
{
    public partial class ListenerEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveListenerEventV2sByQueryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid targetEventListenerV2Id = GetRandomId();
            DateTimeOffset baseDateTimeOffset = GetRandomDateTimeOffset();

            List<ListenerEventV2> matchingListenerEventV2s =
                Enumerable.Range(start: 0, count: 4).Select(index =>
                {
                    ListenerEventV2 matchingListenerEventV2 = CreateRandomListenerEventV2();
                    matchingListenerEventV2.EventListenerV2Id = targetEventListenerV2Id;
                    matchingListenerEventV2.Status = ListenerEventStatusV2.Error;
                    matchingListenerEventV2.CreatedDate = baseDateTimeOffset.AddMinutes(-index);

                    return matchingListenerEventV2;
                }).ToList();

            ListenerEventV2 differentListenerEventV2 = CreateRandomListenerEventV2();

            ListenerEventV2 successStatusListenerEventV2 = CreateRandomListenerEventV2();
            successStatusListenerEventV2.EventListenerV2Id = targetEventListenerV2Id;
            successStatusListenerEventV2.Status = ListenerEventStatusV2.Success;

            IQueryable<ListenerEventV2> allListenerEventV2s = matchingListenerEventV2s
                .Append(differentListenerEventV2)
                .Append(successStatusListenerEventV2)
                .AsQueryable();

            var inputListenerEventV2Query = new ListenerEventV2Query
            {
                EventListenerV2Id = targetEventListenerV2Id,
                Status = ListenerEventStatusV2.Error,
                Skip = 1,
                Take = 2
            };

            List<ListenerEventV2> expectedListenerEventV2s = matchingListenerEventV2s
                .OrderByDescending(listenerEventV2 => listenerEventV2.CreatedDate)
                .ThenBy(listenerEventV2 => listenerEventV2.Id)
                .Skip(1)
                .Take(2)
                .ToList();

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(allListenerEventV2s);

            // when
            IReadOnlyList<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2OrchestrationService.RetrieveListenerEventV2sByQueryAsync(
                    inputListenerEventV2Query, randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s, options =>
                options.WithStrictOrdering());

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveListenerEventV2sWithEventListenerV2ByQueryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid targetEventListenerV2Id = GetRandomId();
            DateTimeOffset baseDateTimeOffset = GetRandomDateTimeOffset();

            List<ListenerEventV2> matchingListenerEventV2s =
                Enumerable.Range(start: 0, count: 4).Select(index =>
                {
                    ListenerEventV2 matchingListenerEventV2 = CreateRandomListenerEventV2();
                    matchingListenerEventV2.EventListenerV2Id = targetEventListenerV2Id;
                    matchingListenerEventV2.Status = ListenerEventStatusV2.Error;
                    matchingListenerEventV2.CreatedDate = baseDateTimeOffset.AddMinutes(-index);

                    return matchingListenerEventV2;
                }).ToList();

            ListenerEventV2 differentListenerEventV2 = CreateRandomListenerEventV2();

            ListenerEventV2 successStatusListenerEventV2 = CreateRandomListenerEventV2();
            successStatusListenerEventV2.EventListenerV2Id = targetEventListenerV2Id;
            successStatusListenerEventV2.Status = ListenerEventStatusV2.Success;

            IQueryable<ListenerEventV2> allListenerEventV2s = matchingListenerEventV2s
                .Append(differentListenerEventV2)
                .Append(successStatusListenerEventV2)
                .AsQueryable();

            var inputListenerEventV2Query = new ListenerEventV2Query
            {
                EventListenerV2Id = targetEventListenerV2Id,
                Status = ListenerEventStatusV2.Error,
                Skip = 1,
                Take = 2
            };

            List<ListenerEventV2> expectedListenerEventV2s = matchingListenerEventV2s
                .OrderByDescending(listenerEventV2 => listenerEventV2.CreatedDate)
                .ThenBy(listenerEventV2 => listenerEventV2.Id)
                .Skip(1)
                .Take(2)
                .ToList();

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sWithEventListenerV2Async(randomCancellationToken))
                    .ReturnsAsync(allListenerEventV2s);

            // when
            IReadOnlyList<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2OrchestrationService
                    .RetrieveListenerEventV2sWithEventListenerV2ByQueryAsync(
                        inputListenerEventV2Query, randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s, options =>
                options.WithStrictOrdering());

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sWithEventListenerV2Async(randomCancellationToken),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
