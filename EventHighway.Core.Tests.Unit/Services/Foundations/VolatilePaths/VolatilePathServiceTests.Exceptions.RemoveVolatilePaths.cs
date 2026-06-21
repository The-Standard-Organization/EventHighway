// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
                    innerException: jsonException);

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
    }
}
