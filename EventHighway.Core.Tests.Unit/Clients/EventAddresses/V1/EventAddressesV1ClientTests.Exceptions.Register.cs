// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventAddresses.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

#pragma warning disable CS0618 // Testing deprecated V1 exposure surface
namespace EventHighway.Core.Tests.Unit.Clients.EventAddresses.V1
{
    public partial class EventAddressesV1ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRegisterIfDependencyValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            EventAddressV1 someEventAddressV1 = CreateRandomEventAddressV1();

            var expectedEventAddressV1ClientDependencyValidationException =
                new EventAddressV1ClientDependencyValidationException(
                    message: "Event address client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventAddressV1ServiceMock.Setup(service =>
                service.AddEventAddressV1Async(It.IsAny<EventAddressV1>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventAddressV1> registerEventAddressV1Task =
                this.eventAddressV1sClient.RegisterEventAddressV1Async(someEventAddressV1);

            EventAddressV1ClientDependencyValidationException
                actualEventAddressV1ClientDependencyValidationException =
                    await Assert.ThrowsAsync<EventAddressV1ClientDependencyValidationException>(
                        registerEventAddressV1Task.AsTask);

            // then
            actualEventAddressV1ClientDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventAddressV1ClientDependencyValidationException);

            this.eventAddressV1ServiceMock.Verify(service =>
                service.AddEventAddressV1Async(It.IsAny<EventAddressV1>()),
                    Times.Once);

            this.eventAddressV1ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRegisterIfDependencyErrorOccursAsync()
        {
            // given
            EventAddressV1 someEventAddressV1 = CreateRandomEventAddressV1();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var eventV1DependencyException =
                new EventAddressV1DependencyException(
                    someMessage,
                    someInnerException);

            var expectedEventAddressV1ClientDependencyException =
                new EventAddressV1ClientDependencyException(
                    message: "Event address client dependency error occurred, contact support.",
                    innerException: eventV1DependencyException.InnerException as Xeption);

            this.eventAddressV1ServiceMock.Setup(service =>
                service.AddEventAddressV1Async(It.IsAny<EventAddressV1>()))
                    .ThrowsAsync(eventV1DependencyException);

            // when
            ValueTask<EventAddressV1> registerEventAddressV1Task =
                this.eventAddressV1sClient.RegisterEventAddressV1Async(someEventAddressV1);

            EventAddressV1ClientDependencyException
                actualEventAddressV1ClientDependencyException =
                    await Assert.ThrowsAsync<EventAddressV1ClientDependencyException>(
                        registerEventAddressV1Task.AsTask);

            // then
            actualEventAddressV1ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV1ClientDependencyException);

            this.eventAddressV1ServiceMock.Verify(service =>
                service.AddEventAddressV1Async(It.IsAny<EventAddressV1>()),
                    Times.Once);

            this.eventAddressV1ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRegisterIfServiceErrorOccursAsync()
        {
            // given
            EventAddressV1 someEventAddressV1 = CreateRandomEventAddressV1();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var eventV1ServiceException =
                new EventAddressV1ServiceException(
                    someMessage,
                    someInnerException);

            var expectedEventAddressV1ClientServiceException =
                new EventAddressV1ClientServiceException(
                    message: "Event address client service error occurred, contact support.",
                    innerException: eventV1ServiceException.InnerException as Xeption);

            this.eventAddressV1ServiceMock.Setup(service =>
                service.AddEventAddressV1Async(It.IsAny<EventAddressV1>()))
                    .ThrowsAsync(eventV1ServiceException);

            // when
            ValueTask<EventAddressV1> registerEventAddressV1Task =
                this.eventAddressV1sClient.RegisterEventAddressV1Async(someEventAddressV1);

            EventAddressV1ClientServiceException actualEventAddressV1ClientServiceException =
                await Assert.ThrowsAsync<EventAddressV1ClientServiceException>(
                    registerEventAddressV1Task.AsTask);

            // then
            actualEventAddressV1ClientServiceException.Should()
                .BeEquivalentTo(expectedEventAddressV1ClientServiceException);

            this.eventAddressV1ServiceMock.Verify(service =>
                service.AddEventAddressV1Async(It.IsAny<EventAddressV1>()),
                    Times.Once);

            this.eventAddressV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
