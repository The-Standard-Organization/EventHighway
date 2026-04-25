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
        private async Task ShouldRemoveListenerEventV1ByIdAsync()
        {
            // given
            Guid randomListenerEventV1Id = GetRandomId();
            Guid inputListenerEventV1Id = randomListenerEventV1Id;

            ListenerEventV1 randomListenerEventV1 =
                CreateRandomListenerEventV1();

            ListenerEventV1 retrievedListenerEventV1 =
                randomListenerEventV1;

            ListenerEventV1 deletedListenerEventV1 =
                retrievedListenerEventV1;

            ListenerEventV1 expectedListenerEventV1 =
                deletedListenerEventV1.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectListenerEventByIdV1Async(
                    inputListenerEventV1Id))
                        .ReturnsAsync(retrievedListenerEventV1);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteListenerEventV1Async(
                    retrievedListenerEventV1))
                        .ReturnsAsync(deletedListenerEventV1);

            // when
            ListenerEventV1 actualListenerEventV1 =
                await this.listenerEventV1Service
                    .RemoveListenerEventByIdAsync(
                        inputListenerEventV1Id);

            // then
            actualListenerEventV1.Should().BeEquivalentTo(
                expectedListenerEventV1);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventByIdV1Async(
                    inputListenerEventV1Id),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteListenerEventV1Async(
                    retrievedListenerEventV1),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
