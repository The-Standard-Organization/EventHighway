// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V1
{
    public partial class EventArchiveV1ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldAddEventArchiveV1Async()
        {
            // given
            EventArchiveV1 randomEventArchiveV1 =
                CreateRandomEventArchiveV1();

            EventArchiveV1 inputEventArchiveV1 =
                randomEventArchiveV1;

            EventArchiveV1 addedEventArchiveV1 =
                inputEventArchiveV1;

            EventArchiveV1 expectedEventArchiveV1 =
                addedEventArchiveV1.DeepClone();

            this.eventArchiveV1ServiceMock.Setup(service =>
                service.AddEventArchiveV1Async(
                    inputEventArchiveV1))
                        .ReturnsAsync(addedEventArchiveV1);

            // when
            EventArchiveV1 actualEventArchiveV1 =
                await this.eventArchiveV1ProcessingService
                    .AddEventArchiveV1Async(
                        inputEventArchiveV1);

            // then
            actualEventArchiveV1.Should().BeEquivalentTo(
                expectedEventArchiveV1);

            this.eventArchiveV1ServiceMock.Verify(service =>
                service.AddEventArchiveV1Async(
                    inputEventArchiveV1),
                        Times.Once);

            this.eventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
