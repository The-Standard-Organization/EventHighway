// ---------------------------------------------------------------
// Copyright (c) Aspen Publishing. All rights reserved.
// ---------------------------------------------------------------

using System;
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
        public async Task ShouldRetrieveEventV1ArchiveByIdAsync()
        {
            // given
            Guid randomEventV1ArchiveId = GetRandomId();
            Guid inputEventV1ArchiveId = randomEventV1ArchiveId;
            EventV1Archive randomEventV1Archive = CreateRandomEventV1Archive();
            EventV1Archive retrievedEventV1Archive = randomEventV1Archive;
            EventV1Archive expectedEventV1Archive = retrievedEventV1Archive.DeepClone();

            this.eventV1ArchiveServiceMock.Setup(service =>
                service.RetrieveEventV1ArchiveByIdAsync(inputEventV1ArchiveId))
                    .ReturnsAsync(retrievedEventV1Archive);

            // when 
            EventV1Archive actualEventV1Archive =
                await this.eventV1ArchiveProcessingService
                    .RetrieveEventV1ArchiveByIdAsync(inputEventV1ArchiveId);

            // then
            actualEventV1Archive.Should().BeEquivalentTo(expectedEventV1Archive);

            this.eventV1ArchiveServiceMock.Verify(service =>
                service.RetrieveEventV1ArchiveByIdAsync(inputEventV1ArchiveId),
                    Times.Once);

            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
