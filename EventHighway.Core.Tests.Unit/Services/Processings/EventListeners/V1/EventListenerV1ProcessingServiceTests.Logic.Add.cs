// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventListeners.V1
{
    public partial class EventListenerV1ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldAddEventListenerV1Async()
        {
            // given
            EventListenerV1 randomEventListenerV1 =
                CreateRandomEventListenerV1();

            EventListenerV1 inputEventListenerV1 =
                randomEventListenerV1;

            EventListenerV1 addedEventListenerV1 =
                inputEventListenerV1;

            EventListenerV1 expectedEventListenerV1 =
                addedEventListenerV1.DeepClone();

            this.eventListenerV1ServiceMock.Setup(broker =>
                broker.AddEventListenerAsync(
                    inputEventListenerV1))
                        .ReturnsAsync(addedEventListenerV1);

            // when
            EventListenerV1 actualEventListenerV1 =
                await this.eventListenerV1ProcessingService
                    .AddEventListenerV1Async(
                        inputEventListenerV1);

            // then
            actualEventListenerV1.Should().BeEquivalentTo(
                expectedEventListenerV1);

            this.eventListenerV1ServiceMock.Verify(broker =>
                broker.AddEventListenerAsync(
                    inputEventListenerV1),
                        Times.Once);

            this.eventListenerV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
