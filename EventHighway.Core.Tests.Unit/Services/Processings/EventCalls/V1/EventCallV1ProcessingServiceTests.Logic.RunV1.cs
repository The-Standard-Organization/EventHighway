// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventCalls.V1
{
    public partial class EventCallV1ProcessingServiceTests
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

            this.eventCallV1ServiceMock.Setup(service =>
                service.RunEventCallV1AsyncV1(
                    inputEventCallV1))
                        .ReturnsAsync(ranEventCallV1);

            // when
            EventCallV1 actualEventCallV1 =
                await this.eventCallV1ProcessingService
                    .RunEventCallV1AsyncV1(inputEventCallV1);

            // then
            actualEventCallV1.Should().BeEquivalentTo(
                expectedEventCallV1);

            this.eventCallV1ServiceMock.Verify(service =>
                service.RunEventCallV1AsyncV1(
                    inputEventCallV1),
                        Times.Once);

            this.eventCallV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
