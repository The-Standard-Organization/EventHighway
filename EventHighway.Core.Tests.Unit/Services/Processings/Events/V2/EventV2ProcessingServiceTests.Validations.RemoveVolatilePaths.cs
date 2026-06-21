// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveVolatilePathsIfEventV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 nullEventV2 = null;

            var nullEventV2ProcessingException =
                new NullEventV2ProcessingException(message: "Event is null.");

            var expectedEventV2ProcessingValidationException =
                new EventV2ProcessingValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: nullEventV2ProcessingException);

            // when
            ValueTask<string> removeVolatilePathsTask =
                this.eventV2ProcessingService.RemoveVolatilePathsAsync(
                    nullEventV2,
                    randomCancellationToken);

            EventV2ProcessingValidationException
                actualEventV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventV2ProcessingValidationException>(
                        removeVolatilePathsTask.AsTask);

            // then
            actualEventV2ProcessingValidationException.Should().BeEquivalentTo(
                expectedEventV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingValidationException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
