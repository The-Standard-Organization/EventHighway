// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListeners.V1
{
    public partial class EventListenerV1ServiceTests
    {
        [Fact]
        public async Task ShouldAddEventListenerV1Async()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            EventListenerV1 randomEventListenerV1 =
                CreateRandomEventListenerV1(
                    randomDateTimeOffset);

            EventListenerV1 inputEventListenerV1 =
                randomEventListenerV1;

            EventListenerV1 storageEventListenerV1 =
                inputEventListenerV1;

            EventListenerV1 expectedEventListenerV1 =
                storageEventListenerV1.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertEventListenerV1Async(
                    inputEventListenerV1))
                        .ReturnsAsync(storageEventListenerV1);

            // when
            EventListenerV1 actualEventListenerV1 =
                await this.eventListenerV1Service
                    .AddEventListenerAsync(
                        inputEventListenerV1);

            // then
            actualEventListenerV1.Should().BeEquivalentTo(
                expectedEventListenerV1);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV1Async(
                    inputEventListenerV1),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
