// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

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
        public async Task ShouldMarkEventV1AsImmediateAsync()
        {
            // given
            EventV1 randomEventV1 =
                CreateRandomEventV1();

            EventV1 inputEventV1 = randomEventV1;
            EventV1 modifiedEventV1 = inputEventV1;

            EventV1 expectedEventV1 =
                modifiedEventV1.DeepClone();

            this.eventV1ProcessingServiceMock.Setup(service =>
                service.MarkEventAsImmediateAsync(inputEventV1))
                    .ReturnsAsync(modifiedEventV1);

            // when
            EventV1 actualEventV1 =
                await this.eventV1OrchestrationService
                    .MarkEventAsImmediateAsync(
                        inputEventV1);

            // then
            actualEventV1.Should().BeEquivalentTo(
                expectedEventV1);

            this.eventV1ProcessingServiceMock.Verify(broker =>
                broker.MarkEventAsImmediateAsync(inputEventV1),
                    Times.Once);

            this.eventV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.eventAddressV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.eventCallV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
