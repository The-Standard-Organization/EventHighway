// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V1
{
    public partial class EventArchiveV1ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            EventArchiveV1 someEventArchiveV1 = CreateRandomEventArchiveV1();

            var expectedEventArchiveV1ProcessingDependencyValidationException =
                new EventArchiveV1ProcessingDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventArchiveV1ServiceMock.Setup(service =>
                service.AddEventArchiveV1Async(It.IsAny<EventArchiveV1>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventArchiveV1> addEventArchiveV1Task =
                this.eventArchiveV1ProcessingService.AddEventArchiveV1Async(someEventArchiveV1);

            EventArchiveV1ProcessingDependencyValidationException
                actualEventArchiveV1ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventArchiveV1ProcessingDependencyValidationException>(
                        addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1ProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV1ProcessingDependencyValidationException);

            this.eventArchiveV1ServiceMock.Verify(service =>
                service.AddEventArchiveV1Async(It.IsAny<EventArchiveV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            EventArchiveV1 someEventArchiveV1 = CreateRandomEventArchiveV1();

            var expectedEventArchiveV1ProcessingDependencyException =
                new EventArchiveV1ProcessingDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventArchiveV1ServiceMock.Setup(service =>
                service.AddEventArchiveV1Async(It.IsAny<EventArchiveV1>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventArchiveV1> addEventArchiveV1Task =
                this.eventArchiveV1ProcessingService.AddEventArchiveV1Async(someEventArchiveV1);

            EventArchiveV1ProcessingDependencyException
                actualEventArchiveV1ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventArchiveV1ProcessingDependencyException>(
                        addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventArchiveV1ProcessingDependencyException);

            this.eventArchiveV1ServiceMock.Verify(service =>
                service.AddEventArchiveV1Async(It.IsAny<EventArchiveV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1ProcessingDependencyException))),
                        Times.Once);

            this.eventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            EventArchiveV1 someEventArchiveV1 = CreateRandomEventArchiveV1();
            var serviceException = new Exception();

            var failedEventArchiveV1ProcessingServiceException =
                new FailedEventArchiveV1ProcessingServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventArchiveV1ProcessingExceptionException =
                new EventArchiveV1ProcessingServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventArchiveV1ProcessingServiceException);

            this.eventArchiveV1ServiceMock.Setup(service =>
                service.AddEventArchiveV1Async(It.IsAny<EventArchiveV1>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventArchiveV1> addEventArchiveV1Task =
                this.eventArchiveV1ProcessingService.AddEventArchiveV1Async(
                    someEventArchiveV1);

            EventArchiveV1ProcessingServiceException
                actualEventArchiveV1ProcessingServiceException =
                    await Assert.ThrowsAsync<EventArchiveV1ProcessingServiceException>(
                        addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1ProcessingServiceException.Should().BeEquivalentTo(
                expectedEventArchiveV1ProcessingExceptionException);

            this.eventArchiveV1ServiceMock.Verify(service =>
                service.AddEventArchiveV1Async(It.IsAny<EventArchiveV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1ProcessingExceptionException))),
                        Times.Once);

            this.eventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
