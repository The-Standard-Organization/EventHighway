// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.ArchivingEvents.V2
{
    public partial class ArchivingEventV2ClientTests
    {
        [Fact]
        public async Task ShouldPurgeEventArchiveV2sFromConfigurationAsync()
        {
            // given
            CancellationToken inputCancellationToken = TestContext.Current.CancellationToken;

            this.archivingEventV2CoordinationServiceMock.Setup(service =>
                service.PurgeEventArchiveV2sAsync(inputCancellationToken))
                    .Returns(ValueTask.CompletedTask);

            // when
            await this.archivingEventV2Client
                .PurgeEventArchiveV2sAsync(inputCancellationToken);

            // then
            this.archivingEventV2CoordinationServiceMock.Verify(service =>
                service.PurgeEventArchiveV2sAsync(inputCancellationToken),
                    Times.Once);

            this.archivingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
