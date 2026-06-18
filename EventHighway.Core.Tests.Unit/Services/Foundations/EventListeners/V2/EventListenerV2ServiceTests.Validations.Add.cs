// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListeners.V2
{
    public partial class EventListenerV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventListenerV2IsNullAndLogItAsync()
        {
            // given
            EventListenerV2 nullEventListenerV2 = null;

            var nullEventListenerV2Exception =
                new NullEventListenerV2Exception(message: "Event listener is null.");

            var expectedEventListenerV2ValidationException =
                new EventListenerV2ValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: nullEventListenerV2Exception);

            // when
            ValueTask<EventListenerV2> addEventListenerV2Task =
                this.eventListenerV2Service.AddEventListenerV2Async(
                    nullEventListenerV2,
                    TestContext.Current.CancellationToken);

            EventListenerV2ValidationException actualEventListenerV2ValidationException =
                await Assert.ThrowsAsync<EventListenerV2ValidationException>(
                    addEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ValidationException.Should().BeEquivalentTo(
                expectedEventListenerV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV2Async(
                    It.IsAny<EventListenerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnAddIfEventListenerV2IsInvalidAndLogItAsync(
            string invalidText)
        {
            var invalidEventListenerV2 = new EventListenerV2
            {
                Id = Guid.Empty,
                Name = invalidText,
                Description = invalidText,
                HandlerName = invalidText,
                HandlerConfigurations = null,
                EventAddressId = Guid.Empty
            };

            var invalidEventListenerV2Exception =
                new InvalidEventListenerV2Exception(
                    message: "Event listener is invalid, fix the errors and try again.");

            invalidEventListenerV2Exception.AddData(
                key: nameof(EventListenerV2.Id),
                values: "Required");

            invalidEventListenerV2Exception.AddData(
                key: nameof(EventListenerV2.Name),
                values: "Required");

            invalidEventListenerV2Exception.AddData(
                key: nameof(EventListenerV2.Description),
                values: "Required");

            invalidEventListenerV2Exception.AddData(
                key: nameof(EventListenerV2.HandlerId),
                values: "Required");

            invalidEventListenerV2Exception.AddData(
                key: nameof(EventListenerV2.HandlerName),
                values: "Required");

            invalidEventListenerV2Exception.AddData(
                key: nameof(EventListenerV2.HandlerConfigurations),
                values: "Required");

            invalidEventListenerV2Exception.AddData(
                key: nameof(EventListenerV2.EventAddressId),
                values: "Required");

            invalidEventListenerV2Exception.AddData(
                key: nameof(EventListenerV2.CreatedDate),
                values: "Required");

            invalidEventListenerV2Exception.AddData(
                key: nameof(EventListenerV2.UpdatedDate),
                values: "Required");

            var expectedEventListenerV2ValidationException =
                new EventListenerV2ValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV2Exception);

            // when
            ValueTask<EventListenerV2> addEventListenerV2Task =
                this.eventListenerV2Service.AddEventListenerV2Async(
                    invalidEventListenerV2,
                    TestContext.Current.CancellationToken);

            EventListenerV2ValidationException actualEventListenerV2ValidationException =
                await Assert.ThrowsAsync<EventListenerV2ValidationException>(
                    addEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ValidationException.Should().BeEquivalentTo(
                expectedEventListenerV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV2Async(
                    It.IsAny<EventListenerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset anotherRandomDateTimeOffset = GetRandomDateTimeOffset();
            EventListenerV2 randomEventListenerV2 = CreateRandomEventListenerV2(dates: randomDateTimeOffset);
            EventListenerV2 invalidEventListenerV2 = randomEventListenerV2;
            invalidEventListenerV2.UpdatedDate = anotherRandomDateTimeOffset;

            var invalidEventListenerV2Exception =
                new InvalidEventListenerV2Exception(
                    message: "Event listener is invalid, fix the errors and try again.");

            invalidEventListenerV2Exception.AddData(
                key: nameof(EventListenerV2.CreatedDate),
                values: $"Date is not the same as {nameof(EventListenerV2.UpdatedDate)}");

            var expectedEventListenerV2ValidationException =
                new EventListenerV2ValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventListenerV2> addEventListenerV2Task =
                this.eventListenerV2Service.AddEventListenerV2Async(
                    invalidEventListenerV2,
                    TestContext.Current.CancellationToken);

            EventListenerV2ValidationException actualEventListenerV2ValidationException =
                await Assert.ThrowsAsync<EventListenerV2ValidationException>(
                    addEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ValidationException.Should().BeEquivalentTo(
                expectedEventListenerV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV2Async(
                    It.IsAny<EventListenerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventListenerV2NameExceedsMaxLengthAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventListenerV2 randomEventListenerV2 = CreateRandomEventListenerV2(randomDateTimeOffset);
            EventListenerV2 invalidEventListenerV2 = randomEventListenerV2;
            invalidEventListenerV2.Name = GetRandomStringWithLengthOf(451);

            var invalidEventListenerV2Exception =
                new InvalidEventListenerV2Exception(
                    message: "Event listener is invalid, fix the errors and try again.");

            invalidEventListenerV2Exception.AddData(
                key: nameof(EventListenerV2.Name),
                values: $"Text exceed max length of {invalidEventListenerV2.Name.Length - 1} characters");

            var expectedEventListenerV2ValidationException =
                new EventListenerV2ValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventListenerV2> addEventListenerV2Task =
                this.eventListenerV2Service.AddEventListenerV2Async(
                    invalidEventListenerV2,
                    TestContext.Current.CancellationToken);

            EventListenerV2ValidationException actualEventListenerV2ValidationException =
                await Assert.ThrowsAsync<EventListenerV2ValidationException>(
                    addEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ValidationException.Should().BeEquivalentTo(
                expectedEventListenerV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV2Async(
                    It.IsAny<EventListenerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeAndAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            EventListenerV2 randomEventListenerV2 =
                CreateRandomEventListenerV2(randomDateTimeOffset
                    .AddSeconds(minutesBeforeAndAfter));

            EventListenerV2 invalidEventListenerV2 = randomEventListenerV2;

            var invalidEventListenerV2Exception =
                new InvalidEventListenerV2Exception(
                    message: "Event listener is invalid, fix the errors and try again.");

            invalidEventListenerV2Exception.AddData(
                key: nameof(EventListenerV2.CreatedDate),
                values: "Date is not recent");

            var expectedEventListenerV2ValidationException =
                new EventListenerV2ValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventListenerV2> addEventListenerV2Task =
                this.eventListenerV2Service.AddEventListenerV2Async(
                    invalidEventListenerV2,
                    TestContext.Current.CancellationToken);

            EventListenerV2ValidationException actualEventListenerV2ValidationException =
                await Assert.ThrowsAsync<EventListenerV2ValidationException>(
                    addEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ValidationException.Should().BeEquivalentTo(
                expectedEventListenerV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV2Async(
                    It.IsAny<EventListenerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnAddIfHandlerConfigurationIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventListenerV2 randomEventListenerV2 = CreateRandomEventListenerV2(randomDateTimeOffset);
            EventListenerV2 invalidEventListenerV2 = randomEventListenerV2;

            invalidEventListenerV2.HandlerConfigurations = new[]
            {
                new HandlerConfiguration
                {
                    Id = Guid.Empty,
                    Name = invalidText,
                    Value = invalidText,
                    CreatedDate = default,
                    UpdatedDate = default
                }
            };

            var invalidEventListenerV2Exception =
                new InvalidEventListenerV2Exception(
                    message: "Event listener  handler configuration is invalid, fix the errors and try again.");

            invalidEventListenerV2Exception.AddData(
                key: "HandlerConfiguration[0].Id",
                values: "Required");

            invalidEventListenerV2Exception.AddData(
                key: "HandlerConfiguration[0].Name",
                values: "Required");

            invalidEventListenerV2Exception.AddData(
                key: "HandlerConfiguration[0].Value",
                values: "Required");

            invalidEventListenerV2Exception.AddData(
                key: "HandlerConfiguration[0].CreatedDate",
                values: new[] { "Required", "Date is not recent" });

            invalidEventListenerV2Exception.AddData(
                key: "HandlerConfiguration[0].UpdatedDate",
                values: "Required");

            var expectedEventListenerV2ValidationException =
                new EventListenerV2ValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventListenerV2> addEventListenerV2Task =
                this.eventListenerV2Service.AddEventListenerV2Async(
                    invalidEventListenerV2,
                    TestContext.Current.CancellationToken);

            EventListenerV2ValidationException actualEventListenerV2ValidationException =
                await Assert.ThrowsAsync<EventListenerV2ValidationException>(
                    addEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ValidationException.Should().BeEquivalentTo(
                expectedEventListenerV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(2));

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV2Async(
                    It.IsAny<EventListenerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfHandlerConfigurationNameExceedsMaxLengthAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventListenerV2 randomEventListenerV2 = CreateRandomEventListenerV2(randomDateTimeOffset);
            EventListenerV2 invalidEventListenerV2 = randomEventListenerV2;
            HandlerConfiguration randomHandlerConfiguration = CreateRandomHandlerConfiguration(randomDateTimeOffset);
            randomHandlerConfiguration.Name = GetRandomStringWithLengthOf(451);

            invalidEventListenerV2.HandlerConfigurations = new[] { randomHandlerConfiguration };

            var invalidEventListenerV2Exception =
                new InvalidEventListenerV2Exception(
                    message: "Event listener  handler configuration is invalid, fix the errors and try again.");

            invalidEventListenerV2Exception.AddData(
                key: "HandlerConfiguration[0].Name",
                values: $"Text exceed max length of {randomHandlerConfiguration.Name.Length - 1} characters");

            var expectedEventListenerV2ValidationException =
                new EventListenerV2ValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventListenerV2> addEventListenerV2Task =
                this.eventListenerV2Service.AddEventListenerV2Async(
                    invalidEventListenerV2,
                    TestContext.Current.CancellationToken);

            EventListenerV2ValidationException actualEventListenerV2ValidationException =
                await Assert.ThrowsAsync<EventListenerV2ValidationException>(
                    addEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ValidationException.Should().BeEquivalentTo(
                expectedEventListenerV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(2));

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV2Async(
                    It.IsAny<EventListenerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnAddIfHandlerConfigurationCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset anotherRandomDateTimeOffset = GetRandomDateTimeOffset();
            EventListenerV2 randomEventListenerV2 = CreateRandomEventListenerV2(randomDateTimeOffset);
            EventListenerV2 invalidEventListenerV2 = randomEventListenerV2;
            HandlerConfiguration randomHandlerConfiguration = CreateRandomHandlerConfiguration(randomDateTimeOffset);
            randomHandlerConfiguration.UpdatedDate = anotherRandomDateTimeOffset;

            invalidEventListenerV2.HandlerConfigurations = new[] { randomHandlerConfiguration };

            var invalidEventListenerV2Exception =
                new InvalidEventListenerV2Exception(
                    message: "Event listener  handler configuration is invalid, fix the errors and try again.");

            invalidEventListenerV2Exception.AddData(
                key: "HandlerConfiguration[0].CreatedDate",
                values: $"Date is not the same as {nameof(HandlerConfiguration.UpdatedDate)}");

            var expectedEventListenerV2ValidationException =
                new EventListenerV2ValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventListenerV2> addEventListenerV2Task =
                this.eventListenerV2Service.AddEventListenerV2Async(
                    invalidEventListenerV2,
                    TestContext.Current.CancellationToken);

            EventListenerV2ValidationException actualEventListenerV2ValidationException =
                await Assert.ThrowsAsync<EventListenerV2ValidationException>(
                    addEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ValidationException.Should().BeEquivalentTo(
                expectedEventListenerV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(2));

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV2Async(
                    It.IsAny<EventListenerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task
            ShouldThrowValidationExceptionOnAddIfHandlerConfigurationCreatedDateIsNotRecentAndLogItAsync(
                int minutesBeforeAndAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventListenerV2 randomEventListenerV2 = CreateRandomEventListenerV2(randomDateTimeOffset);
            EventListenerV2 invalidEventListenerV2 = randomEventListenerV2;

            HandlerConfiguration randomHandlerConfiguration =
                CreateRandomHandlerConfiguration(
                    randomDateTimeOffset.AddSeconds(minutesBeforeAndAfter));

            invalidEventListenerV2.HandlerConfigurations = new[] { randomHandlerConfiguration };

            var invalidEventListenerV2Exception =
                new InvalidEventListenerV2Exception(
                    message: "Event listener  handler configuration is invalid, fix the errors and try again.");

            invalidEventListenerV2Exception.AddData(
                key: "HandlerConfiguration[0].CreatedDate",
                values: "Date is not recent");

            var expectedEventListenerV2ValidationException =
                new EventListenerV2ValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventListenerV2> addEventListenerV2Task =
                this.eventListenerV2Service.AddEventListenerV2Async(
                    invalidEventListenerV2,
                    TestContext.Current.CancellationToken);

            EventListenerV2ValidationException actualEventListenerV2ValidationException =
                await Assert.ThrowsAsync<EventListenerV2ValidationException>(
                    addEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ValidationException.Should().BeEquivalentTo(
                expectedEventListenerV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(2));

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV2Async(
                    It.IsAny<EventListenerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
