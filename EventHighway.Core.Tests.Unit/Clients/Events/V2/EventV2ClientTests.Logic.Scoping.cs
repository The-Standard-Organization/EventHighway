// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.Events.V2
{
    public partial class EventV2ClientTests
    {
        [Fact]
        public async Task ShouldResolveCoordinationServiceInNewScopePerOperationAsync()
        {
            // given
            EventV2 randomEventV2 = CreateRandomEventV2();
            Guid inputEventV2Id = randomEventV2.Id;
            int expectedResolutionCount = 2;

            this.eventV2CoordinationServiceMock.Setup(service =>
                service.RetrieveEventV2ByIdAsync(
                    inputEventV2Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(randomEventV2);

            // when
            await this.eventV2Client.RetrieveEventV2ByIdAsync(inputEventV2Id);
            await this.eventV2Client.RetrieveEventV2ByIdAsync(inputEventV2Id);

            // then
            this.coordinationServiceResolutionCount.Should()
                .Be(expectedResolutionCount);

            this.eventV2CoordinationServiceMock.Verify(service =>
                service.RetrieveEventV2ByIdAsync(
                    inputEventV2Id, It.IsAny<CancellationToken>()),
                        Times.Exactly(2));

            this.eventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
