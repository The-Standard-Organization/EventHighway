// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V1
{
    public partial class ListenerEventV1ArchiveProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfListenerEventV1ArchiveIsNullAndLogItAsync()
        {
            // given
            ListenerEventV1Archive nullListenerEventV1Archive = null;

            var expectedListenerEventV1ArchiveProcessingValidationException =
                new NullListenerEventV1ArchiveProcessingException(
                    message: "Listener event archive is null.");
            // when
            ValueTask<ListenerEventV1Archive> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveProcessingService.AddListenerEventV1ArchiveAsync(
                    nullListenerEventV1Archive);

            ListenerEventV1ArchiveProcessingValidationException actualListenerEventV1ArchiveProcessingValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ArchiveProcessingValidationException>(
                    addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveProcessingValidationException.Should().BeEquivalentTo(
                expectedListenerEventV1ArchiveProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveProcessingValidationException))),
                        Times.Once);

            this.listenerEventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}