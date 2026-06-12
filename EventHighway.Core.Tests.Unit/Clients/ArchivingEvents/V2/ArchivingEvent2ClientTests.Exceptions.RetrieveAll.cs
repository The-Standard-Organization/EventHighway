// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.ArchivingEvents.V2
{
    public partial class ArchivingEvent2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveAllIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedArchivingEvent2ClientValidationException =
                new ArchivingEvent2ClientValidationException(
                    message: "Archiving event client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.archivingEvent2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<IQueryable<EventV2>> retrieveAllDeadEventV2sTask =
                this.archivingEvent2Client
                    .RetrieveAllDeadEventV2sWithListenersAsync(randomCancellationToken);

            ArchivingEvent2ClientValidationException actualArchivingEvent2ClientValidationException =
                await Assert.ThrowsAsync<ArchivingEvent2ClientValidationException>(
                    retrieveAllDeadEventV2sTask.AsTask);

            // then
            actualArchivingEvent2ClientValidationException.Should()
                .BeEquivalentTo(expectedArchivingEvent2ClientValidationException);

            this.archivingEvent2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.archivingEvent2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var archivingEvent2OrchestrationDependencyException =
                new ArchivingEvent2OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedArchivingEvent2ClientDependencyException =
                new ArchivingEvent2ClientDependencyException(
                    message: "Archiving event client dependency error occurred, contact support.",

                    innerException: archivingEvent2OrchestrationDependencyException
                        .InnerException as Xeption,

                    data: (archivingEvent2OrchestrationDependencyException
                        .InnerException as Xeption).Data);

            this.archivingEvent2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(archivingEvent2OrchestrationDependencyException);

            // when
            ValueTask<IQueryable<EventV2>> retrieveAllDeadEventV2sTask =
                this.archivingEvent2Client
                    .RetrieveAllDeadEventV2sWithListenersAsync(randomCancellationToken);

            ArchivingEvent2ClientDependencyException actualArchivingEvent2ClientDependencyException =
                await Assert.ThrowsAsync<ArchivingEvent2ClientDependencyException>(
                    retrieveAllDeadEventV2sTask.AsTask);

            // then
            actualArchivingEvent2ClientDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEvent2ClientDependencyException);

            this.archivingEvent2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.archivingEvent2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var archivingEvent2OrchestrationServiceException =
                new ArchivingEvent2OrchestrationServiceException(
                    someMessage,
                    someInnerException);

            var expectedArchivingEvent2ClientDependencyException =
                new ArchivingEvent2ClientDependencyException(
                    message: "Archiving event client dependency error occurred, contact support.",

                    innerException: archivingEvent2OrchestrationServiceException
                        .InnerException as Xeption,

                    data: (archivingEvent2OrchestrationServiceException
                        .InnerException as Xeption).Data);

            this.archivingEvent2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(archivingEvent2OrchestrationServiceException);

            // when
            ValueTask<IQueryable<EventV2>> retrieveAllDeadEventV2sTask =
                this.archivingEvent2Client
                    .RetrieveAllDeadEventV2sWithListenersAsync(randomCancellationToken);

            ArchivingEvent2ClientDependencyException actualArchivingEvent2ClientDependencyException =
                await Assert.ThrowsAsync<ArchivingEvent2ClientDependencyException>(
                    retrieveAllDeadEventV2sTask.AsTask);

            // then
            actualArchivingEvent2ClientDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEvent2ClientDependencyException);

            this.archivingEvent2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.archivingEvent2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
