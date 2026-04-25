// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

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
        public async Task ShouldThrowValidationExceptionOnModifyIfListenerEventV1IsNullAndLogItAsync()
        {
            // given
            ListenerEventV1 nullListenerEventV1 = null;

            var nullListenerEventV1ProcessingException =
                new NullListenerEventV1ProcessingException(message: "Listener event is null.");

            var expectedListenerEventV1ProcessingValidationException =
                new ListenerEventV1ProcessingValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventV1ProcessingException);

            // when
            ValueTask<ListenerEventV1> modifyListenerEventV1Task =
                this.listenerEventV1ProcessingService.ModifyListenerEventAsync(
                    nullListenerEventV1);

            ListenerEventV1ProcessingValidationException
                actualListenerEventV1ProcessingValidationException =
                    await Assert.ThrowsAsync<ListenerEventV1ProcessingValidationException>(
                        modifyListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1ProcessingValidationException.Should().BeEquivalentTo(
                expectedListenerEventV1ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ProcessingValidationException))),
                        Times.Once);

            this.listenerEventV1ServiceMock.Verify(broker =>
                broker.ModifyListenerEventAsync(
                    It.IsAny<ListenerEventV1>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
