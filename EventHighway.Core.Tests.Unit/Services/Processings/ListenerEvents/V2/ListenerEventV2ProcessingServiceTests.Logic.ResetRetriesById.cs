// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.Retries;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V2
{
    public partial class ListenerEventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldResetRetriesForListenerEventV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid inputListenerEventV2Id = GetRandomId();
            DateTimeOffset randomNow = GetRandomDateTimeOffset();

            RetryConfiguration randomRetryConfiguration = CreateRandomRetryConfiguration();
            int delta = randomRetryConfiguration.RetryAttemptsAllowed;

            ListenerEventV2 storageListenerEventV2 = CreateRandomListenerEventV2();
            storageListenerEventV2.Id = inputListenerEventV2Id;
            storageListenerEventV2.Status = ListenerEventStatusV2.Error;

            int originalRetryAttemptsAllowed = storageListenerEventV2.RetryAttemptsAllowed;
            int originalRemainingRetryAttempts = storageListenerEventV2.RemainingRetryAttempts;
            DateTimeOffset originalCreatedDate = storageListenerEventV2.CreatedDate;
            DateTimeOffset? originalDispatchedDate = storageListenerEventV2.DispatchedDate;

            ListenerEventV2 returnedListenerEventV2 = storageListenerEventV2.DeepClone();

            var mockSequence = new MockSequence();

            this.listenerEventV2ServiceMock.InSequence(mockSequence).Setup(service =>
                service.RetrieveListenerEventV2ByIdAsync(
                    inputListenerEventV2Id,
                    randomCancellationToken))
                        .ReturnsAsync(storageListenerEventV2);

            this.configurationBrokerMock.InSequence(mockSequence).Setup(broker =>
                broker.GetRetryConfiguration())
                    .Returns(randomRetryConfiguration);

            this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomNow);

            this.listenerEventV2ServiceMock.InSequence(mockSequence).Setup(service =>
                service.ModifyListenerEventV2Async(
                    It.Is<ListenerEventV2>(lev =>
                        lev.Id == inputListenerEventV2Id
                        && lev.Status == ListenerEventStatusV2.Error
                        && lev.RetryAttemptsAllowed == originalRetryAttemptsAllowed + delta
                        && lev.RemainingRetryAttempts == originalRemainingRetryAttempts + delta
                        && lev.NextRetryAttemptNotBefore == null
                        && lev.UpdatedDate == randomNow
                        && lev.CreatedDate == originalCreatedDate
                        && lev.DispatchedDate == originalDispatchedDate),
                    randomCancellationToken))
                        .ReturnsAsync(returnedListenerEventV2);

            // when
            ListenerEventV2 actualListenerEventV2 =
                await this.listenerEventV2ProcessingService
                    .ResetRetriesForListenerEventV2ByIdAsync(
                        inputListenerEventV2Id, randomCancellationToken);

            // then
            actualListenerEventV2.Should().BeEquivalentTo(returnedListenerEventV2);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventV2ByIdAsync(
                    inputListenerEventV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetRetryConfiguration(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
