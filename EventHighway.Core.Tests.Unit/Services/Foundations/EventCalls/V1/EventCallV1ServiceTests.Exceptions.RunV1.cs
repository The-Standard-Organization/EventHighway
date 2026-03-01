// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1.Exceptions;
using FluentAssertions;
using Moq;
using RESTFulSense.Exceptions;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventCalls.V1
{
    public partial class EventCallV1ServiceTests
    {
        [Theory]
        [MemberData(nameof(CriticalDependencyExceptions))]
        public async Task ShouldThrowCriticalDependencyExceptionOnRunV1IfCriticalDependencyErrorOccursAndLogItAsync(
            Xeption criticalDependencyException)
        {
            // given
            EventCallV1 someEventCallV1 = CreateRandomEventCallV1();

            var failedEventCallV1ConfigurationException =
                new FailedEventCallV1ConfigurationException(
                    message: "Failed event call configuration error occurred, contact support.",
                    innerException: criticalDependencyException);

            var expectedEventCallV1DependencyException =
                new EventCallV1DependencyException(
                    message: "Event call dependency error occurred, contact support.",
                    innerException: failedEventCallV1ConfigurationException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                        .ThrowsAsync(criticalDependencyException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventCallV1Service.RunEventCallV1AsyncV1(someEventCallV1);

            EventCallV1DependencyException actualEventCallV1DependencyException =
                await Assert.ThrowsAsync<EventCallV1DependencyException>(
                    runEventCallV1Task.AsTask);

            // then
            actualEventCallV1DependencyException.Should()
                .BeEquivalentTo(expectedEventCallV1DependencyException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventCallV1DependencyException))),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddV1IfHttpUnprocessableErrorOccursAndLogItAsync()
        {
            // given
            EventCallV1 someEventCallV1 = CreateRandomEventCallV1();
            var httpUnprocessableEntityException = new HttpResponseUnprocessableEntityException();

            var failedEventCallV1RequestException =
                new FailedEventCallV1RequestException(
                    message: "Failed event call request error occurred, fix the errors and try again.",
                    innerException: httpUnprocessableEntityException);

            var expectedEventCallV1DependencyValidationException =
                new EventCallV1DependencyValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: failedEventCallV1RequestException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                        .ThrowsAsync(httpUnprocessableEntityException);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventCallV1Service.RunEventCallV1AsyncV1(someEventCallV1);

            EventCallV1DependencyValidationException actualEventCallV1DependencyValidationException =
                await Assert.ThrowsAsync<EventCallV1DependencyValidationException>(
                    runEventCallV1Task.AsTask);

            // then
            actualEventCallV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventCallV1DependencyValidationException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV1DependencyValidationException))),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
