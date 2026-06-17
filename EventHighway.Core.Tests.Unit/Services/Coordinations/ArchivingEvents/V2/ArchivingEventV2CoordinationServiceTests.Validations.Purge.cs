// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnPurgeArchivedEventV2sAsyncAndLogItAsync()
        {
            // given
            DateTimeOffset olderThan = default;


            var invalidEventV1Exception =
                new InvalidArchivingEventV2CoordinationException(
                    message: "Archiving event is invalid, fix the errors and try again.");

            invalidEventV1Exception.AddData(
                key: nameof(olderThan),
                values: "Required.");

            var expectedException =
                new ArchivingEventV2CoordinationValidationException(
                    message: "Archiving event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1Exception);

            // when
            ValueTask task =
                this.archivingEventV2CoordinationService
                    .PurgeArchivedEventV2sAsync(
                        olderThan,
                        CancellationToken.None);

            ArchivingEventV2CoordinationValidationException actualException =
                await Assert.ThrowsAsync<ArchivingEventV2CoordinationValidationException>(
                    task.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedException);

            this.loggingBrokerMock.Verify(service =>
                service.LogErrorAsync(
                    It.IsAny<Exception>()),
                        Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.PurgeArchivedEventV2sAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);


            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
