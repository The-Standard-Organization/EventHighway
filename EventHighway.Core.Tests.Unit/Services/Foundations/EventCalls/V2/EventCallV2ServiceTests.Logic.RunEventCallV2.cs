// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Configurations.Dispatch;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.PromotedProperties;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventCalls.V2
{
    public partial class EventCallV2ServiceTests
    {
        [Fact]
        public async Task ShouldTimeOutEventCallV2WhenHandlerExceedsConfiguredTimeoutAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventCallV2 randomEventCallV2 = CreateRandomEventCallV2();
            EventCallV2 inputEventCallV2 = randomEventCallV2;

            var dispatchConfiguration = new DispatchConfiguration
            {
                HandlerTimeout = TimeSpan.Zero
            };

            var neverCompletingTaskSource =
                new TaskCompletionSource<EventHandlerResult>();

            this.configurationBrokerMock.Setup(broker =>
                broker.GetDispatchConfiguration())
                    .Returns(dispatchConfiguration);

            this.eventHandlerBrokerMock.Setup(broker => broker.GetAll())
                .Returns(new[] { this.eventHandlerMock.Object });

            this.eventHandlerMock.SetupGet(handler => handler.Id)
                .Returns(inputEventCallV2.HandlerId);

            this.eventHandlerMock
                .Setup(handler =>
                    handler.HandleAsync(
                        inputEventCallV2.Content,
                        It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<EventHandlerResult>(neverCompletingTaskSource.Task));

            // when
            EventCallV2 actualEventCallV2 =
                await this.eventCallV2Service
                    .RunEventCallV2Async(inputEventCallV2, randomCancellationToken);

            // then
            actualEventCallV2.IsSuccess.Should().BeFalse();
            actualEventCallV2.ResponseCode.Should().Be("HandlerTimeout");
            actualEventCallV2.Response.Should().Be(actualEventCallV2.ResponseMessage);

            this.eventHandlerBrokerMock.Verify(broker => broker.GetAll(),
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();

            this.eventHandlerMock.Verify(handler =>
                handler.HandleAsync(
                    inputEventCallV2.Content,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRunEventCallV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string randomHandlerName = GetRandomString();

            EventCallV2 randomEventCallV2 = CreateRandomEventCallV2();
            randomEventCallV2.HandlerName = randomHandlerName;

            EventCallV2 inputEventCallV2 = randomEventCallV2;

            EventHandlerResult randomEventHandlerResult =
                CreateRandomEventHandlerResult();

            EventHandlerResult returnedEventHandlerResult =
                randomEventHandlerResult;

            EventCallV2 expectedEventCallV2 =
                inputEventCallV2.DeepClone();

            expectedEventCallV2.IsSuccess =
                returnedEventHandlerResult.IsSuccess;

            expectedEventCallV2.Response =
                returnedEventHandlerResult.Response;

            expectedEventCallV2.ResponseCode =
                returnedEventHandlerResult.ResponseCode;

            expectedEventCallV2.ResponseMessage =
                returnedEventHandlerResult.ResponseMessage;

            this.eventHandlerBrokerMock.Setup(broker => broker.GetAll())
                .Returns(new[] { this.eventHandlerMock.Object });

            this.eventHandlerMock.SetupGet(handler => handler.Id)
                .Returns(inputEventCallV2.HandlerId);

            this.eventHandlerMock
                .Setup(handler =>
                    handler.HandleAsync(
                        inputEventCallV2.Content,
                        randomCancellationToken))
                .ReturnsAsync(returnedEventHandlerResult);

            // when
            EventCallV2 actualEventCallV2 =
                await this.eventCallV2Service
                    .RunEventCallV2Async(inputEventCallV2, randomCancellationToken);

            // then
            actualEventCallV2.Should().BeEquivalentTo(expectedEventCallV2);

            this.eventHandlerBrokerMock.Verify(broker => broker.GetAll(),
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();

            this.eventHandlerMock.VerifyGet(handler => handler.Id,
                Times.AtLeastOnce);

            this.eventHandlerMock.Verify(handler =>
                handler.HandleAsync(
                    inputEventCallV2.Content,
                    randomCancellationToken),
                Times.Once);

            this.eventHandlerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldSkipEventCallV2WhenFilterCriteriaDoesNotMatchAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string randomHandlerName = GetRandomString();
            string promotedPropertyName = "OrderType";
            string promotedPropertyValue = "Standard";

            EventCallV2 randomEventCallV2 = CreateRandomEventCallV2();
            randomEventCallV2.HandlerName = randomHandlerName;
            randomEventCallV2.FilterCriteria = $"meta(\"{promotedPropertyName}\") == \"Express\"";
            randomEventCallV2.RequiredPromotedProperties = new[] { promotedPropertyName };

            randomEventCallV2.PromotedProperties =
                new List<PromotedProperty>
                {
                    new PromotedProperty
                    {
                        Name = promotedPropertyName,
                        Value = promotedPropertyValue
                    }
                };

            EventCallV2 inputEventCallV2 = randomEventCallV2;

            EventCallV2 expectedEventCallV2 = inputEventCallV2.DeepClone();
            expectedEventCallV2.IsSuccess = true;
            expectedEventCallV2.ResponseCode = "SkippedNotMatchingFilter";
            expectedEventCallV2.ResponseMessage =
                "Event was not handled because it did not match the listener's filter criteria.";

            expectedEventCallV2.Response = expectedEventCallV2.ResponseMessage;

            this.eventHandlerBrokerMock.Setup(broker => broker.GetAll())
                .Returns(new[] { this.eventHandlerMock.Object });

            this.eventHandlerMock.SetupGet(handler => handler.Id)
                .Returns(inputEventCallV2.HandlerId);

            this.eventHandlerMock
                .Setup(handler =>
                    handler.HandleAsync(
                        It.IsAny<string>(),
                        randomCancellationToken))
                .ReturnsAsync(CreateRandomEventHandlerResult());

            // when
            EventCallV2 actualEventCallV2 =
                await this.eventCallV2Service
                    .RunEventCallV2Async(inputEventCallV2, randomCancellationToken);

            // then
            actualEventCallV2.Should().BeEquivalentTo(expectedEventCallV2);

            this.eventHandlerBrokerMock.Verify(broker => broker.GetAll(),
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();

            this.eventHandlerMock.VerifyGet(handler => handler.Id,
                Times.AtLeastOnce);

            this.eventHandlerMock.Verify(handler =>
                handler.HandleAsync(
                    It.IsAny<string>(),
                    randomCancellationToken),
                Times.Never);

            this.eventHandlerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnBadFilterCriteriaWhenFilterExpressionIsInvalidAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string randomHandlerName = GetRandomString();

            EventCallV2 randomEventCallV2 = CreateRandomEventCallV2();
            randomEventCallV2.HandlerName = randomHandlerName;
            randomEventCallV2.FilterCriteria = "this is not a valid expression !!!";
            randomEventCallV2.RequiredPromotedProperties = System.Array.Empty<string>();
            randomEventCallV2.PromotedProperties = new List<PromotedProperty>();

            EventCallV2 inputEventCallV2 = randomEventCallV2;

            EventCallV2 expectedEventCallV2 = inputEventCallV2.DeepClone();
            expectedEventCallV2.IsSuccess = false;
            expectedEventCallV2.ResponseCode = "BadFilterCriteria";
            expectedEventCallV2.ResponseMessage =
                "The filter criteria expression is invalid. " +
                "Check that the expression is correctly formatted " +
                "and uses a valid Dynamic Expresso expression syntax. " +
                "See the Dynamic Expresso documentation for more details - " +
                "https://github.com/dynamicexpresso/DynamicExpresso";

            expectedEventCallV2.Response = expectedEventCallV2.ResponseMessage;

            this.eventHandlerBrokerMock.Setup(broker => broker.GetAll())
                .Returns(new[] { this.eventHandlerMock.Object });

            this.eventHandlerMock.SetupGet(handler => handler.Id)
                .Returns(inputEventCallV2.HandlerId);

            this.eventHandlerMock
                .Setup(handler =>
                    handler.HandleAsync(
                        It.IsAny<string>(),
                        randomCancellationToken))
                .ReturnsAsync(CreateRandomEventHandlerResult());

            // when
            EventCallV2 actualEventCallV2 =
                await this.eventCallV2Service
                    .RunEventCallV2Async(inputEventCallV2, randomCancellationToken);

            // then
            actualEventCallV2.Should().BeEquivalentTo(expectedEventCallV2,
                options => options.Including(c => c.IsSuccess)
                                  .Including(c => c.ResponseCode)
                                  .Including(c => c.ResponseMessage));

            this.eventHandlerBrokerMock.Verify(broker => broker.GetAll(),
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();

            this.eventHandlerMock.VerifyGet(handler => handler.Id,
                Times.AtLeastOnce);

            this.eventHandlerMock.Verify(handler =>
                handler.HandleAsync(
                    It.IsAny<string>(),
                    randomCancellationToken),
                Times.Never);

            this.eventHandlerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnMissingRequiredMetadataWhenPromotedPropertyIsAbsentAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string randomHandlerName = GetRandomString();
            string requiredPropertyName = "OrderType";

            EventCallV2 randomEventCallV2 = CreateRandomEventCallV2();
            randomEventCallV2.HandlerName = randomHandlerName;
            randomEventCallV2.FilterCriteria = null;
            randomEventCallV2.RequiredPromotedProperties = new[] { requiredPropertyName };
            randomEventCallV2.PromotedProperties = new List<PromotedProperty>();

            EventCallV2 inputEventCallV2 = randomEventCallV2;

            EventCallV2 expectedEventCallV2 = inputEventCallV2.DeepClone();
            expectedEventCallV2.IsSuccess = false;
            expectedEventCallV2.ResponseCode = "MissingRequiredMetadata";
            expectedEventCallV2.ResponseMessage =
                "One or more promoted properties could not be extracted from the event content. " +
                "Check that the event listener is correctly configured. " +
                "Promoted properties must match property names in the event content. (Case Sensitive)";

            expectedEventCallV2.Response = expectedEventCallV2.ResponseMessage;

            this.eventHandlerBrokerMock.Setup(broker => broker.GetAll())
                .Returns(new[] { this.eventHandlerMock.Object });

            this.eventHandlerMock.SetupGet(handler => handler.Id)
                .Returns(inputEventCallV2.HandlerId);

            this.eventHandlerMock
                .Setup(handler =>
                    handler.HandleAsync(
                        It.IsAny<string>(),
                        randomCancellationToken))
                .ReturnsAsync(CreateRandomEventHandlerResult());

            // when
            EventCallV2 actualEventCallV2 =
                await this.eventCallV2Service
                    .RunEventCallV2Async(inputEventCallV2, randomCancellationToken);

            // then
            actualEventCallV2.Should().BeEquivalentTo(expectedEventCallV2,
                options => options.Including(c => c.IsSuccess)
                                  .Including(c => c.ResponseCode)
                                  .Including(c => c.ResponseMessage));

            this.eventHandlerBrokerMock.Verify(broker => broker.GetAll(),
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();

            this.eventHandlerMock.VerifyGet(handler => handler.Id,
                Times.AtLeastOnce);

            this.eventHandlerMock.Verify(handler =>
                handler.HandleAsync(
                    It.IsAny<string>(),
                    randomCancellationToken),
                Times.Never);

            this.eventHandlerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
