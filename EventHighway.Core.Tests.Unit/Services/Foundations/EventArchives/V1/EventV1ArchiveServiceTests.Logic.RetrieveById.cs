// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEventV1ArchiveByIdAsync()
        {
            // given
            Guid randomEventV1ArchiveId = GetRandomId();
            Guid inputEventV1ArchiveId = randomEventV1ArchiveId;

            EventV1Archive randomEventV1Archive =
                CreateRandomEventV1Archive();

            EventV1Archive selectedEventV1Archive =
                randomEventV1Archive;

            EventV1Archive expectedEventV1Archive =
                selectedEventV1Archive.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventV1ArchiveByIdAsync(
                    inputEventV1ArchiveId))
                        .ReturnsAsync(selectedEventV1Archive);

            // when
            EventV1Archive actualEventV1Archive =
                await this.eventV1ArchiveService
                    .RetrieveEventV1ArchiveByIdAsync(
                        inputEventV1ArchiveId);

            // then
            actualEventV1Archive.Should()
                .BeEquivalentTo(expectedEventV1Archive);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV1ArchiveByIdAsync(
                    inputEventV1ArchiveId),
                        Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
