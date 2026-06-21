// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRemoveVolatilePathsAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 randomEventV2 =
                CreateRandomEventV2();

            EventV2 inputEventV2 = randomEventV2;
            string randomCleanedContent = GetRandomString();
            string expectedContent = randomCleanedContent;

            this.eventV2ServiceMock
                .Setup(service => service.RemoveVolatilePathsAsync(
                    inputEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(randomCleanedContent);

            // when
            string actualContent =
                await this.eventV2ProcessingService
                    .RemoveVolatilePathsAsync(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualContent.Should().Be(expectedContent);

            this.eventV2ServiceMock.Verify(service =>
                service.RemoveVolatilePathsAsync(
                    inputEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
