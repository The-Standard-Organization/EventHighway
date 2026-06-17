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
        public async Task ShouldThrowValidationExceptionOnPurgeArchivedEventV2sIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTimeOffset();

            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedArchivingEventV2ClientValidationException =
                new ArchivingEventV2ClientValidationException(
                    message: "Archiving event client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.archivingEventV2CoordinationServiceMock.Setup(service =>
                service.PurgeArchivedEventV2sAsync(
                    It.IsAny<DateTimeOffset>(),
                        It.IsAny<CancellationToken>()))
                            .ThrowsAsync(validationException);

            // when
            ValueTask purgeArchivedEventV2sTask =
                this.archivingEventV2Client.PurgeArchivedEventV2sAsync(
                    someDateTime, randomCancellationToken);

            ArchivingEventV2ClientValidationException actualArchivingEventV2ClientValidationException =
                await Assert.ThrowsAsync<ArchivingEventV2ClientValidationException>(
                    purgeArchivedEventV2sTask.AsTask);

            // then
            actualArchivingEventV2ClientValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2ClientValidationException);

            this.archivingEventV2CoordinationServiceMock.Verify(service =>
                service.PurgeArchivedEventV2sAsync(
                    It.IsAny<DateTimeOffset>(),
                        It.IsAny<CancellationToken>()),
                            Times.Once);

            this.archivingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
