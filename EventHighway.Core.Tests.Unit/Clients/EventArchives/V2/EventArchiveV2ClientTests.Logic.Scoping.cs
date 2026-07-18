// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventArchives.V2
{
    public partial class EventArchiveV2ClientTests
    {
        [Fact]
        public async Task ShouldResolveServiceInNewScopePerOperationAsync()
        {
            // given
            EventArchiveV2 randomEventArchiveV2 = CreateRandomEventArchiveV2();
            Guid inputEventArchiveV2Id = randomEventArchiveV2.Id;
            int expectedResolutionCount = 2;

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2ByIdAsync(
                    inputEventArchiveV2Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(randomEventArchiveV2);

            // when
            await this.eventArchiveV2Client.RetrieveEventArchiveV2ByIdAsync(
                inputEventArchiveV2Id);

            await this.eventArchiveV2Client.RetrieveEventArchiveV2ByIdAsync(
                inputEventArchiveV2Id);

            // then
            this.archiveServiceResolutionCount.Should()
                .Be(expectedResolutionCount);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2ByIdAsync(
                    inputEventArchiveV2Id, It.IsAny<CancellationToken>()),
                        Times.Exactly(2));

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
