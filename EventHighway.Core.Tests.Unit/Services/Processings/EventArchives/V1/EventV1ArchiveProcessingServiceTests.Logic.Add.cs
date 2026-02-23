// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V1
{
    public partial class EventV1ArchiveProcessingServiceTests
    {
        [Fact]
        public async Task ShouldAddEventV1ArchiveAsync()
        {
            // given
            EventV1Archive randomEventV1Archive =
                CreateRandomEventV1Archive();

            EventV1Archive inputEventV1Archive =
                randomEventV1Archive;

            EventV1Archive addedEventV1Archive =
                inputEventV1Archive;

            EventV1Archive expectedEventV1Archive =
                addedEventV1Archive.DeepClone();

            this.eventV1ArchiveServiceMock.Setup(service =>
                service.AddEventV1ArchiveAsync(
                    inputEventV1Archive))
                        .ReturnsAsync(addedEventV1Archive);

            // when
            EventV1Archive actualEventV1Archive =
                await this.eventV1ArchiveProcessingService
                    .AddEventV1ArchiveAsync(
                        inputEventV1Archive);

            // then
            actualEventV1Archive.Should().BeEquivalentTo(
                expectedEventV1Archive);

            this.eventV1ArchiveServiceMock.Verify(service =>
                service.AddEventV1ArchiveAsync(
                    inputEventV1Archive),
                        Times.Once);

            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
