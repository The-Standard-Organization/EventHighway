// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.ArchivingEvents.V2
{
    public partial class ArchivingEventV2ClientTests
    {
        [Fact]
        public async Task ShouldPurgeArchivedEventV2sAsync()
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTimeOffset();

            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            this.archivingEventV2CoordinationServiceMock.Setup(service =>
                service.PurgeArchivedEventV2sAsync(
                    olderThan: someDateTime,
                    cancellationToken: randomCancellationToken))
                    .Returns(ValueTask.CompletedTask);

            // when
            await this.archivingEventV2Client
                .PurgeArchivedEventV2sAsync(
                    olderThan: someDateTime,
                    cancellationToken: randomCancellationToken);

            // then
            this.archivingEventV2CoordinationServiceMock.Verify(service =>
                service.PurgeArchivedEventV2sAsync(
                    olderThan: someDateTime,
                    cancellationToken: randomCancellationToken),
                    Times.Once);

            this.archivingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
