// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V2
{
    public partial class EventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldRemoveEventV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventV2Id = GetRandomId();
            Guid inputEventV2Id = randomEventV2Id;
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 removedEventV2 = randomEventV2;

            EventV2 expectedEventV2 =
                removedEventV2.DeepClone();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RemoveEventV2ByIdAsync(
                    inputEventV2Id,
                    randomCancellationToken))
                        .ReturnsAsync(removedEventV2);

            // when
            EventV2 actualEventV2 =
                await this.eventV2CoordinationService
                    .RemoveEventV2ByIdAsync(
                        inputEventV2Id,
                        randomCancellationToken);

            // then
            actualEventV2.Should()
                .BeEquivalentTo(expectedEventV2);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RemoveEventV2ByIdAsync(
                    inputEventV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2OrchestrationServiceMock
                .VerifyNoOtherCalls();

            this.eventListenerV2OrchestrationServiceMock
                .VerifyNoOtherCalls();

            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
