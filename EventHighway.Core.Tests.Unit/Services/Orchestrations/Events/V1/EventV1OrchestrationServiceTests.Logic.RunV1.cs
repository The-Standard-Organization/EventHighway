// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V1
{
    public partial class EventV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRunEventCallV1AsyncV1()
        {
            // given
            EventCallV1 randomEventCallV1 =
                CreateRandomEventCallV1();

            EventCallV1 inputEventCallV1 =
                randomEventCallV1;

            EventCallV1 ranEventCallV1 =
                inputEventCallV1;

            EventCallV1 expectedEventCallV1 =
                inputEventCallV1.DeepClone();

            this.eventCallV1ProcessingServiceMock.Setup(service =>
                service.RunEventCallV1AsyncV1(
                    inputEventCallV1))
                        .ReturnsAsync(ranEventCallV1);

            // when
            EventCallV1 actualEventCallV1 =
                await this.eventV1OrchestrationService
                    .RunEventCallV1AsyncV1(inputEventCallV1);

            // then
            actualEventCallV1.Should().BeEquivalentTo(
                expectedEventCallV1);

            this.eventCallV1ProcessingServiceMock.Verify(service =>
                service.RunEventCallV1AsyncV1(
                    inputEventCallV1),
                        Times.Once);

            this.eventCallV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.eventV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.eventAddressV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
