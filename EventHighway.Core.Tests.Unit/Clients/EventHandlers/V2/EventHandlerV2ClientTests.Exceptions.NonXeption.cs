// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Clients.EventHandlers.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventHandlers.V2
{
    public partial class EventHandlerV2ClientTests
    {
        [Fact]
        public async Task ShouldPreserveRootCauseOnRegisterIfUnexpectedNonXeptionErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEventHandler someEventHandler = CreateRandomEventHandler();
            var unexpectedException = new Exception(message: GetRandomString());

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RegisterEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(unexpectedException);

            // when
            ValueTask<IEventHandler> registerEventHandlerV2Task =
                this.eventHandlerV2Client.RegisterEventHandlerV2Async(
                    someEventHandler, randomCancellationToken);

            EventHandlerV2ClientServiceException actualEventHandlerV2ClientServiceException =
                await Assert.ThrowsAsync<EventHandlerV2ClientServiceException>(
                    registerEventHandlerV2Task.AsTask);

            // then
            actualEventHandlerV2ClientServiceException.InnerException
                .Should().NotBeNull();

            actualEventHandlerV2ClientServiceException.InnerException
                .Should().BeAssignableTo<Xeption>();

            actualEventHandlerV2ClientServiceException.InnerException.InnerException
                .Should().BeSameAs(unexpectedException);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RegisterEventHandlerV2Async(
                    It.IsAny<IEventHandler>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
