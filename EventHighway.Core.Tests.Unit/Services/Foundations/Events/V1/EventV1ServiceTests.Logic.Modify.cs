// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V1
{
    public partial class EventV1ServiceTests
    {
        [Fact]
        public async Task ShouldModifyEventV1Async()
        {
            // given
            DateTimeOffset randomDateTime =
                GetRandomDateTimeOffset();

            int randomDaysAgo = GetRandomNegativeNumber();

            EventV1 randomEventV1 =
                CreateRandomEventV1(randomDateTime);

            EventV1 inputEventV1 =
                randomEventV1;

            inputEventV1.CreatedDate =
                randomDateTime.AddDays(randomDaysAgo);

            EventV1 storageEventV1 =
                inputEventV1.DeepClone();

            int randomSecondsAgo =
                GetRandomNegativeNumber();

            DateTimeOffset storageUpdatedDate =
                randomDateTime.AddSeconds(
                    randomSecondsAgo);

            storageEventV1.UpdatedDate =
                storageUpdatedDate;

            EventV1 persistedEventV1 =
                inputEventV1;

            EventV1 expectedEventV1 =
                persistedEventV1.DeepClone();

            Guid eventV1Id = inputEventV1.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventByIdV1Async(
                    eventV1Id))
                        .ReturnsAsync(
                            storageEventV1);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateEventV1Async(
                    inputEventV1))
                        .ReturnsAsync(persistedEventV1);

            // when
            EventV1 actualEventV1 =
                await this.eventV1Service
                    .ModifyEventAsync(
                        inputEventV1);

            // then
            actualEventV1.Should().BeEquivalentTo(
                expectedEventV1);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventByIdV1Async(
                    eventV1Id),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventV1Async(
                    inputEventV1),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
