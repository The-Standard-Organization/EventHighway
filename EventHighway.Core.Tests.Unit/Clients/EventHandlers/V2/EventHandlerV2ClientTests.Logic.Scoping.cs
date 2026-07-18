// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventHandlers.V2
{
    public partial class EventHandlerV2ClientTests
    {
        [Fact]
        public async Task ShouldResolveServiceInNewScopePerOperationAsync()
        {
            // given
            var inputEventHandlerV2Query = new EventHandlerV2Query();
            int expectedResolutionCount = 2;

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventHandlerV2sByQueryAsync(
                    inputEventHandlerV2Query, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<EventHandlerV2>());

            // when
            await this.eventHandlerV2Client.RetrieveAllEventHandlerV2sAsync(
                inputEventHandlerV2Query);

            await this.eventHandlerV2Client.RetrieveAllEventHandlerV2sAsync(
                inputEventHandlerV2Query);

            // then
            this.eventHandlerProcessingServiceResolutionCount.Should()
                .Be(expectedResolutionCount);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventHandlerV2sByQueryAsync(
                    inputEventHandlerV2Query, It.IsAny<CancellationToken>()),
                        Times.Exactly(2));

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
