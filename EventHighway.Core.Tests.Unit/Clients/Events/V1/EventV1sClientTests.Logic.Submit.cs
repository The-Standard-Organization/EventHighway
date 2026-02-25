// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.Events.V1
{
    public partial class EventV1sClientTests
    {
        [Fact]
        public async Task ShouldSubmitEventV1Async()
        {
            // given 
            EventV1 randomEventV1 = CreateRandomEventV1();
            EventV1 inputEventV1 = randomEventV1;
            EventV1 submittedEventV1 = inputEventV1;

            EventV1 expectedEventV1 =
                submittedEventV1.DeepClone();

            this.eventV1CoordinationServiceMock.Setup(service =>
                service.SubmitEventV1Async(inputEventV1))
                    .ReturnsAsync(submittedEventV1);

            // when
            EventV1 actualEventV1 =
                await this.eventV1SClient
                    .SubmitEventV1Async(inputEventV1);

            // then
            actualEventV1.Should()
                .BeEquivalentTo(expectedEventV1);

            this.eventV1CoordinationServiceMock.Verify(service =>
                service.SubmitEventV1Async(inputEventV1),
                    Times.Once);

            this.eventV1CoordinationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
