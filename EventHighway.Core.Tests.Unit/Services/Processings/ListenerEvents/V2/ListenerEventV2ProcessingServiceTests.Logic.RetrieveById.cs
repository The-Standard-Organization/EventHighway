// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V2
{
    public partial class ListenerEventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveListenerEventV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 randomListenerEventV2 = CreateRandomListenerEventV2();
            ListenerEventV2 retrievedListenerEventV2 = randomListenerEventV2;
            ListenerEventV2 expectedListenerEventV2 = retrievedListenerEventV2.DeepClone();
            Guid inputListenerEventV2Id = retrievedListenerEventV2.Id;

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveListenerEventV2ByIdAsync(
                    inputListenerEventV2Id,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedListenerEventV2);

            // when
            ListenerEventV2 actualListenerEventV2 =
                await this.listenerEventV2ProcessingService
                    .RetrieveListenerEventV2ByIdAsync(inputListenerEventV2Id, randomCancellationToken);

            // then
            actualListenerEventV2.Should().BeEquivalentTo(expectedListenerEventV2);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventV2ByIdAsync(
                    inputListenerEventV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
