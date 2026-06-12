// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.ArchivingEvents.V2
{
    public partial class ArchivingEvent2ClientTests
    {
        [Fact]
        public async Task ShouldRemoveEventV2AndListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;

            this.archivingEvent2OrchestrationServiceMock.Setup(service =>
                service.RemoveEventV2AndListenerEventV2sAsync(
                    inputEventV2,
                    randomCancellationToken))
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.archivingEvent2Client
                .RemoveEventV2AndListenerEventV2sAsync(
                    inputEventV2,
                    randomCancellationToken);

            // then
            this.archivingEvent2OrchestrationServiceMock.Verify(service =>
                service.RemoveEventV2AndListenerEventV2sAsync(
                    inputEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.archivingEvent2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
