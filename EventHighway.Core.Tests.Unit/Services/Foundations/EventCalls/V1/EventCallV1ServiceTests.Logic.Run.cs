// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventCalls.V1
{
    public partial class EventCallV1ServiceTests
    {
        [Fact]
        public async Task ShouldRunEventCallV1Async()
        {
            // given
            EventCallV1 randomEventCallV1 =
                CreateRandomEventCallV1();

            EventCallV1 inputEventCallV1 =
                randomEventCallV1;

            string randomCallResponse =
                CreateRandomResponse();

            string postedCallResponse =
                randomCallResponse;

            EventCallV1 expectedEventCallV1 =
                inputEventCallV1.DeepClone();

            expectedEventCallV1.Response =
                postedCallResponse;

            this.apiBrokerMock.Setup(broker =>
                broker.PostAsync(
                    inputEventCallV1.Content,
                    inputEventCallV1.Endpoint,
                    inputEventCallV1.Secret))
                        .ReturnsAsync(postedCallResponse);

            // when
            EventCallV1 actualEventCallV1 =
                await this.eventCallV1Service
                    .RunEventCallAsync(inputEventCallV1);

            // then
            actualEventCallV1.Should().BeEquivalentTo(
                expectedEventCallV1);

            this.apiBrokerMock.Verify(broker =>
                broker.PostAsync(
                    inputEventCallV1.Content,
                    inputEventCallV1.Endpoint,
                    inputEventCallV1.Secret),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
