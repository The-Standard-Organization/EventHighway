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
    public partial class ListenerEventV1ArchiveProcessingServiceTests
    {
        [Fact]
        public async Task ShouldAddListenerEventV1ArchiveAsync()
        {
            // given
            ListenerEventV1Archive randomListenerEventV1Archive =
                CreateRandomListenerEventV1Archive();

            ListenerEventV1Archive inputListenerEventV1Archive =
                randomListenerEventV1Archive;

            ListenerEventV1Archive storageListenerEventV1Archive =
                inputListenerEventV1Archive;

            ListenerEventV1Archive expectedListenerEventV1Archive =
                storageListenerEventV1Archive.DeepClone();

            this.listenerEventV1ArchiveServiceMock.Setup(broker =>
                broker.AddListenerEventV1ArchiveAsync(inputListenerEventV1Archive))
                    .ReturnsAsync(storageListenerEventV1Archive);

            // when
            ListenerEventV1Archive actualListenerEventV1Archive =
                await this.listenerEventV1ArchiveProcessingService
                    .AddListenerEventV1ArchiveAsync(
                        inputListenerEventV1Archive);

            // then
            actualListenerEventV1Archive.Should().BeEquivalentTo(
                expectedListenerEventV1Archive);

            this.listenerEventV1ArchiveServiceMock.Verify(broker =>
                broker.AddListenerEventV1ArchiveAsync(
                    inputListenerEventV1Archive),
                        Times.Once);

            this.listenerEventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
