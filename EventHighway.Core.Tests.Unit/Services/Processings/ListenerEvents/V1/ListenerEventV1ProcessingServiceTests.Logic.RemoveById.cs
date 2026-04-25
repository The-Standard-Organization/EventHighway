// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V1
{
    public partial class ListenerEventV1ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRemoveListenerEventV1ByIdAsync()
        {
            // given
            Guid randomListenerEventId = GetRandomId();
            Guid inputListenerEventId = randomListenerEventId;

            ListenerEventV1 randomListenerEventV1 =
                CreateRandomListenerEventV1();

            ListenerEventV1 removedListenerEventV1 =
                randomListenerEventV1;

            ListenerEventV1 expectedListenerEventV1 =
                removedListenerEventV1.DeepClone();

            this.listenerEventV1ServiceMock.Setup(service =>
                service.RemoveListenerEventByIdAsync(
                    inputListenerEventId))
                        .ReturnsAsync(removedListenerEventV1);

            // when
            ListenerEventV1 actualListenerEventV1 =
                await this.listenerEventV1ProcessingService
                    .RemoveListenerEventByIdAsync(
                        inputListenerEventId);

            // then
            actualListenerEventV1.Should()
                .BeEquivalentTo(expectedListenerEventV1);

            this.listenerEventV1ServiceMock.Verify(service =>
                service.RemoveListenerEventByIdAsync(
                    inputListenerEventId),
                        Times.Once);

            this.listenerEventV1ServiceMock
                .VerifyNoOtherCalls();

            this.loggingBrokerMock
                .VerifyNoOtherCalls();
        }
    }
}
