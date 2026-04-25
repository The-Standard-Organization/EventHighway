// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventAddresses.V1
{
    public partial class EventAddressV1ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidEventAddressV1Id = Guid.Empty;

            var invalidEventAddressV1ProcessingException =
                new InvalidEventAddressV1ProcessingException(
                    message: "Event address is invalid, fix the errors and try again.");

            invalidEventAddressV1ProcessingException.AddData(
                key: nameof(EventAddressV1.Id),
                values: "Required");

            var expectedEventAddressV1ProcessingValidationException =
                new EventAddressV1ProcessingValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: invalidEventAddressV1ProcessingException);

            // when
            ValueTask<EventAddressV1> retrieveEventAddressV1ByIdTask =
                this.eventAddressV1ProcessingService.RetrieveEventAddressV1ByIdAsync(
                    invalidEventAddressV1Id);

            EventAddressV1ProcessingValidationException
                actualEventAddressV1ProcessingValidationException =
                    await Assert.ThrowsAsync<EventAddressV1ProcessingValidationException>(
                        retrieveEventAddressV1ByIdTask.AsTask);

            // then
            actualEventAddressV1ProcessingValidationException.Should()
                .BeEquivalentTo(expectedEventAddressV1ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1ProcessingValidationException))),
                        Times.Once);

            this.eventAddressV1ServiceMock.Verify(broker =>
                broker.RetrieveEventAddressByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventAddressV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
