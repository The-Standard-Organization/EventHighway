// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V2
{
    public partial class ListenerEventV2ServiceTests
    {
        [Fact]
        public async Task ShouldBulkRestoreListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<ListenerEventV2> randomListenerEventV2s =
                CreateRandomRestoreListenerEventV2s();

            List<ListenerEventV2> inputListenerEventV2s = randomListenerEventV2s;

            List<ListenerEventV2> expectedListenerEventV2s =
                inputListenerEventV2s.Select(item => item.DeepClone()).ToList();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(DateTimeOffset.MaxValue);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkInsertListenerEventV2sAsync(
                    It.Is<List<ListenerEventV2>>(actual =>
                        SameListenerEventV2sAs(expectedListenerEventV2s, actual)),
                            randomCancellationToken))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2Service.BulkRestoreListenerEventV2sAsync(
                    inputListenerEventV2s,
                        randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkInsertListenerEventV2sAsync(
                    It.Is<List<ListenerEventV2>>(actual =>
                        SameListenerEventV2sAs(expectedListenerEventV2s, actual)),
                            randomCancellationToken),
                                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldBulkRestoreValidListenerEventV2sAndLogInvalidOnesAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<ListenerEventV2> validListenerEventV2s =
                CreateRandomRestoreListenerEventV2s();

            var invalidListenerEventV2 = new ListenerEventV2
            {
                Id = Guid.Empty,
                EventV2Id = Guid.Empty,
                EventAddressV2Id = Guid.Empty,
                EventListenerV2Id = Guid.Empty,
                Status = GetInvalidEnum<ListenerEventStatusV2>()
            };

            List<ListenerEventV2> inputListenerEventV2s =
                validListenerEventV2s.Append(invalidListenerEventV2).ToList();

            List<ListenerEventV2> expectedListenerEventV2s =
                validListenerEventV2s.Select(item => item.DeepClone()).ToList();

            var invalidListenerEventV2Exception =
                new InvalidListenerEventV2Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.Id),
                values: "Required");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.EventV2Id),
                values: "Required");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.EventAddressV2Id),
                values: "Required");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.EventListenerV2Id),
                values: "Required");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.EventParticipantV2Id),
                values: "Required");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.Status),
                values: "Value is not recognized");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.CreatedDate),
                values: "Required");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.UpdatedDate),
                values: "Required");

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(DateTimeOffset.MaxValue);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkInsertListenerEventV2sAsync(
                    It.Is<List<ListenerEventV2>>(actual =>
                        SameListenerEventV2sAs(expectedListenerEventV2s, actual)),
                            randomCancellationToken))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2Service.BulkRestoreListenerEventV2sAsync(
                    inputListenerEventV2s,
                        randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    invalidListenerEventV2Exception))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkInsertListenerEventV2sAsync(
                    It.Is<List<ListenerEventV2>>(actual =>
                        SameListenerEventV2sAs(expectedListenerEventV2s, actual)),
                            randomCancellationToken),
                                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldBulkRestoreValidListenerEventV2sAndLogReverseDatedOnesAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<ListenerEventV2> validListenerEventV2s =
                CreateRandomRestoreListenerEventV2s();

            ListenerEventV2 reverseDatedListenerEventV2 =
                CreateRandomRestoreListenerEventV2s().First();

            reverseDatedListenerEventV2.UpdatedDate =
                reverseDatedListenerEventV2.CreatedDate.AddMinutes(-GetRandomNumber());

            List<ListenerEventV2> inputListenerEventV2s =
                validListenerEventV2s.Append(reverseDatedListenerEventV2).ToList();

            List<ListenerEventV2> expectedListenerEventV2s =
                validListenerEventV2s.Select(item => item.DeepClone()).ToList();

            var invalidListenerEventV2Exception =
                new InvalidListenerEventV2Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.CreatedDate),
                values: $"Date is later than {nameof(ListenerEventV2.UpdatedDate)}");

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(DateTimeOffset.MaxValue);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkInsertListenerEventV2sAsync(
                    It.Is<List<ListenerEventV2>>(actual =>
                        SameListenerEventV2sAs(expectedListenerEventV2s, actual)),
                            randomCancellationToken))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2Service.BulkRestoreListenerEventV2sAsync(
                    inputListenerEventV2s,
                        randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    invalidListenerEventV2Exception))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkInsertListenerEventV2sAsync(
                    It.Is<List<ListenerEventV2>>(actual =>
                        SameListenerEventV2sAs(expectedListenerEventV2s, actual)),
                            randomCancellationToken),
                                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldBulkRestoreValidListenerEventV2sAndLogFutureDatedOnesAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset now = GetRandomDateTimeOffset();

            List<ListenerEventV2> validListenerEventV2s =
                CreateRandomRestoreListenerEventV2s();

            foreach (ListenerEventV2 validListenerEventV2 in validListenerEventV2s)
            {
                validListenerEventV2.CreatedDate = now.AddMinutes(-GetRandomNumber() - 2);
                validListenerEventV2.UpdatedDate = now.AddMinutes(-1);
            }

            ListenerEventV2 futureDatedListenerEventV2 =
                CreateRandomRestoreListenerEventV2s().First();

            futureDatedListenerEventV2.CreatedDate = now.AddMinutes(-1);
            futureDatedListenerEventV2.UpdatedDate = now.AddMinutes(GetRandomNumber());

            List<ListenerEventV2> inputListenerEventV2s =
                validListenerEventV2s.Append(futureDatedListenerEventV2).ToList();

            List<ListenerEventV2> expectedListenerEventV2s =
                validListenerEventV2s.Select(item => item.DeepClone()).ToList();

            var invalidListenerEventV2Exception =
                new InvalidListenerEventV2Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.UpdatedDate),
                values: "Date is in the future");

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkInsertListenerEventV2sAsync(
                    It.Is<List<ListenerEventV2>>(actual =>
                        SameListenerEventV2sAs(expectedListenerEventV2s, actual)),
                            randomCancellationToken))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2Service.BulkRestoreListenerEventV2sAsync(
                    inputListenerEventV2s,
                        randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    invalidListenerEventV2Exception))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkInsertListenerEventV2sAsync(
                    It.Is<List<ListenerEventV2>>(actual =>
                        SameListenerEventV2sAs(expectedListenerEventV2s, actual)),
                            randomCancellationToken),
                                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
