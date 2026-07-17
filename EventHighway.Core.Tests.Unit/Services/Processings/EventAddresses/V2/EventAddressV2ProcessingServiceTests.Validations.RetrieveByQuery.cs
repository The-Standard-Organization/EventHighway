// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
            ValueTask<IQueryable<EventAddressV2>> retrieveEventAddressV2sByQueryTask =
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
    }
}
