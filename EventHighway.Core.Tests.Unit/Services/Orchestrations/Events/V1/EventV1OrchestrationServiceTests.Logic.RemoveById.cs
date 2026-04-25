// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V1
{
    public partial class EventV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRemoveEventV1ByIdAsync()
        {
            // given
            Guid randomEventV1Id = GetRandomId();
            Guid inputEventV1Id = randomEventV1Id;
            EventV1 randomEventV1 = CreateRandomEventV1();
            EventV1 removedEventV1 = randomEventV1;

            EventV1 expectedEventV1 =
                removedEventV1.DeepClone();

            this.eventV1ProcessingServiceMock.Setup(service =>
                service.RemoveEventByIdAsync(
                    inputEventV1Id))
                        .ReturnsAsync(removedEventV1);

            // when
            EventV1 actualEventV1 =
                await this.eventV1OrchestrationService
                    .RemoveEventByIdAsync(
                        inputEventV1Id);

            // then
            actualEventV1.Should()
                .BeEquivalentTo(expectedEventV1);

            this.eventV1ProcessingServiceMock.Verify(service =>
                service.RemoveEventByIdAsync(
                    inputEventV1Id),
                        Times.Once);

            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
