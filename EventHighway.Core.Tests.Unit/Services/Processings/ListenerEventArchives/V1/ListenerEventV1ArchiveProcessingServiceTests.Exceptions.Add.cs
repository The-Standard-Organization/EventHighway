// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V1
{
    public partial class ListenerEventV1ArchiveProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            ListenerEventV1Archive someListenerEventV1Archive = CreateRandomListenerEventV1Archive();

            var expectedListenerEventV1ArchiveProcessingDependencyValidationException =
                new ListenerEventV1ArchiveProcessingDependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventV1ArchiveServiceMock.Setup(service =>
                service.AddListenerEventV1ArchiveAsync(
                    It.IsAny<ListenerEventV1Archive>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<ListenerEventV1Archive> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveProcessingService.AddListenerEventV1ArchiveAsync(
                    someListenerEventV1Archive);

            ListenerEventV1ArchiveProcessingDependencyValidationException
                actualListenerEventV1ArchiveProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<ListenerEventV1ArchiveProcessingDependencyValidationException>(
                        addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedListenerEventV1ArchiveProcessingDependencyValidationException);

            this.listenerEventV1ArchiveServiceMock.Verify(service =>
                service.AddListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveProcessingDependencyValidationException))),
                        Times.Once);

            this.listenerEventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
