// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V1
{
    public partial class ListenerEventV1ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidListenerEventV1Id = Guid.Empty;

            var invalidListenerEventV1Exception =
                new InvalidListenerEventV1ProcessingException(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV1Exception.AddData(
                key: nameof(ListenerEventV1.Id),
                values: "Required");

            var expectedListenerEventV1ProcessingValidationException =
                new ListenerEventV1ProcessingValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV1Exception);

            // when
            ValueTask<ListenerEventV1> removeListenerEventV1ByIdTask =
                this.listenerEventV1ProcessingService
                    .RemoveListenerEventByIdAsync(
                        invalidListenerEventV1Id);

            ListenerEventV1ProcessingValidationException
                actualListenerEventV1ProcessingValidationException =
                    await Assert.ThrowsAsync<ListenerEventV1ProcessingValidationException>(
                        removeListenerEventV1ByIdTask.AsTask);

            // then
            actualListenerEventV1ProcessingValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV1ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ProcessingValidationException))),
                        Times.Once);

            this.listenerEventV1ServiceMock.Verify(broker =>
                broker.RemoveListenerEventByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
