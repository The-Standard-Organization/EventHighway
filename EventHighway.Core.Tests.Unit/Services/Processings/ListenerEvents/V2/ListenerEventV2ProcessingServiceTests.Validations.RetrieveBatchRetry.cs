// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
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
        public async Task ShouldThrowValidationExceptionOnRetrieveBatchRetryIfTakeIsNegativeAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int negativeTake = -1;

            var invalidListenerEventV2ProcessingException =
                new InvalidListenerEventV2ProcessingException(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV2ProcessingException.UpsertDataList(
                key: "Take",
                value: "Value must be greater than or equal to 0");

            var expectedListenerEventV2ProcessingValidationException =
                new ListenerEventV2ProcessingValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV2ProcessingException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchRetryTask =
                this.listenerEventV2ProcessingService
                    .RetrieveBatchOfRetryListenerEventV2sAsync(negativeTake, randomCancellationToken);

            ListenerEventV2ProcessingValidationException
                actualListenerEventV2ProcessingValidationException =
                    await Assert.ThrowsAsync<ListenerEventV2ProcessingValidationException>(
                        retrieveBatchRetryTask.AsTask);

            // then
            actualListenerEventV2ProcessingValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ProcessingValidationException))),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
