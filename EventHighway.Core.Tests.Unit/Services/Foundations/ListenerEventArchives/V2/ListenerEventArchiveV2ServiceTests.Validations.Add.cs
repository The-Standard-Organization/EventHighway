// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfListenerEventArchiveV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventArchiveV2 nullListenerEventArchiveV2 = null;

            var nullListenerEventArchiveV2Exception =
                new NullListenerEventArchiveV2Exception(
                    message: "Listener event archive is null.");

            var expectedListenerEventArchiveV2ValidationException =
                new ListenerEventArchiveV2ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventArchiveV2Exception);

            // when
            ValueTask<ListenerEventArchiveV2> addListenerEventArchiveV2Task =
                this.listenerEventArchiveV2Service.AddListenerEventArchiveV2Async(
                    nullListenerEventArchiveV2,
                    randomCancellationToken);

            ListenerEventArchiveV2ValidationException actualListenerEventArchiveV2ValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ValidationException>(
                    addListenerEventArchiveV2Task.AsTask);

            // then
            actualListenerEventArchiveV2ValidationException.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfListenerEventArchiveV2IsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventArchiveStatusV2 invalidStatus =
                GetInvalidEnum<ListenerEventArchiveStatusV2>();

            var invalidListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = Guid.Empty,
                Status = invalidStatus,
                EventV2Id = Guid.Empty,
                EventAddressV2Id = Guid.Empty,
                EventListenerV2Id = Guid.Empty
            };

            var invalidListenerEventArchiveV2Exception =
                new InvalidListenerEventArchiveV2Exception(
                    message: "Listener event archive is invalid, fix the errors and try again.");

            invalidListenerEventArchiveV2Exception.AddData(
                key: nameof(ListenerEventArchiveV2.Id),
                values: "Required");

            invalidListenerEventArchiveV2Exception.AddData(
                key: nameof(ListenerEventArchiveV2.EventV2Id),
                values: "Required");

            invalidListenerEventArchiveV2Exception.AddData(
                key: nameof(ListenerEventArchiveV2.EventAddressV2Id),
                values: "Required");

            invalidListenerEventArchiveV2Exception.AddData(
                key: nameof(ListenerEventArchiveV2.EventListenerV2Id),
                values: "Required");

            invalidListenerEventArchiveV2Exception.AddData(
                key: nameof(ListenerEventArchiveV2.EventParticipantV2Id),
                values: "Required");

            invalidListenerEventArchiveV2Exception.AddData(
                key: nameof(ListenerEventArchiveV2.Status),
                values: "Value is not recognized");

            invalidListenerEventArchiveV2Exception.AddData(
                key: nameof(ListenerEventArchiveV2.CreatedDate),
                values: "Required");

            invalidListenerEventArchiveV2Exception.AddData(
                key: nameof(ListenerEventArchiveV2.UpdatedDate),
                values: "Required");

            invalidListenerEventArchiveV2Exception.AddData(
                key: nameof(ListenerEventArchiveV2.ArchivedDate),
                values: "Required");

            var expectedListenerEventArchiveV2ValidationException =
                new ListenerEventArchiveV2ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventArchiveV2Exception);

            // when
            ValueTask<ListenerEventArchiveV2> addListenerEventArchiveV2Task =
                this.listenerEventArchiveV2Service.AddListenerEventArchiveV2Async(
                    invalidListenerEventArchiveV2,
                    randomCancellationToken);

            ListenerEventArchiveV2ValidationException actualListenerEventArchiveV2ValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ValidationException>(
                    addListenerEventArchiveV2Task.AsTask);

            // then
            actualListenerEventArchiveV2ValidationException.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnAddIfArchivedDateIsNotRecentAndLogItAsync(
            int secondsBeforeAndAfterNow)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            ListenerEventArchiveV2 randomListenerEventArchiveV2 =
                CreateRandomListenerEventArchiveV2(
                    date: randomDateTimeOffset.AddSeconds(secondsBeforeAndAfterNow));

            ListenerEventArchiveV2 invalidListenerEventArchiveV2 = randomListenerEventArchiveV2;

            var invalidListenerEventArchiveV2Exception =
                new InvalidListenerEventArchiveV2Exception(
                    message: "Listener event archive is invalid, fix the errors and try again.");

            invalidListenerEventArchiveV2Exception.AddData(
                key: nameof(ListenerEventArchiveV2.ArchivedDate),
                values: "Date is not recent");

            var expectedListenerEventArchiveV2ValidationException =
                new ListenerEventArchiveV2ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventArchiveV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ListenerEventArchiveV2> addListenerEventArchiveV2Task =
                this.listenerEventArchiveV2Service.AddListenerEventArchiveV2Async(
                    invalidListenerEventArchiveV2,
                    randomCancellationToken);

            ListenerEventArchiveV2ValidationException actualListenerEventArchiveV2ValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ValidationException>(
                    addListenerEventArchiveV2Task.AsTask);

            // then
            actualListenerEventArchiveV2ValidationException.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventParticipantV2IdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            ListenerEventArchiveV2 randomListenerEventArchiveV2 =
                CreateRandomListenerEventArchiveV2(date: randomDateTimeOffset);

            ListenerEventArchiveV2 invalidListenerEventArchiveV2 = randomListenerEventArchiveV2;
            invalidListenerEventArchiveV2.EventParticipantV2Id = Guid.Empty;

            var invalidListenerEventArchiveV2Exception =
                new InvalidListenerEventArchiveV2Exception(
                    message: "Listener event archive is invalid, fix the errors and try again.");

            invalidListenerEventArchiveV2Exception.AddData(
                key: nameof(ListenerEventArchiveV2.EventParticipantV2Id),
                values: "Required");

            var expectedListenerEventArchiveV2ValidationException =
                new ListenerEventArchiveV2ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventArchiveV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ListenerEventArchiveV2> addListenerEventArchiveV2Task =
                this.listenerEventArchiveV2Service.AddListenerEventArchiveV2Async(
                    invalidListenerEventArchiveV2,
                    randomCancellationToken);

            ListenerEventArchiveV2ValidationException actualListenerEventArchiveV2ValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ValidationException>(
                    addListenerEventArchiveV2Task.AsTask);

            // then
            actualListenerEventArchiveV2ValidationException.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
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
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset anotherRandomDateTimeOffset = GetRandomDateTimeOffset();

            ListenerEventArchiveV2 randomListenerEventArchiveV2 =
                CreateRandomListenerEventArchiveV2(randomDateTimeOffset);

            ListenerEventArchiveV2 invalidListenerEventArchiveV2 = randomListenerEventArchiveV2;
            invalidListenerEventArchiveV2.UpdatedDate = anotherRandomDateTimeOffset;

            var invalidListenerEventArchiveV2Exception =
                new InvalidListenerEventArchiveV2Exception(
                    message: "Listener event archive is invalid, fix the errors and try again.");

            invalidListenerEventArchiveV2Exception.AddData(
                key: nameof(ListenerEventArchiveV2.CreatedDate),
                values: $"Date is not the same as {nameof(ListenerEventArchiveV2.UpdatedDate)}");

            var expectedListenerEventArchiveV2ValidationException =
                new ListenerEventArchiveV2ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventArchiveV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ListenerEventArchiveV2> addListenerEventArchiveV2Task =
                this.listenerEventArchiveV2Service.AddListenerEventArchiveV2Async(
                    invalidListenerEventArchiveV2,
                    randomCancellationToken);

            ListenerEventArchiveV2ValidationException actualListenerEventArchiveV2ValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ValidationException>(
                    addListenerEventArchiveV2Task.AsTask);

            // then
            actualListenerEventArchiveV2ValidationException.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
