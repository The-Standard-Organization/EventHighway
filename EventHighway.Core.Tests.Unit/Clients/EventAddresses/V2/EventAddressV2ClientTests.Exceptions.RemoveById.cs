// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventAddresses.V2
{
    public partial class EventAddressV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressV2Id = GetRandomId();

            var expectedEventAddressV2ClientValidationException =
                new EventAddressV2ClientValidationException(
                    message: "Event address client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RemoveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventAddressV2> removeEventAddressV2ByIdTask =
                this.eventAddressV2Client.RemoveEventAddressV2ByIdAsync(
                    someEventAddressV2Id,
                    randomCancellationToken);

            EventAddressV2ClientValidationException actualEventAddressV2ClientValidationException =
                await Assert.ThrowsAsync<EventAddressV2ClientValidationException>(
                    removeEventAddressV2ByIdTask.AsTask);

            // then
            actualEventAddressV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventAddressV2ClientValidationException);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressV2Id = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventAddressV2ProcessingDependencyException =
                new EventAddressV2ProcessingDependencyException(
                    someMessage,
                    someInnerException);

            var expectedEventAddressV2ClientDependencyException =
                new EventAddressV2ClientDependencyException(
                    message: "Event address client dependency error occurred, contact support.",
                    innerException: eventAddressV2ProcessingDependencyException.InnerException as Xeption,
                    data: (eventAddressV2ProcessingDependencyException.InnerException as Xeption).Data);

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RemoveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventAddressV2ProcessingDependencyException);

            // when
            ValueTask<EventAddressV2> removeEventAddressV2ByIdTask =
                this.eventAddressV2Client.RemoveEventAddressV2ByIdAsync(
                    someEventAddressV2Id,
                    randomCancellationToken);

            EventAddressV2ClientDependencyException actualEventAddressV2ClientDependencyException =
                await Assert.ThrowsAsync<EventAddressV2ClientDependencyException>(
                    removeEventAddressV2ByIdTask.AsTask);

            // then
            actualEventAddressV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV2ClientDependencyException);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressV2Id = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventAddressV2ProcessingServiceException =
                new EventAddressV2ProcessingServiceException(
                    someMessage,
                    someInnerException);

            var expectedEventAddressV2ClientDependencyException =
                new EventAddressV2ClientDependencyException(
                    message: "Event address client dependency error occurred, contact support.",
                    innerException: eventAddressV2ProcessingServiceException.InnerException as Xeption,
                    data: (eventAddressV2ProcessingServiceException.InnerException as Xeption).Data);

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RemoveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventAddressV2ProcessingServiceException);

            // when
            ValueTask<EventAddressV2> removeEventAddressV2ByIdTask =
                this.eventAddressV2Client.RemoveEventAddressV2ByIdAsync(
                    someEventAddressV2Id,
                    randomCancellationToken);

            EventAddressV2ClientDependencyException actualEventAddressV2ClientDependencyException =
                await Assert.ThrowsAsync<EventAddressV2ClientDependencyException>(
                    removeEventAddressV2ByIdTask.AsTask);

            // then
            actualEventAddressV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV2ClientDependencyException);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
