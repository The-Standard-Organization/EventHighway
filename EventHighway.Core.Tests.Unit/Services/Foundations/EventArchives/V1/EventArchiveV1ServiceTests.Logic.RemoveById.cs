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
        private async Task ShouldRemoveEventArchiveV1ByIdAsync()
        {
            // given
            Guid randomEventArchiveV1Id = GetRandomId();
            Guid inputEventArchiveV1Id = randomEventArchiveV1Id;

            EventArchiveV1 randomEventArchiveV1 =
                CreateRandomEventArchiveV1();

            EventArchiveV1 retrievedEventArchiveV1 =
                randomEventArchiveV1;

            EventArchiveV1 deletedEventArchiveV1 =
                retrievedEventArchiveV1;

            EventArchiveV1 expectedEventArchiveV1 =
                deletedEventArchiveV1.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventArchiveV1ByIdAsync(
                    inputEventArchiveV1Id))
                        .ReturnsAsync(retrievedEventArchiveV1);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteEventArchiveV1Async(
                    retrievedEventArchiveV1))
                        .ReturnsAsync(deletedEventArchiveV1);

            // when
            EventArchiveV1 actualEventArchiveV1 =
                await this.eventArchiveV1Service
                    .RemoveEventArchiveV1ByIdAsync(
                        inputEventArchiveV1Id);

            // then
            actualEventArchiveV1.Should().BeEquivalentTo(
                expectedEventArchiveV1);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventArchiveV1ByIdAsync(
                    inputEventArchiveV1Id),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventArchiveV1Async(
                    retrievedEventArchiveV1),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
