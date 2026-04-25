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
        public async Task ShouldAddListenerEventV1ArchiveAsync()
        {
            // given
            ListenerEventArchiveV1 randomListenerEventV1Archive =
                CreateRandomListenerEventArchiveV1();

            ListenerEventArchiveV1 inputListenerEventV1Archive =
                randomListenerEventV1Archive;

            ListenerEventArchiveV1 storageListenerEventV1Archive =
                inputListenerEventV1Archive;

            ListenerEventArchiveV1 expectedListenerEventV1Archive =
                storageListenerEventV1Archive.DeepClone();

            this.listenerEventArchiveV1ServiceMock.Setup(broker =>
                broker.AddListenerEventArchiveAsync(inputListenerEventV1Archive))
                    .ReturnsAsync(storageListenerEventV1Archive);

            // when
            ListenerEventArchiveV1 actualListenerEventV1Archive =
                await this.listenerEventArchiveV1ProcessingService
                    .AddListenerEventArchiveV1Async(
                        inputListenerEventV1Archive);

            // then
            actualListenerEventV1Archive.Should().BeEquivalentTo(
                expectedListenerEventV1Archive);

            this.listenerEventArchiveV1ServiceMock.Verify(service =>
                service.AddListenerEventArchiveAsync(
                    inputListenerEventV1Archive),
                        Times.Once);

            this.listenerEventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
