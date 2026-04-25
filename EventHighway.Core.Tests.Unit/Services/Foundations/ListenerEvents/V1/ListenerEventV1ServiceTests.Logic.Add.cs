// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V1
{
    public partial class ListenerEventV1ServiceTests
    {
        [Fact]
        public async Task ShouldAddListenerEventV1Async()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            ListenerEventV1 randomListenerEventV1 =
                CreateRandomListenerEventV1(
                    randomDateTimeOffset);

            ListenerEventV1 inputListenerEventV1 =
                randomListenerEventV1;

            ListenerEventV1 storageListenerEventV1 =
                inputListenerEventV1;

            ListenerEventV1 expectedListenerEventV1 =
                storageListenerEventV1.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertListenerEventV1Async(
                    inputListenerEventV1))
                        .ReturnsAsync(storageListenerEventV1);

            // when
            ListenerEventV1 actualListenerEventV1 =
                await this.listenerEventV1Service
                    .AddListenerEventAsync(
                        inputListenerEventV1);

            // then
            actualListenerEventV1.Should().BeEquivalentTo(
                expectedListenerEventV1);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1Async(
                    inputListenerEventV1),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
