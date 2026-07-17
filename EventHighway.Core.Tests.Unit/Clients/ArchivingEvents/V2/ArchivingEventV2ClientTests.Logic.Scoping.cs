// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.ArchivingEvents.V2
{
    public partial class ArchivingEventV2ClientTests
    {
        [Fact]
        public async Task ShouldResolveServiceInNewScopePerOperationAsync()
        {
            // given
            int expectedResolutionCount = 2;

            this.archivingEventV2CoordinationServiceMock.Setup(service =>
                service.ArchiveEventV2sAsync(It.IsAny<CancellationToken>()))
                    .Returns(ValueTask.CompletedTask);

            // when
            await this.archivingEventV2Client.ArchiveEventV2sAsync();
            await this.archivingEventV2Client.ArchiveEventV2sAsync();

            // then
            this.archivingEventCoordinationServiceResolutionCount.Should()
                .Be(expectedResolutionCount);

            this.archivingEventV2CoordinationServiceMock.Verify(service =>
                service.ArchiveEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Exactly(2));

            this.archivingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
