// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using FluentAssertions;
using Moq;
using Xeptions;

#pragma warning disable CS0618 // Testing deprecated V1 exposure surface
namespace EventHighway.Core.Tests.Unit.Clients.Events.V1
{
    public partial class EventV1sClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnSubmitIfDependencyValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();

            var expectedEventV1ClientDependencyValidationException =
                new EventV1ClientDependencyValidationException(
                    message: "Event client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventV1CoordinationServiceMock.Setup(service =>
                service.SubmitEventV1Async(It.IsAny<EventV1>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventV1> submitEventV1Task =
                this.eventV1SClient.SubmitEventV1Async(someEventV1);

            EventV1ClientDependencyValidationException actualEventV1ClientDependencyValidationException =
                await Assert.ThrowsAsync<EventV1ClientDependencyValidationException>(
                    submitEventV1Task.AsTask);

            // then
            actualEventV1ClientDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1ClientDependencyValidationException);

            this.eventV1CoordinationServiceMock.Verify(service =>
                service.SubmitEventV1Async(It.IsAny<EventV1>()),
                    Times.Once);

            this.eventV1CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnSubmitIfDependencyErrorOccursAsync()
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var eventV1CoordinationDependencyException =
                new EventV1CoordinationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedEventV1ClientDependencyException =
                new EventV1ClientDependencyException(
                    message: "Event client dependency error occurred, contact support.",

                    innerException: eventV1CoordinationDependencyException
                        .InnerException as Xeption);

            this.eventV1CoordinationServiceMock.Setup(service =>
                service.SubmitEventV1Async(It.IsAny<EventV1>()))
                    .ThrowsAsync(eventV1CoordinationDependencyException);

            // when
            ValueTask<EventV1> submitEventV1Task =
                this.eventV1SClient.SubmitEventV1Async(someEventV1);

            EventV1ClientDependencyException actualEventV1ClientDependencyException =
                await Assert.ThrowsAsync<EventV1ClientDependencyException>(
                    submitEventV1Task.AsTask);

            // then
            actualEventV1ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventV1ClientDependencyException);

            this.eventV1CoordinationServiceMock.Verify(service =>
                service.SubmitEventV1Async(It.IsAny<EventV1>()),
                    Times.Once);

            this.eventV1CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnSubmitIfServiceErrorOccursAsync()
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var eventV1CoordinationServiceException =
                new EventV1CoordinationServiceException(
                    someMessage,
                    someInnerException);

            var expectedEventV1ClientServiceException =
                new EventV1ClientServiceException(
                    message: "Event client service error occurred, contact support.",

                    innerException: eventV1CoordinationServiceException
                        .InnerException as Xeption);

            this.eventV1CoordinationServiceMock.Setup(service =>
                service.SubmitEventV1Async(It.IsAny<EventV1>()))
                    .ThrowsAsync(eventV1CoordinationServiceException);

            // when
            ValueTask<EventV1> submitEventV1Task =
                this.eventV1SClient.SubmitEventV1Async(someEventV1);

            EventV1ClientServiceException actualEventV1ClientServiceException =
                await Assert.ThrowsAsync<EventV1ClientServiceException>(
                    submitEventV1Task.AsTask);

            // then
            actualEventV1ClientServiceException.Should()
                .BeEquivalentTo(expectedEventV1ClientServiceException);

            this.eventV1CoordinationServiceMock.Verify(service =>
                service.SubmitEventV1Async(It.IsAny<EventV1>()),
                    Times.Once);

            this.eventV1CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
