// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Text.Json;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.VolatilePaths.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.VolatilePaths
{
    public partial class VolatilePathServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveVolatilePathsWhenJsonExceptionOccursAsync()
        {
            // given
            string someContent = GetRandomString();
            string[] somePaths = new[] { GetRandomString() };
            var jsonException = new JsonException();

            var failedJsonVolatilePathServiceException =
                new FailedJsonVolatilePathServiceException(
                    message: "Failed jsonvolatile path service error occurred, contact support.",
                    innerException: jsonException,
                    data: jsonException.Data);

            var expectedVolatilePathServiceDependencyValidationException =
                new VolatilePathServiceDependencyValidationException(
                    message: "Volatile path service dependency validation error occurred, fix the errors and try again.",
                    innerException: failedJsonVolatilePathServiceException);

            this.jsonBrokerMock
                .Setup(broker => broker.IsValidJson(someContent))
                .Throws(jsonException);

            // when
            ValueTask<string> removeVolatilePathsTask =
                this.volatilePathService.RemoveVolatilePathsAsync(
                    someContent,
                    somePaths);

            VolatilePathServiceDependencyValidationException actualException =
                await Assert.ThrowsAsync<VolatilePathServiceDependencyValidationException>(
                    removeVolatilePathsTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedVolatilePathServiceDependencyValidationException);

            this.jsonBrokerMock.Verify(
                broker => broker.IsValidJson(someContent),
                Times.Once);

            this.loggingBrokerMock.Verify(
                broker => broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedVolatilePathServiceDependencyValidationException))),
                Times.Once);

            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveVolatilePathsWhenExceptionOccursAsync()
        {
            // given
            string someContent = GetRandomString();
            string[] somePaths = new[] { GetRandomString() };
            var serviceException = new Exception();

            var failedVolatilePathServiceException =
                new FailedVolatilePathServiceException(
                    message: "Failed volatile path service error occurred, contact support.",
                    innerException: serviceException);

            var expectedVolatilePathServiceException =
                new VolatilePathServiceException(
                    message: "Volatile path service error occurred, contact support.",
                    innerException: failedVolatilePathServiceException);

            this.jsonBrokerMock
                .Setup(broker => broker.IsValidJson(someContent))
                .Throws(serviceException);

            // when
            ValueTask<string> removeVolatilePathsTask =
                this.volatilePathService.RemoveVolatilePathsAsync(
                    someContent,
                    somePaths);

            VolatilePathServiceException actualException =
                await Assert.ThrowsAsync<VolatilePathServiceException>(
                    removeVolatilePathsTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedVolatilePathServiceException);

            this.jsonBrokerMock.Verify(
                broker => broker.IsValidJson(someContent),
                Times.Once);

            this.loggingBrokerMock.Verify(
                broker => broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedVolatilePathServiceException))),
                Times.Once);

            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
