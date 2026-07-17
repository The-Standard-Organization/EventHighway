// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V2;
using EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V2
{
    public partial class EventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByQueryIfQueryIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2Query nullEventV2Query = null;

            var nullEventV2QueryCoordinationException =
                new NullEventV2QueryCoordinationException(
                    message: "Event query is null.");

            var expectedEventV2CoordinationValidationException =
                new EventV2CoordinationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: nullEventV2QueryCoordinationException);

            // when
            ValueTask<IQueryable<EventV2>> retrieveEventV2sByQueryTask =
                this.eventV2CoordinationService.RetrieveEventV2sByQueryAsync(
                    nullEventV2Query,
                    randomCancellationToken);

            EventV2CoordinationValidationException
                actualEventV2CoordinationValidationException =
                    await Assert.ThrowsAsync<EventV2CoordinationValidationException>(
                        retrieveEventV2sByQueryTask.AsTask);

            // then
            actualEventV2CoordinationValidationException.Should()
                .BeEquivalentTo(expectedEventV2CoordinationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2CoordinationValidationException))),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
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

            var invalidEventV2Query = new EventV2Query
            {
                Skip = -1,
                Take = invalidTake,
                CreatedFrom = randomDateTimeOffset,
                CreatedTo = randomDateTimeOffset.AddMinutes(-1),
                ScheduledFrom = randomDateTimeOffset,
                ScheduledTo = randomDateTimeOffset.AddMinutes(-1)
            };

            var invalidEventV2QueryCoordinationException =
                new InvalidEventV2QueryCoordinationException(
                    message: "Event query is invalid, fix the errors and try again.");

            invalidEventV2QueryCoordinationException.AddData(
                key: nameof(EventV2Query.Skip),
                values: "Value must be zero or greater");

            invalidEventV2QueryCoordinationException.AddData(
                key: nameof(EventV2Query.Take),
                values: "Value must be between 1 and 1000");

            invalidEventV2QueryCoordinationException.AddData(
                key: nameof(EventV2Query.CreatedTo),
                values: $"Date must be after {nameof(EventV2Query.CreatedFrom)}");

            invalidEventV2QueryCoordinationException.AddData(
                key: nameof(EventV2Query.ScheduledTo),
                values: $"Date must be after {nameof(EventV2Query.ScheduledFrom)}");

            var expectedEventV2CoordinationValidationException =
                new EventV2CoordinationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV2QueryCoordinationException);

            // when
            ValueTask<IQueryable<EventV2>> retrieveEventV2sByQueryTask =
                this.eventV2CoordinationService.RetrieveEventV2sByQueryAsync(
                    invalidEventV2Query,
                    randomCancellationToken);

            EventV2CoordinationValidationException
                actualEventV2CoordinationValidationException =
                    await Assert.ThrowsAsync<EventV2CoordinationValidationException>(
                        retrieveEventV2sByQueryTask.AsTask);

            // then
            actualEventV2CoordinationValidationException.Should()
                .BeEquivalentTo(expectedEventV2CoordinationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2CoordinationValidationException))),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
