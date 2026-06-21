// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.Json;
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
        public async Task
            ShouldThrowDependencyValidationExceptionOnRemoveVolatilePathsWhenJsonExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 randomEventV2 =
                CreateRandomEventV2(dates: GetRandomDateTimeOffset());

            EventV2 inputEventV2 = randomEventV2;
            string someJsonContent = GetRandomString();
            inputEventV2.Content = someJsonContent;

            string[] someVolatileContentPaths =
                new[] { GetRandomString() };

            var loopDetectionConfiguration = new LoopDetection
            {
                VolatilePaths = new List<VolatilePaths>
                {
                    new VolatilePaths
                    {
                        EventAddressId = inputEventV2.EventAddressId,
                        VolatileContentPaths = someVolatileContentPaths
                    }
                }
            };

            var jsonException = new JsonException();
            jsonException.Data.Add("ErrorCode", new List<string> { "JsonError" });

            var failedJsonEventV2Exception =
                new FailedJsonEventV2Exception(
                    message: "Failed json event error occurred, contact support.",
                    innerException: jsonException,
                    data: jsonException.Data);

            var expectedEventV2DependencyValidationException =
                new EventV2DependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: failedJsonEventV2Exception);

            this.configurationBrokerMock
                .Setup(broker => broker.GetLoopDetectionConfiguration())
                .Returns(loopDetectionConfiguration);

            this.jsonBrokerMock
                .Setup(broker => broker.IsValidJson(someJsonContent))
                .Throws(jsonException);

            // when
            ValueTask<string> removeVolatilePathsTask =
                this.eventV2Service.RemoveVolatilePathsAsync(
                    inputEventV2,
                    randomCancellationToken);

            EventV2DependencyValidationException actualEventV2DependencyValidationException =
                await Assert.ThrowsAsync<EventV2DependencyValidationException>(
                    removeVolatilePathsTask.AsTask);

            // then
            actualEventV2DependencyValidationException.Should().BeEquivalentTo(
                expectedEventV2DependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2DependencyValidationException))),
                        Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.jsonBrokerMock.Verify(broker =>
                broker.IsValidJson(someJsonContent),
                    Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveVolatilePathsWhenExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 randomEventV2 =
                CreateRandomEventV2(dates: GetRandomDateTimeOffset());

            EventV2 inputEventV2 = randomEventV2;
            string someJsonContent = GetRandomString();
            inputEventV2.Content = someJsonContent;

            string[] someVolatileContentPaths =
                new[] { GetRandomString() };

            var loopDetectionConfiguration = new LoopDetection
            {
                VolatilePaths = new List<VolatilePaths>
                {
                    new VolatilePaths
                    {
                        EventAddressId = inputEventV2.EventAddressId,
                        VolatileContentPaths = someVolatileContentPaths
                    }
                }
            };

            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventV2ServiceException =
                new FailedEventV2ServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventV2ServiceException =
                new EventV2ServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV2ServiceException);

            this.configurationBrokerMock
                .Setup(broker => broker.GetLoopDetectionConfiguration())
                .Returns(loopDetectionConfiguration);

            this.jsonBrokerMock
                .Setup(broker => broker.IsValidJson(someJsonContent))
                .Throws(serviceException);

            // when
            ValueTask<string> removeVolatilePathsTask =
                this.eventV2Service.RemoveVolatilePathsAsync(
                    inputEventV2,
                    randomCancellationToken);

            EventV2ServiceException actualEventV2ServiceException =
                await Assert.ThrowsAsync<EventV2ServiceException>(
                    removeVolatilePathsTask.AsTask);

            // then
            actualEventV2ServiceException.Should().BeEquivalentTo(
                expectedEventV2ServiceException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ServiceException))),
                        Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.jsonBrokerMock.Verify(broker =>
                broker.IsValidJson(someJsonContent),
                    Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
