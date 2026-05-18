// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventArchiveV1ServiceTests
    {
        [Fact]
        public async Task ShouldAddEventArchiveV1Async()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            EventArchiveV1 randomEventArchiveV1 =
                CreateRandomEventArchiveV1(
                    date: randomDateTimeOffset);

            EventArchiveV1 inputEventArchiveV1 =
                randomEventArchiveV1;

            EventArchiveV1 insertedEventArchiveV1 =
                inputEventArchiveV1;

            EventArchiveV1 expectedEventArchiveV1 =
                insertedEventArchiveV1.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertEventArchiveV1Async(
                    inputEventArchiveV1))
                        .ReturnsAsync(insertedEventArchiveV1);

            // when
            EventArchiveV1 actualEventArchiveV1 =
                await this.eventArchiveV1Service
                    .AddEventArchiveV1Async(
                        inputEventArchiveV1);

            // then
            actualEventArchiveV1.Should().BeEquivalentTo(
                expectedEventArchiveV1);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventArchiveV1Async(
                    inputEventArchiveV1),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
