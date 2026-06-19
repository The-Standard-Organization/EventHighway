// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchOfDeadIfDependencyErrorOccursAndLogItAsync(
            Xeption eventV2DependencyException)
        {
            // given
            int randomTake = GetRandomNumber();
            int inputTake = randomTake;

            var expectedEventV2ProcessingDependencyException =
                new EventV2ProcessingDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: eventV2DependencyException.InnerException as Xeption);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync())
                    .ThrowsAsync(eventV2DependencyException);

            // when
            var retrieveBatchOfDeadTask =
                this.eventV2ProcessingService
                    .RetrieveBatchOfDeadEventV2sAsync(inputTake);

            EventV2ProcessingDependencyException actualEventV2ProcessingDependencyException =
                await Assert.ThrowsAsync<EventV2ProcessingDependencyException>(
                    retrieveBatchOfDeadTask.AsTask);

            // then
            actualEventV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventV2ProcessingDependencyException);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingDependencyException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
