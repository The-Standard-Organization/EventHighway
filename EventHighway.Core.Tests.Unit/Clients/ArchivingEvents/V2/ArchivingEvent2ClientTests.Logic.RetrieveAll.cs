// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.ArchivingEvents.V2
{
    public partial class ArchivingEvent2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllDeadEventV2sWithListenersAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventV2> randomEventV2s =
                CreateRandomEventV2s();

            IQueryable<EventV2> retrievedEventV2s =
                randomEventV2s;

            IQueryable<EventV2> expectedEventV2s =
                retrievedEventV2s.DeepClone();

            this.archivingEvent2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventV2s);

            // when
            IQueryable<EventV2> actualEventV2s =
                await this.archivingEvent2Client
                    .RetrieveAllDeadEventV2sWithListenersAsync(randomCancellationToken);

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.archivingEvent2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(randomCancellationToken),
                    Times.Once);

            this.archivingEvent2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
