// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

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
    }
}
