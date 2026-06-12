// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldThrowValidationExceptionOnRemoveIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();

            var expectedArchivingEvent2ClientValidationException =
                new ArchivingEvent2ClientValidationException(
                    message: "Archiving event client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.archivingEvent2OrchestrationServiceMock.Setup(service =>
                service.RemoveEventV2AndListenerEventV2sAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask removeEventV2Task =
                this.archivingEvent2Client.RemoveEventV2AndListenerEventV2sAsync(
                    someEventV2,
                    randomCancellationToken);

            ArchivingEvent2ClientValidationException actualArchivingEvent2ClientValidationException =
                await Assert.ThrowsAsync<ArchivingEvent2ClientValidationException>(
                    removeEventV2Task.AsTask);

            // then
            actualArchivingEvent2ClientValidationException.Should()
                .BeEquivalentTo(expectedArchivingEvent2ClientValidationException);

            this.archivingEvent2OrchestrationServiceMock.Verify(service =>
                service.RemoveEventV2AndListenerEventV2sAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.archivingEvent2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();
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
                service.RemoveEventV2AndListenerEventV2sAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(archivingEvent2OrchestrationDependencyException);

            // when
            ValueTask removeEventV2Task =
                this.archivingEvent2Client.RemoveEventV2AndListenerEventV2sAsync(
                    someEventV2,
                    randomCancellationToken);

            ArchivingEvent2ClientDependencyException actualArchivingEvent2ClientDependencyException =
                await Assert.ThrowsAsync<ArchivingEvent2ClientDependencyException>(
                    removeEventV2Task.AsTask);

            // then
            actualArchivingEvent2ClientDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEvent2ClientDependencyException);

            this.archivingEvent2OrchestrationServiceMock.Verify(service =>
                service.RemoveEventV2AndListenerEventV2sAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.archivingEvent2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();
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
                service.RemoveEventV2AndListenerEventV2sAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(archivingEvent2OrchestrationServiceException);

            // when
            ValueTask removeEventV2Task =
                this.archivingEvent2Client.RemoveEventV2AndListenerEventV2sAsync(
                    someEventV2,
                    randomCancellationToken);

            ArchivingEvent2ClientDependencyException actualArchivingEvent2ClientDependencyException =
                await Assert.ThrowsAsync<ArchivingEvent2ClientDependencyException>(
                    removeEventV2Task.AsTask);

            // then
            actualArchivingEvent2ClientDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEvent2ClientDependencyException);

            this.archivingEvent2OrchestrationServiceMock.Verify(service =>
                service.RemoveEventV2AndListenerEventV2sAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.archivingEvent2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
