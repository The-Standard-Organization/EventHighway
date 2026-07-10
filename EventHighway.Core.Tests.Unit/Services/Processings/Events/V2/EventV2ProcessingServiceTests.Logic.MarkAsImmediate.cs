// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldMarkEventV2AsImmediateAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            DateTimeOffset retrievedDateTimeOffset =
                randomDateTimeOffset;

            EventV2 randomEventV2 =
                CreateRandomEventV2(
                    eventV2Type: EventTypeV2.Scheduled);

            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.Type = EventTypeV2.Immediate;
            inputEventV2.UpdatedDate = retrievedDateTimeOffset;
            EventV2 modifiedEventV2 = inputEventV2;

            EventV2 expectedEventV2 =
                modifiedEventV2.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(retrievedDateTimeOffset);

            this.eventV2ServiceMock.Setup(broker =>
                broker.ModifyEventV2Async(
                    inputEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(modifiedEventV2);

            // when
            EventV2 actualEventV2 =
                await this.eventV2ProcessingService
                    .MarkEventV2AsImmediateAsync(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualEventV2.Should().BeEquivalentTo(
                expectedEventV2);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventV2ServiceMock.Verify(broker =>
                broker.ModifyEventV2Async(
                    inputEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2ServiceMock
                .VerifyNoOtherCalls();

            this.loggingBrokerMock
                .VerifyNoOtherCalls();
        }
    }
}
