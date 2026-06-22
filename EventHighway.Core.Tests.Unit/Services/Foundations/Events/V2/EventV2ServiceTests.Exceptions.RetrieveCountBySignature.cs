// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V2
{
    public partial class EventV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveCountBySignatureWhenTimeoutOccursAndLogItAsync()
        {
            // given
            EventV2 someEventV2 =
                CreateRandomEventV2(dates: GetRandomDateTimeOffset());

            var loopDetectionConfig = new LoopDetection
            {
                Window = TimeSpan.FromSeconds(GetRandomNumber())
            };

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventV2Exception =
                new TimeoutEventV2Exception(
                    message: "Failed event timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventV2DependencyException =
                new EventV2DependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: timeoutEventV2Exception);

            this.configurationBrokerMock
                .Setup(broker => broker.GetLoopDetectionConfiguration())
                    .Returns(loopDetectionConfig);

            this.storageBrokerMock
                .Setup(broker => broker.SelectAllEventV2sAsync(
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<int> retrieveCountTask =
                this.eventV2Service.RetrieveEventV2CountBySignatureAsync(
                    someEventV2,
                    TestContext.Current.CancellationToken);

            EventV2DependencyException actualEventV2DependencyException =
                await Assert.ThrowsAsync<EventV2DependencyException>(
                    retrieveCountTask.AsTask);

            // then
            actualEventV2DependencyException.Should().BeEquivalentTo(
                expectedEventV2DependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2DependencyException))),
                        Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventV2sAsync(TestContext.Current.CancellationToken),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
