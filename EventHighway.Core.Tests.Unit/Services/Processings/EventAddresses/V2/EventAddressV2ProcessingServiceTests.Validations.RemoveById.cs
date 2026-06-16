// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidEventAddressV2Id = Guid.Empty;

            var invalidEventAddressV2ProcessingException =
                new InvalidEventAddressV2ProcessingException(
                    message: "Event address is invalid, fix the errors and try again.");

            invalidEventAddressV2ProcessingException.AddData(
                key: nameof(EventAddressV2.Id),
                values: "Required");

            var expectedEventAddressV2ProcessingValidationException =
                new EventAddressV2ProcessingValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: invalidEventAddressV2ProcessingException);

            // when
            ValueTask<EventAddressV2> removeEventAddressV2ByIdTask =
                this.eventAddressV2ProcessingService
                    .RemoveEventAddressV2ByIdAsync(
                        invalidEventAddressV2Id,
                        randomCancellationToken);

            EventAddressV2ProcessingValidationException
                actualEventAddressV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventAddressV2ProcessingValidationException>(
                        removeEventAddressV2ByIdTask.AsTask);

            // then
            actualEventAddressV2ProcessingValidationException.Should().BeEquivalentTo(
                expectedEventAddressV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2ProcessingValidationException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RemoveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
