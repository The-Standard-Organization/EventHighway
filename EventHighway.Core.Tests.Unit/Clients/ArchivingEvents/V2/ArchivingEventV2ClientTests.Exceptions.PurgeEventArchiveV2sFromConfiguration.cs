// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.ArchivingEvents.V2
{
    public partial class ArchivingEventV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnPurgeFromConfigurationIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken someCancellationToken = TestContext.Current.CancellationToken;

            var expectedArchivingEventV2ClientValidationException =
                new ArchivingEventV2ClientValidationException(
                    message: "Archiving event client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.archivingEventV2CoordinationServiceMock.Setup(service =>
                service.PurgeEventArchiveV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask purgeEventArchiveV2sTask =
                this.archivingEventV2Client
                    .PurgeEventArchiveV2sAsync(someCancellationToken);

            ArchivingEventV2ClientValidationException actualArchivingEventV2ClientValidationException =
                await Assert.ThrowsAsync<ArchivingEventV2ClientValidationException>(
                    purgeEventArchiveV2sTask.AsTask);

            // then
            actualArchivingEventV2ClientValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2ClientValidationException);

            this.archivingEventV2CoordinationServiceMock.Verify(service =>
                service.PurgeEventArchiveV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.archivingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnPurgeFromConfigurationIfDependencyErrorOccursAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken someCancellationToken = TestContext.Current.CancellationToken;

            var expectedArchivingEventV2ClientDependencyException =
                new ArchivingEventV2ClientDependencyException(
                    message: "Archiving event client dependency error occurred, contact support.",

                    innerException: dependencyException
                        .InnerException as Xeption,

                    data: (dependencyException
                        .InnerException as Xeption).Data);

            this.archivingEventV2CoordinationServiceMock.Setup(service =>
                service.PurgeEventArchiveV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask purgeEventArchiveV2sTask =
                this.archivingEventV2Client
                    .PurgeEventArchiveV2sAsync(someCancellationToken);

            ArchivingEventV2ClientDependencyException actualArchivingEventV2ClientDependencyException =
                await Assert.ThrowsAsync<ArchivingEventV2ClientDependencyException>(
                    purgeEventArchiveV2sTask.AsTask);

            // then
            actualArchivingEventV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEventV2ClientDependencyException);

            this.archivingEventV2CoordinationServiceMock.Verify(service =>
                service.PurgeEventArchiveV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.archivingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnPurgeFromConfigurationIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken = TestContext.Current.CancellationToken;

            var someXeption = new Xeption(message: GetRandomString());

            var expectedArchivingEventV2ClientServiceException =
                new ArchivingEventV2ClientServiceException(
                    message: "Archiving event client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.archivingEventV2CoordinationServiceMock.Setup(service =>
                service.PurgeEventArchiveV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask purgeEventArchiveV2sTask =
                this.archivingEventV2Client
                    .PurgeEventArchiveV2sAsync(randomCancellationToken);

            ArchivingEventV2ClientServiceException actualArchivingEventV2ClientServiceException =
                await Assert.ThrowsAsync<ArchivingEventV2ClientServiceException>(
                    purgeEventArchiveV2sTask.AsTask);

            // then
            actualArchivingEventV2ClientServiceException.Should()
                .BeEquivalentTo(expectedArchivingEventV2ClientServiceException);

            this.archivingEventV2CoordinationServiceMock.Verify(service =>
                service.PurgeEventArchiveV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.archivingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnPurgeFromConfigurationAsync()
        {
            // given
            CancellationToken someCancellationToken = TestContext.Current.CancellationToken;

            var operationCanceledException =
                new OperationCanceledException();

            this.archivingEventV2CoordinationServiceMock.Setup(service =>
                service.PurgeEventArchiveV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask purgeEventArchiveV2sTask =
                this.archivingEventV2Client
                    .PurgeEventArchiveV2sAsync(someCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    purgeEventArchiveV2sTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.archivingEventV2CoordinationServiceMock.Verify(service =>
                service.PurgeEventArchiveV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.archivingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
