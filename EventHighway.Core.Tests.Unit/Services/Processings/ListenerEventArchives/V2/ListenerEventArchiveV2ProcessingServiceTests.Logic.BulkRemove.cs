// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldBulkRemoveListenerEventArchiveV2sAsync()
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<ListenerEventArchiveV2> randomListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            IQueryable<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                randomListenerEventArchiveV2s.DeepClone();

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.BulkRemoveListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                    cancellationToken))
                        .Returns(ValueTask.CompletedTask);

            // when
            ValueTask bulkRemoveTask =
                this.listenerEventArchiveV2ProcessingService
                    .BulkRemoveListenerEventArchiveV2sAsync(
                        inputListenerEventArchiveV2s,
                        cancellationToken);

            await bulkRemoveTask;

            // then
            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.BulkRemoveListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                    cancellationToken),
                Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
