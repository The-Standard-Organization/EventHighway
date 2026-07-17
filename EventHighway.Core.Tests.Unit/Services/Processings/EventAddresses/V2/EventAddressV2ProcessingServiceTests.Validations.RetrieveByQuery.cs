// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventAddresses.V2
{
    public partial class EventAddressV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByQueryIfQueryIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventAddressV2Query nullEventAddressV2Query = null;

            var nullEventAddressV2QueryProcessingException =
                new NullEventAddressV2QueryProcessingException(
                    message: "Event address query is null.");

            var expectedEventAddressV2ProcessingValidationException =
                new EventAddressV2ProcessingValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: nullEventAddressV2QueryProcessingException);

            // when
            ValueTask<IReadOnlyList<EventAddressV2>> retrieveEventAddressV2sByQueryTask =
                this.eventAddressV2ProcessingService.RetrieveEventAddressV2sByQueryAsync(
                    nullEventAddressV2Query,
                    randomCancellationToken);

            EventAddressV2ProcessingValidationException
                actualEventAddressV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventAddressV2ProcessingValidationException>(
                        retrieveEventAddressV2sByQueryTask.AsTask);

            // then
            actualEventAddressV2ProcessingValidationException.Should()
                .BeEquivalentTo(expectedEventAddressV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2ProcessingValidationException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1001)]
        public async Task ShouldThrowValidationExceptionOnRetrieveByQueryIfQueryIsInvalidAndLogItAsync(
            int invalidTake)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            var invalidEventAddressV2Query = new EventAddressV2Query
            {
                Skip = -1,
                Take = invalidTake,
                CreatedFrom = randomDateTimeOffset,
                CreatedTo = randomDateTimeOffset.AddMinutes(-1)
            };

            var invalidEventAddressV2QueryProcessingException =
                new InvalidEventAddressV2QueryProcessingException(
                    message: "Event address query is invalid, fix the errors and try again.");

            invalidEventAddressV2QueryProcessingException.AddData(
                key: nameof(EventAddressV2Query.Skip),
                values: "Value must be zero or greater");

            invalidEventAddressV2QueryProcessingException.AddData(
                key: nameof(EventAddressV2Query.Take),
                values: "Value must be between 1 and 1000");

            invalidEventAddressV2QueryProcessingException.AddData(
                key: nameof(EventAddressV2Query.CreatedTo),
                values: $"Date must be after {nameof(EventAddressV2Query.CreatedFrom)}");

            var expectedEventAddressV2ProcessingValidationException =
                new EventAddressV2ProcessingValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: invalidEventAddressV2QueryProcessingException);

            // when
            ValueTask<IReadOnlyList<EventAddressV2>> retrieveEventAddressV2sByQueryTask =
                this.eventAddressV2ProcessingService.RetrieveEventAddressV2sByQueryAsync(
                    invalidEventAddressV2Query,
                    randomCancellationToken);

            EventAddressV2ProcessingValidationException
                actualEventAddressV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventAddressV2ProcessingValidationException>(
                        retrieveEventAddressV2sByQueryTask.AsTask);

            // then
            actualEventAddressV2ProcessingValidationException.Should()
                .BeEquivalentTo(expectedEventAddressV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2ProcessingValidationException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
