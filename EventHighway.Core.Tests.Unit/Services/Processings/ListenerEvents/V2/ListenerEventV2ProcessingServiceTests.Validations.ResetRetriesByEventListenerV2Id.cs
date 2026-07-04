// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V2
{
    public partial class ListenerEventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnResetRetriesByEventListenerV2IdIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidEventListenerV2Id = Guid.Empty;

            var invalidListenerEventV2ProcessingException =
                new InvalidListenerEventV2ProcessingException(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV2ProcessingException.UpsertDataList(
                key: nameof(ListenerEventV2.EventListenerV2Id),
                value: "Required");

            var expectedListenerEventV2ProcessingValidationException =
                new ListenerEventV2ProcessingValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV2ProcessingException);

            // when
            ValueTask resetRetriesTask =
                this.listenerEventV2ProcessingService
                    .ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                        invalidEventListenerV2Id, randomCancellationToken);

            ListenerEventV2ProcessingValidationException
                actualListenerEventV2ProcessingValidationException =
                    await Assert.ThrowsAsync<ListenerEventV2ProcessingValidationException>(
                        resetRetriesTask.AsTask);

            // then
            actualListenerEventV2ProcessingValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ProcessingValidationException))),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
