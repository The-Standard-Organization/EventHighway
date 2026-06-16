// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventAddresses.V2
{
    public partial class EventAddressV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRegisterIfEventAddressV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventAddressV2 nullEventAddressV2 = null;

            var nullEventAddressV2ProcessingException =
                new NullEventAddressV2ProcessingException(
                    message: "Event address is null.");

            var expectedEventAddressV2ProcessingValidationException =
                new EventAddressV2ProcessingValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: nullEventAddressV2ProcessingException);

            // when
            ValueTask<EventAddressV2> registerEventAddressV2Task =
                this.eventAddressV2ProcessingService.RegisterEventAddressV2Async(
                    nullEventAddressV2,
                    randomCancellationToken);

            EventAddressV2ProcessingValidationException
                actualEventAddressV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventAddressV2ProcessingValidationException>(
                        registerEventAddressV2Task.AsTask);

            // then
            actualEventAddressV2ProcessingValidationException.Should().BeEquivalentTo(
                expectedEventAddressV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2ProcessingValidationException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.AddEventAddressV2Async(
                    It.IsAny<EventAddressV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
