// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Coordinations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.Events.V2
{
    public partial class EventV2ClientTests
    {
        [Fact]
        public async Task ShouldHandleConcurrentMixedV2ClientOperationsAsync()
        {
            // given
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            Guid inputEventAddressV2Id = randomEventAddressV2.Id;

            DateTimeOffset scheduledDate =
                DateTimeOffset.UtcNow.AddSeconds(GetRandomNumber());

            EventV2 inputEventV2 =
                CreateRandomEventV2(inputEventAddressV2Id, scheduledDate);

            EventV2 submittedEventV2 =
                await this.clientBroker.SubmitEventV2Async(inputEventV2);

            TrafficPeriodV2 period = TrafficPeriodV2.Day;
            DateTimeOffset windowStart = DateTimeOffset.UtcNow.AddDays(-1);
            int concurrentOperationsCount = 24;

            // when — hammer six different V2 clients concurrently. Before the scope-per-operation
            // fix these would share one EF DbContext and collide with a "second operation was
            // started on this context instance" failure; now each operation isolates its own scope.
            IEnumerable<Task> concurrentOperations =
                Enumerable.Range(start: 0, count: concurrentOperationsCount)
                    .Select(iteration => Task.Run(async () =>
                    {
                        switch (iteration % 6)
                        {
                            case 0:
                                await this.clientBroker.RetrieveEventV2ByIdAsync(
                                    submittedEventV2.Id);

                                break;

                            case 1:
                                await this.clientBroker.RetrieveAllEventV2sAsync(
                                    new EventV2Query { EventAddressV2Id = inputEventAddressV2Id });

                                break;

                            case 2:
                                await this.clientBroker.RetrieveAllEventArchiveV2sAsync(
                                    new EventArchiveV2Query { Take = 1000 });

                                break;

                            case 3:
                                await this.clientBroker.RetrieveAllListenerEventV2sAsync(
                                    new ListenerEventV2Query { Take = 1000 });

                                break;

                            case 4:
                                await this.clientBroker.RetrieveEventListenerV2sByEventAddressIdAsync(
                                    inputEventAddressV2Id);

                                break;

                            default:
                                await this.clientBroker.RetrieveHealthRagStatusV2Async(
                                    period, windowStart);

                                break;
                        }
                    }));

            Func<Task> concurrentOperationsTask =
                () => Task.WhenAll(concurrentOperations);

            // then
            await concurrentOperationsTask.Should().NotThrowAsync();

            await this.clientBroker.RemoveEventV2ByIdAsync(submittedEventV2.Id);

            await this.clientBroker.RemoveEventAddressV2ByIdAsync(
                inputEventAddressV2Id);
        }
    }
}
