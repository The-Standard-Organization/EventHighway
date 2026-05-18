// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V1
{
    public partial class ListenerEventArchiveV1ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfListenerEventArchiveV1IsNullAndLogItAsync()
        {
            // given
            ListenerEventArchiveV1 nullListenerEventArchiveV1 = null;

            var nullListenerEventArchiveV1ProcessingException =
                new NullListenerEventArchiveV1ProcessingException(
                    message: "Listener event archive is null.");

            var expectedListenerEventArchiveV1ProcessingValidationException =
                new ListenerEventArchiveV1ProcessingValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventArchiveV1ProcessingException);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventArchiveV1Task =
                this.listenerEventArchiveV1ProcessingService.AddListenerEventArchiveV1Async(
                    nullListenerEventArchiveV1);

            ListenerEventArchiveV1ProcessingValidationException actualListenerEventArchiveV1ProcessingValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV1ProcessingValidationException>(
                    addListenerEventArchiveV1Task.AsTask);

            // then
            actualListenerEventArchiveV1ProcessingValidationException.Should().BeEquivalentTo(
                expectedListenerEventArchiveV1ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV1ProcessingValidationException))),
                        Times.Once);

            this.listenerEventArchiveV1ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV1Async(
                    It.IsAny<ListenerEventArchiveV1>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}