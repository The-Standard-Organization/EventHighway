// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventArchives.V2
{
    public partial class EventArchiveV2ClientTests
    {
        [Fact]
        public async Task ShouldCopyContentHashWhenArchivingDeadEventV2Async()
        {
            // given
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            Guid inputEventAddressV2Id =
                randomEventAddressV2.Id;

            EventV2 inputEventV2 =
                CreateDeadEventV2Filler(inputEventAddressV2Id).Create();

            EventV2 submittedEventV2 =
                await this.clientBroker.SubmitEventV2Async(inputEventV2);

            Guid inputEventArchiveV2Id = submittedEventV2.Id;

            await this.clientBroker.ArchiveDeadEventV2sAsync();

            // when
            EventArchiveV2 actualEventArchiveV2 =
                await this.clientBroker
                    .RetrieveEventArchiveV2ByIdAsync(
                        inputEventArchiveV2Id);

            // then
            actualEventArchiveV2.Should().NotBeNull();
            actualEventArchiveV2.ContentHash.Should().NotBeNullOrWhiteSpace();
            actualEventArchiveV2.ContentHash.Should().Be(submittedEventV2.ContentHash);

            await this.clientBroker
                .PurgeEventArchiveV2sAsync(DateTimeOffset.UtcNow.AddSeconds(1));

            await this.clientBroker
                .RemoveEventAddressV2ByIdAsync(inputEventAddressV2Id);
        }
    }
}
