// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.ArchivingEvents.V2
{
    public partial class ArchivingEventV2ClientTests
    {
        [Fact]
        public async Task ShouldPurgeEventArchiveV2sAsync()
        {
            // given
            DateTimeOffset futureDateTimeOffset =
                DateTimeOffset.UtcNow.AddDays(1);

            // when
            Func<Task> purgeEventArchiveV2sTask = async () =>
                await this.clientBroker.PurgeEventArchiveV2sAsync(
                    futureDateTimeOffset);

            // then
            await purgeEventArchiveV2sTask.Should()
                .NotThrowAsync();
        }

        [Fact]
        public async Task ShouldPurgeEventArchiveV2sFromConfigurationAsync()
        {
            // when
            Func<Task> purgeEventArchiveV2sTask = async () =>
                await this.clientBroker.PurgeEventArchiveV2sAsync();

            // then
            await purgeEventArchiveV2sTask.Should()
                .NotThrowAsync();
        }
    }
}
