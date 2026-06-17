// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2CoordinationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventArchiveV2ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnPurgeArchivedEventV2sIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            DateTimeOffset olderThan = GetRandomDateTimeOffset();

            var expectedArchivingEventV2CoordinationDependencyValidationException =
                new ArchivingEventV2CoordinationDependencyValidationException(
                    message: "Archiving event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventArchiveV2OrchestrationServiceMock
                .Setup(service => service.PurgeArchivedEventV2sAsync(
                    It.IsAny<DateTimeOffset>(),
                        It.IsAny<CancellationToken>()))
                            .ThrowsAsync(validationException);

            // when
            ValueTask purgeTask =
                this.archivingEventV2CoordinationService
                    .PurgeArchivedEventV2sAsync(
                        olderThan,
                        TestContext.Current.CancellationToken);

            ArchivingEventV2CoordinationDependencyValidationException
                actualArchivingEventV2CoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<ArchivingEventV2CoordinationDependencyValidationException>(
                        purgeTask.AsTask);

            // then
            actualArchivingEventV2CoordinationDependencyValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2CoordinationDependencyValidationException);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.PurgeArchivedEventV2sAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedArchivingEventV2CoordinationDependencyValidationException))),
                       Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ArchivingEventV2DependencyExceptions))]
        [MemberData(nameof(EventArchiveV2DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnPurgeArchivedEventV2sIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTimeOffset();

            var expectedArchivingEventV2CoordinationDependencyException =
                new ArchivingEventV2CoordinationDependencyException(
                    message: "Archiving event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventArchiveV2OrchestrationServiceMock
                .Setup(service => service.PurgeArchivedEventV2sAsync(
                    It.IsAny<DateTimeOffset>(),
                        It.IsAny<CancellationToken>()))
                            .ThrowsAsync(dependencyException);

            // when
            ValueTask purgeArchiveEventsTask =
                this.archivingEventV2CoordinationService
                    .PurgeArchivedEventV2sAsync(
                        someDateTime,
                        TestContext.Current.CancellationToken);

            ArchivingEventV2CoordinationDependencyException
                actualArchivingEventV2CoordinationDependencyException =
                    await Assert.ThrowsAsync<ArchivingEventV2CoordinationDependencyException>(
                        purgeArchiveEventsTask.AsTask);

            // then
            actualArchivingEventV2CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEventV2CoordinationDependencyException);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.PurgeArchivedEventV2sAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
