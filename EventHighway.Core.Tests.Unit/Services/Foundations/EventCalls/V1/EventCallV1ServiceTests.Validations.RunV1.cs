// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

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
    }
}
