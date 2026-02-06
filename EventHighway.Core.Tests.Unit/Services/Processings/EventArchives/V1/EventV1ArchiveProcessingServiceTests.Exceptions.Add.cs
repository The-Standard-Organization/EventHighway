// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V1
{
    public partial class EventV1ArchiveProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            EventV1Archive someEventV1Archive = CreateRandomEventV1Archive();

            var expectedEventV1ArchiveProcessingDependencyValidationException =
                new EventV1ArchiveProcessingDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventV1ArchiveServiceMock.Setup(service =>
                service.AddEventV1ArchiveAsync(It.IsAny<EventV1Archive>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventV1Archive> addEventV1ArchiveTask =
                this.eventV1ArchiveProcessingService.AddEventV1ArchiveAsync(someEventV1Archive);

            EventV1ArchiveProcessingDependencyValidationException
                actualEventV1ArchiveProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1ArchiveProcessingDependencyValidationException>(
                        addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveProcessingDependencyValidationException);

            this.eventV1ArchiveServiceMock.Verify(service =>
                service.AddEventV1ArchiveAsync(It.IsAny<EventV1Archive>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveProcessingDependencyValidationException))),
                        Times.Once);

            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            EventV1Archive someEventV1Archive = CreateRandomEventV1Archive();

            var expectedEventV1ArchiveProcessingDependencyException =
                new EventV1ArchiveProcessingDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventV1ArchiveServiceMock.Setup(service =>
                service.AddEventV1ArchiveAsync(It.IsAny<EventV1Archive>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventV1Archive> addEventV1ArchiveTask =
                this.eventV1ArchiveProcessingService.AddEventV1ArchiveAsync(someEventV1Archive);

            EventV1ArchiveProcessingDependencyException
                actualEventV1ArchiveProcessingDependencyException =
                    await Assert.ThrowsAsync<EventV1ArchiveProcessingDependencyException>(
                        addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventV1ArchiveProcessingDependencyException);

            this.eventV1ArchiveServiceMock.Verify(service =>
                service.AddEventV1ArchiveAsync(It.IsAny<EventV1Archive>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveProcessingDependencyException))),
                        Times.Once);

            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }


    }
}
