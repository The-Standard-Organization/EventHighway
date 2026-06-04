// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V1
{
    public partial class ListenerEventArchiveV1ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldAddListenerEventArchiveV1Async()
        {
            // given
            ListenerEventArchiveV1 randomListenerEventArchiveV1 =
                CreateRandomListenerEventArchiveV1();

            ListenerEventArchiveV1 inputListenerEventArchiveV1 =
                randomListenerEventArchiveV1;

            ListenerEventArchiveV1 storageListenerEventArchiveV1 =
                inputListenerEventArchiveV1;

            ListenerEventArchiveV1 expectedListenerEventArchiveV1 =
                storageListenerEventArchiveV1.DeepClone();

            this.listenerEventArchiveV1ServiceMock.Setup(broker =>
                broker.AddListenerEventArchiveV1Async(inputListenerEventArchiveV1))
                    .ReturnsAsync(storageListenerEventArchiveV1);

            // when
            ListenerEventArchiveV1 actualListenerEventArchiveV1 =
                await this.listenerEventArchiveV1ProcessingService
                    .AddListenerEventArchiveV1Async(
                        inputListenerEventArchiveV1);

            // then
            actualListenerEventArchiveV1.Should().BeEquivalentTo(
                expectedListenerEventArchiveV1);

            this.listenerEventArchiveV1ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV1Async(
                    inputListenerEventArchiveV1),
                        Times.Once);

            this.listenerEventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
