// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Configurations.Retries;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V2
{
    public partial class ListenerEventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldResetRetriesForListenerEventV2ByEventListenerV2IdOnErrorRowsOnlyAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid inputEventListenerV2Id = GetRandomId();
            DateTimeOffset baseDate = GetRandomDateTimeOffset();

            var batchConfiguration =
                new BatchConfiguration { BatchSizeForBulkProcessing = 2 };

            RetryConfiguration randomRetryConfiguration = CreateRandomRetryConfiguration();
            int delta = randomRetryConfiguration.RetryAttemptsAllowed;

            ListenerEventV2 errorListenerEventV2One = CreateRandomListenerEventV2();
            errorListenerEventV2One.EventListenerV2Id = inputEventListenerV2Id;
            errorListenerEventV2One.Status = ListenerEventStatusV2.Error;
            errorListenerEventV2One.RetryAttemptsAllowed = 5;
            errorListenerEventV2One.RemainingRetryAttempts = 0;
            errorListenerEventV2One.CreatedDate = baseDate;

            ListenerEventV2 errorListenerEventV2Two = CreateRandomListenerEventV2();
            errorListenerEventV2Two.EventListenerV2Id = inputEventListenerV2Id;
            errorListenerEventV2Two.Status = ListenerEventStatusV2.Error;
            errorListenerEventV2Two.RetryAttemptsAllowed = 7;
            errorListenerEventV2Two.RemainingRetryAttempts = 3;
            errorListenerEventV2Two.CreatedDate = baseDate.AddMinutes(1);

            ListenerEventV2 errorListenerEventV2Three = CreateRandomListenerEventV2();
            errorListenerEventV2Three.EventListenerV2Id = inputEventListenerV2Id;
            errorListenerEventV2Three.Status = ListenerEventStatusV2.Error;
            errorListenerEventV2Three.RetryAttemptsAllowed = 9;
            errorListenerEventV2Three.RemainingRetryAttempts = 4;
            errorListenerEventV2Three.CreatedDate = baseDate.AddMinutes(2);

            ListenerEventV2 successListenerEventV2 = CreateRandomListenerEventV2();
            successListenerEventV2.EventListenerV2Id = inputEventListenerV2Id;
            successListenerEventV2.Status = ListenerEventStatusV2.Success;

            IQueryable<ListenerEventV2> retrievedListenerEventV2s = new List<ListenerEventV2>
            {
                errorListenerEventV2One,
                successListenerEventV2,
                errorListenerEventV2Two,
                errorListenerEventV2Three
            }.AsQueryable();

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveListenerEventV2sByEventListenerV2IdAsync(
                    inputEventListenerV2Id,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedListenerEventV2s);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetRetryConfiguration())
                    .Returns(randomRetryConfiguration);

            this.listenerEventV2ServiceMock.Setup(service =>
                service.BulkModifyListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    randomCancellationToken))
                        .ReturnsAsync(new List<ListenerEventV2>());

            // when
            await this.listenerEventV2ProcessingService
                .ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    inputEventListenerV2Id, randomCancellationToken);

            // then
            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventV2sByEventListenerV2IdAsync(
                    inputEventListenerV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetRetryConfiguration(),
                    Times.Once);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.BulkModifyListenerEventV2sAsync(
                    It.Is<IEnumerable<ListenerEventV2>>(rows =>
                        rows.Count() == 2
                        && rows.Any(row =>
                            row.Id == errorListenerEventV2One.Id
                            && row.RetryAttemptsAllowed == 5 + delta
                            && row.RemainingRetryAttempts == 0 + delta
                            && row.NextRetryAttemptNotBefore == null)
                        && rows.Any(row =>
                            row.Id == errorListenerEventV2Two.Id
                            && row.RetryAttemptsAllowed == 7 + delta
                            && row.RemainingRetryAttempts == 3 + delta
                            && row.NextRetryAttemptNotBefore == null)),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.BulkModifyListenerEventV2sAsync(
                    It.Is<IEnumerable<ListenerEventV2>>(rows =>
                        rows.Count() == 1
                        && rows.Any(row =>
                            row.Id == errorListenerEventV2Three.Id
                            && row.RetryAttemptsAllowed == 9 + delta
                            && row.RemainingRetryAttempts == 4 + delta
                            && row.NextRetryAttemptNotBefore == null)),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldResetAllErrorRowsInSingleBatchWhenBatchSizeIsZeroAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid inputEventListenerV2Id = GetRandomId();
            DateTimeOffset baseDate = GetRandomDateTimeOffset();

            var batchConfiguration =
                new BatchConfiguration { BatchSizeForBulkProcessing = 0 };

            RetryConfiguration randomRetryConfiguration = CreateRandomRetryConfiguration();

            ListenerEventV2 errorListenerEventV2One = CreateRandomListenerEventV2();
            errorListenerEventV2One.EventListenerV2Id = inputEventListenerV2Id;
            errorListenerEventV2One.Status = ListenerEventStatusV2.Error;
            errorListenerEventV2One.CreatedDate = baseDate;

            ListenerEventV2 errorListenerEventV2Two = CreateRandomListenerEventV2();
            errorListenerEventV2Two.EventListenerV2Id = inputEventListenerV2Id;
            errorListenerEventV2Two.Status = ListenerEventStatusV2.Error;
            errorListenerEventV2Two.CreatedDate = baseDate.AddMinutes(1);

            ListenerEventV2 errorListenerEventV2Three = CreateRandomListenerEventV2();
            errorListenerEventV2Three.EventListenerV2Id = inputEventListenerV2Id;
            errorListenerEventV2Three.Status = ListenerEventStatusV2.Error;
            errorListenerEventV2Three.CreatedDate = baseDate.AddMinutes(2);

            IQueryable<ListenerEventV2> retrievedListenerEventV2s = new List<ListenerEventV2>
            {
                errorListenerEventV2One,
                errorListenerEventV2Two,
                errorListenerEventV2Three
            }.AsQueryable();

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveListenerEventV2sByEventListenerV2IdAsync(
                    inputEventListenerV2Id,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedListenerEventV2s);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetRetryConfiguration())
                    .Returns(randomRetryConfiguration);

            this.listenerEventV2ServiceMock.Setup(service =>
                service.BulkModifyListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    randomCancellationToken))
                        .ReturnsAsync(new List<ListenerEventV2>());

            // when
            await this.listenerEventV2ProcessingService
                .ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    inputEventListenerV2Id, randomCancellationToken);

            // then
            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventV2sByEventListenerV2IdAsync(
                    inputEventListenerV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetRetryConfiguration(),
                    Times.Once);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.BulkModifyListenerEventV2sAsync(
                    It.Is<IEnumerable<ListenerEventV2>>(rows =>
                        rows.Count() == 3
                        && rows.Any(row => row.Id == errorListenerEventV2One.Id)
                        && rows.Any(row => row.Id == errorListenerEventV2Two.Id)
                        && rows.Any(row => row.Id == errorListenerEventV2Three.Id)),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldPageBulkResetByEventListenerV2IdDeterministicallyByIdWhenCreatedDatesTieAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid inputEventListenerV2Id = GetRandomId();
            DateTimeOffset tiedCreatedDate = GetRandomDateTimeOffset();

            var batchConfiguration =
                new BatchConfiguration { BatchSizeForBulkProcessing = 2 };

            RetryConfiguration randomRetryConfiguration = CreateRandomRetryConfiguration();

            ListenerEventV2 errorListenerEventV2A = CreateRandomListenerEventV2();
            errorListenerEventV2A.Id = new Guid("00000000-0000-0000-0000-000000000001");
            errorListenerEventV2A.EventListenerV2Id = inputEventListenerV2Id;
            errorListenerEventV2A.Status = ListenerEventStatusV2.Error;
            errorListenerEventV2A.CreatedDate = tiedCreatedDate;

            ListenerEventV2 errorListenerEventV2B = CreateRandomListenerEventV2();
            errorListenerEventV2B.Id = new Guid("00000000-0000-0000-0000-000000000002");
            errorListenerEventV2B.EventListenerV2Id = inputEventListenerV2Id;
            errorListenerEventV2B.Status = ListenerEventStatusV2.Error;
            errorListenerEventV2B.CreatedDate = tiedCreatedDate;

            ListenerEventV2 errorListenerEventV2C = CreateRandomListenerEventV2();
            errorListenerEventV2C.Id = new Guid("00000000-0000-0000-0000-000000000003");
            errorListenerEventV2C.EventListenerV2Id = inputEventListenerV2Id;
            errorListenerEventV2C.Status = ListenerEventStatusV2.Error;
            errorListenerEventV2C.CreatedDate = tiedCreatedDate;

            // insertion order deliberately differs from Id order so that a stable
            // CreatedDate-only sort would page the tied rows non-deterministically
            IQueryable<ListenerEventV2> retrievedListenerEventV2s = new List<ListenerEventV2>
            {
                errorListenerEventV2C,
                errorListenerEventV2A,
                errorListenerEventV2B
            }.AsQueryable();

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveListenerEventV2sByEventListenerV2IdAsync(
                    inputEventListenerV2Id,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedListenerEventV2s);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetRetryConfiguration())
                    .Returns(randomRetryConfiguration);

            this.listenerEventV2ServiceMock.Setup(service =>
                service.BulkModifyListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    randomCancellationToken))
                        .ReturnsAsync(new List<ListenerEventV2>());

            // when
            await this.listenerEventV2ProcessingService
                .ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    inputEventListenerV2Id, randomCancellationToken);

            // then
            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventV2sByEventListenerV2IdAsync(
                    inputEventListenerV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetRetryConfiguration(),
                    Times.Once);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.BulkModifyListenerEventV2sAsync(
                    It.Is<IEnumerable<ListenerEventV2>>(rows =>
                        rows.Count() == 2
                        && rows.Any(row => row.Id == errorListenerEventV2A.Id)
                        && rows.Any(row => row.Id == errorListenerEventV2B.Id)),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.BulkModifyListenerEventV2sAsync(
                    It.Is<IEnumerable<ListenerEventV2>>(rows =>
                        rows.Count() == 1
                        && rows.Any(row => row.Id == errorListenerEventV2C.Id)),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
