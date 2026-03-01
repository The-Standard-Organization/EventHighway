// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Net.Http;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventCalls.V1
{
    public partial class EventCallV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddV1IfEventCallV1IsNullAndLogItAsync()
        {
            // given
            EventCallV1 nullEventCallV1 = null;

            var nullEventCallV1Exception =
                new NullEventCallV1Exception(message: "Event call is null.");

            var expectedEventCallV1ValidationException =
                new EventCallV1ValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: nullEventCallV1Exception);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventCallV1Service.RunEventCallV1AsyncV1(nullEventCallV1);

            EventCallV1ValidationException actualEventCallV1ValidationException =
                await Assert.ThrowsAsync<EventCallV1ValidationException>(
                    runEventCallV1Task.AsTask);

            // then
            actualEventCallV1ValidationException.Should().BeEquivalentTo(
                expectedEventCallV1ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV1ValidationException))),
                        Times.Once);

            this.apiBrokerMock.Verify(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.apiBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRunV1IfHttpResponseMessageIsNullAndLogItAsync()
        {
            // given
            EventCallV1 randomEventCallV1 =
                CreateRandomEventCallV1();

            EventCallV1 inputEventCallV1 =
                randomEventCallV1;

            HttpResponseMessage nullHttpResponseMessage = null;

            var nullHttpResponseMessageException =
                new NullHttpResponseMessageException(message: "Http response message is null.");

            var expectedEventCallV1ValidationException =
                new EventCallV1ValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: nullHttpResponseMessageException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostAsyncV1(
                    inputEventCallV1.Content,
                    inputEventCallV1.Endpoint,
                    inputEventCallV1.Secret))
                        .ReturnsAsync(nullHttpResponseMessage);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventCallV1Service.RunEventCallV1AsyncV1(inputEventCallV1);

            EventCallV1ValidationException actualEventCallV1ValidationException =
                await Assert.ThrowsAsync<EventCallV1ValidationException>(
                    runEventCallV1Task.AsTask);

            // then
            actualEventCallV1ValidationException.Should().BeEquivalentTo(
                expectedEventCallV1ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV1ValidationException))),
                        Times.Once);

            this.apiBrokerMock.Verify(broker =>
                broker.PostAsyncV1(
                    inputEventCallV1.Content,
                    inputEventCallV1.Endpoint,
                    inputEventCallV1.Secret),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.apiBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnAddV1IfEventCallV1IsInvalidAndLogItAsync(
            string invalidText)
        {
            var invalidEventCallV1 = new EventCallV1
            {
                Endpoint = invalidText,
                Content = invalidText
            };

            var invalidEventCallV1Exception =
                new InvalidEventCallV1Exception(
                    message: "Event call is invalid, fix the errors and try again.");

            invalidEventCallV1Exception.AddData(
                key: nameof(EventCallV1.Endpoint),
                values: "Required");

            invalidEventCallV1Exception.AddData(
                key: nameof(EventCallV1.Content),
                values: "Required");

            var expectedEventCallV1ValidationException =
                new EventCallV1ValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: invalidEventCallV1Exception);

            // when
            ValueTask<EventCallV1> runEventCallV1Task =
                this.eventCallV1Service.RunEventCallV1AsyncV1(invalidEventCallV1);

            EventCallV1ValidationException actualEventCallV1ValidationException =
                await Assert.ThrowsAsync<EventCallV1ValidationException>(
                    runEventCallV1Task.AsTask);

            // then
            actualEventCallV1ValidationException.Should().BeEquivalentTo(
                expectedEventCallV1ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV1ValidationException))),
                        Times.Once);

            this.apiBrokerMock.Verify(broker =>
                broker.PostAsyncV1(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.apiBrokerMock.VerifyNoOtherCalls();
        }
    }
}
