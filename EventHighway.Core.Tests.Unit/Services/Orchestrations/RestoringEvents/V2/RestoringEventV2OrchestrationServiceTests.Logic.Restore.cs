// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RestoringEvents.V2
{
    public partial class RestoringEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRestoreEventV2sAndListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventArchiveV2> inputEventArchiveV2s = CreateRandomEventArchiveV2s();

            List<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            List<EventV2> expectedEventV2sToRestore =
                inputEventArchiveV2s.Select(MapToEventV2).ToList();

            List<ListenerEventV2> expectedListenerEventV2sToRestore =
                inputListenerEventArchiveV2s.Select(MapToListenerEventV2).ToList();

            var inputSequence = new MockSequence();

            this.eventV2ProcessingServiceMock.InSequence(inputSequence).Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(new List<EventV2>().AsQueryable());

            this.eventV2ProcessingServiceMock.InSequence(inputSequence).Setup(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    randomCancellationToken))
                        .ReturnsAsync(expectedEventV2sToRestore);

            this.listenerEventV2ProcessingServiceMock.InSequence(inputSequence).Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(new List<ListenerEventV2>().AsQueryable());

            this.listenerEventV2ProcessingServiceMock.InSequence(inputSequence).Setup(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    randomCancellationToken))
                        .ReturnsAsync(expectedListenerEventV2sToRestore);

            // when
            await this.restoringEventV2OrchestrationService.RestoreAsync(
                inputEventArchiveV2s,
                inputListenerEventArchiveV2s,
                randomCancellationToken);

            // then
            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.BulkRestoreEventV2sAsync(
                    It.Is<List<EventV2>>(actual =>
                        SameEventV2sAs(expectedEventV2sToRestore, actual)),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.Is<List<ListenerEventV2>>(actual =>
                        SameGeneratedListenerEventV2sAs(expectedListenerEventV2sToRestore, actual)),
                    randomCancellationToken),
                        Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRestoreOnlyAbsentEventV2sAndListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventArchiveV2> inputEventArchiveV2s = CreateRandomEventArchiveV2s();

            List<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            List<EventV2> mappedEventV2s =
                inputEventArchiveV2s.Select(MapToEventV2).ToList();

            List<ListenerEventV2> mappedListenerEventV2s =
                inputListenerEventArchiveV2s.Select(MapToListenerEventV2).ToList();

            IQueryable<EventV2> existingEventV2s =
                new List<EventV2> { new EventV2 { Id = mappedEventV2s.First().Id } }
                    .AsQueryable();

            IQueryable<ListenerEventV2> existingListenerEventV2s =
                new List<ListenerEventV2>
                {
                    new ListenerEventV2
                    {
                        Id = GetRandomId(),
                        CorrelationId = mappedListenerEventV2s.First().CorrelationId
                    }
                }
                    .AsQueryable();

            List<EventV2> expectedEventV2sToRestore =
                mappedEventV2s.Skip(1).ToList();

            List<ListenerEventV2> expectedListenerEventV2sToRestore =
                mappedListenerEventV2s.Skip(1).ToList();

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(existingEventV2s);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    randomCancellationToken))
                        .ReturnsAsync(expectedEventV2sToRestore);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(existingListenerEventV2s);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    randomCancellationToken))
                        .ReturnsAsync(expectedListenerEventV2sToRestore);

            // when
            await this.restoringEventV2OrchestrationService.RestoreAsync(
                inputEventArchiveV2s,
                inputListenerEventArchiveV2s,
                randomCancellationToken);

            // then
            this.eventV2ProcessingServiceMock.Verify(service =>
                service.BulkRestoreEventV2sAsync(
                    It.Is<List<EventV2>>(actual =>
                        SameEventV2sAs(expectedEventV2sToRestore, actual)),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.Is<List<ListenerEventV2>>(actual =>
                        SameGeneratedListenerEventV2sAs(expectedListenerEventV2sToRestore, actual)),
                    randomCancellationToken),
                        Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldPreserveEventParticipantV2IdWhenRestoringEventV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventArchiveV2 inputEventArchiveV2 =
                CreateEventArchiveV2Filler().Create();

            List<EventArchiveV2> inputEventArchiveV2s =
                new List<EventArchiveV2> { inputEventArchiveV2 };

            List<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2>();

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(new List<EventV2>().AsQueryable());

            List<EventV2> capturedEventV2sToRestore = null;

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    randomCancellationToken))
                        .Callback<IEnumerable<EventV2>, CancellationToken>(
                            (actualEventV2s, _) =>
                                capturedEventV2sToRestore = actualEventV2s.ToList())
                        .ReturnsAsync(new List<EventV2>());

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(new List<ListenerEventV2>().AsQueryable());

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    randomCancellationToken))
                        .ReturnsAsync(new List<ListenerEventV2>());

            // when
            await this.restoringEventV2OrchestrationService.RestoreAsync(
                inputEventArchiveV2s,
                inputListenerEventArchiveV2s,
                randomCancellationToken);

            // then
            EventV2 actualEventV2 = capturedEventV2sToRestore.Single();

            actualEventV2.EventParticipantV2Id.Should()
                .Be(inputEventArchiveV2.EventParticipantV2Id);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    randomCancellationToken),
                        Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldResetRetryFieldsOnRestoreWhenListenerEventArchiveV2HasNonDefaultRetryStateAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventArchiveV2> inputEventArchiveV2s = new List<EventArchiveV2>();

            ListenerEventArchiveV2 inputListenerEventArchiveV2 =
                CreateListenerEventArchiveV2Filler().Create();

            inputListenerEventArchiveV2.CorrelationId = GetRandomId();
            inputListenerEventArchiveV2.RemainingRetryAttempts = 0;
            inputListenerEventArchiveV2.RetryAttemptsAllowed = 0;
            inputListenerEventArchiveV2.NextRetryAttemptNotBefore = GetRandomDateTimeOffset();
            inputListenerEventArchiveV2.DispatchedDate = GetRandomDateTimeOffset();

            List<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2> { inputListenerEventArchiveV2 };

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(new List<EventV2>().AsQueryable());

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    randomCancellationToken))
                        .ReturnsAsync(new List<EventV2>());

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(new List<ListenerEventV2>().AsQueryable());

            List<ListenerEventV2> capturedListenerEventV2sToRestore = null;

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    randomCancellationToken))
                        .Callback<IEnumerable<ListenerEventV2>, CancellationToken>(
                            (actualListenerEventV2s, _) =>
                                capturedListenerEventV2sToRestore = actualListenerEventV2s.ToList())
                        .ReturnsAsync(new List<ListenerEventV2>());

            // when
            await this.restoringEventV2OrchestrationService.RestoreAsync(
                inputEventArchiveV2s,
                inputListenerEventArchiveV2s,
                randomCancellationToken);

            // then
            ListenerEventV2 actualListenerEventV2 =
                capturedListenerEventV2sToRestore.Single();

            actualListenerEventV2.Status.Should().Be(ListenerEventStatusV2.Replay);
            actualListenerEventV2.CorrelationId.Should().Be(inputListenerEventArchiveV2.Id);
            actualListenerEventV2.RemainingRetryAttempts.Should().Be(this.retryConfiguration.RetryAttemptsAllowed);
            actualListenerEventV2.RetryAttemptsAllowed.Should().Be(this.retryConfiguration.RetryAttemptsAllowed);
            actualListenerEventV2.NextRetryAttemptNotBefore.Should().BeNull();
            actualListenerEventV2.DispatchedDate.Should().BeNull();

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    randomCancellationToken),
                        Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
