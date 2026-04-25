// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V1
{
    public partial class EventV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldSubmitEventV1Async()
        {
            // given
            EventV1 randomEventV1 =
                CreateRandomEventV1();

            EventV1 inputEventV1 = randomEventV1;
            EventV1 addedEventV1 = inputEventV1;

            EventV1 expectedEventV1 =
                addedEventV1.DeepClone();

            EventAddressV1 randomEventAddressV1 =
                CreateRandomEventAddressV1();

            EventAddressV1 retrievedEventAddressV1 =
                randomEventAddressV1;

            this.eventAddressV1ProcessingServiceMock.Setup(service =>
                service.RetrieveEventAddressByIdAsync(
                    inputEventV1.EventAddressId))
                        .ReturnsAsync(retrievedEventAddressV1);

            this.eventV1ProcessingServiceMock.Setup(service =>
                service.AddEventAsync(inputEventV1))
                    .ReturnsAsync(addedEventV1);

            // when
            EventV1 actualEventV1 =
                await this.eventV1OrchestrationService
                    .SubmitEventAsync(
                        inputEventV1);

            // then
            actualEventV1.Should().BeEquivalentTo(
                expectedEventV1);

            this.eventAddressV1ProcessingServiceMock.Verify(service =>
                service.RetrieveEventAddressByIdAsync(
                    inputEventV1.EventAddressId),
                        Times.Once);

            this.eventV1ProcessingServiceMock.Verify(broker =>
                broker.AddEventAsync(inputEventV1),
                    Times.Once);

            this.eventAddressV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.eventV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.eventCallV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
