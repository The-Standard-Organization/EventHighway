// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.Events.V2
{
    public partial class EventV2ClientTests
    {
        [Fact]
        public async Task ShouldHandleConcurrentEventV2OperationsAsync()
        {
            // given
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            Guid inputEventAddressV2Id = randomEventAddressV2.Id;

            DateTimeOffset scheduledDate =
                DateTimeOffset.UtcNow.AddSeconds(
                    GetRandomNumber());

            EventV2 inputEventV2 =
                CreateRandomEventV2(inputEventAddressV2Id, scheduledDate);

            EventV2 submittedEventV2 =
                await this.clientBroker.SubmitEventV2Async(inputEventV2);

            int concurrentOperationsCount = 20;

            // when
            IEnumerable<Task<EventV2>> concurrentOperations =
                Enumerable.Range(start: 0, count: concurrentOperationsCount)
                    .Select(iteration => Task.Run(async () =>
                        iteration % 2 == 0
                            ? await this.clientBroker.RetrieveEventV2ByIdAsync(
                                submittedEventV2.Id)

                            : (await this.clientBroker.RetrieveAllEventV2sAsync(
                                new EventV2Query { EventAddressV2Id = inputEventAddressV2Id }))
                                .ToList()
                                .First(eventV2 => eventV2.Id == submittedEventV2.Id)));

            EventV2[] actualEventV2s =
                await Task.WhenAll(concurrentOperations);

            // then
            actualEventV2s.Should().HaveCount(concurrentOperationsCount);

            actualEventV2s.Should().AllSatisfy(actualEventV2 =>
                actualEventV2.Id.Should().Be(submittedEventV2.Id));

            await this.clientBroker.RemoveEventV2ByIdAsync(submittedEventV2.Id);

            await this.clientBroker.RemoveEventAddressV2ByIdAsync(
                inputEventAddressV2Id);
        }
    }
}
