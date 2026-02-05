// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V1
{
    public partial class ListenerEventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfListenerEventV1ArchiveIsNullAndLogItAsync()
        {
            // given
            ListenerEventV1Archive nullListenerEventV1Archive = null;

            var nullListenerEventV1ArchiveException =
                new NullListenerEventV1ArchiveException(message: "Listener event archive is null.");

            var expectedListenerEventV1ArchiveValidationException =
                new ListenerEventV1ArchiveValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventV1ArchiveException);

            // when
            ValueTask<ListenerEventV1Archive> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveService.AddListenerEventV1ArchiveAsync(nullListenerEventV1Archive);

            ListenerEventV1ArchiveValidationException actualListenerEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ArchiveValidationException>(
                    addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveValidationException.Should().BeEquivalentTo(
                expectedListenerEventV1ArchiveValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnAddIfListenerEventV1ArchiveIsInvalidAndLogItAsync(
            string invalidText)
        {
            ListenerEventV1ArchiveStatus invalidListenerEventV1ArchiveStatus =
                GetInvalidEnum<ListenerEventV1ArchiveStatus>();

            var invalidListenerEventV1Archive = new ListenerEventV1Archive
            {
                Id = Guid.Empty,
                Response = invalidText,
                Status = invalidListenerEventV1ArchiveStatus,
                EventId = Guid.Empty,
                EventAddressId = Guid.Empty,
                EventListenerId = Guid.Empty
            };

            var invalidListenerEventV1ArchiveException =
                new InvalidListenerEventV1ArchiveException(
                    message: "Listener event archive is invalid, fix the errors and try again.");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventV1Archive.Id),
                values: "Required");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventV1Archive.EventId),
                values: "Required");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventV1Archive.EventAddressId),
                values: "Required");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventV1Archive.EventListenerId),
                values: "Required");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventV1Archive.Status),
                values: "Value is not recognized");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventV1Archive.CreatedDate),
                values: "Required");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventV1Archive.UpdatedDate),
                values: "Required");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventV1Archive.ArchivedDate),
                values: "Required");

            var expectedListenerEventV1ArchiveValidationException =
                new ListenerEventV1ArchiveValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV1ArchiveException);

            // when
            ValueTask<ListenerEventV1Archive> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveService.AddListenerEventV1ArchiveAsync(invalidListenerEventV1Archive);

            ListenerEventV1ArchiveValidationException actualListenerEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ArchiveValidationException>(
                    addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveValidationException.Should().BeEquivalentTo(
                expectedListenerEventV1ArchiveValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
