// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.VolatilePaths.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.VolatilePaths
{
    public partial class VolatilePathServiceTests
    {
        [Theory]
        [InlineData(null, new[] { "path" }, "content")]
        [InlineData("someContent", null, "volatileContentPaths")]
        public async Task ShouldThrowValidationExceptionOnRemoveVolatilePathsIfInvalidAsync(
            string invalidContent,
            string[] invalidPaths,
            string invalidParameter)
        {
            // given
            var invalidVolatilePathServiceException =
                new InvalidVolatilePathServiceException(
                    message: "Invalid volatile path service error occurred, fix the errors and try again.");

            invalidVolatilePathServiceException.UpsertDataList(
                key: invalidParameter,
                value: "Value is required");

            var expectedVolatilePathServiceValidationException =
                new VolatilePathServiceValidationException(
                    message: "Volatile path validation error occurred, fix the errors and try again.",
                    innerException: invalidVolatilePathServiceException);

            // when
            ValueTask<string> removeVolatilePathsTask =
                this.volatilePathService.RemoveVolatilePathsAsync(
                    invalidContent,
                    invalidPaths);

            VolatilePathServiceValidationException actualException =
                await Assert.ThrowsAsync<VolatilePathServiceValidationException>(
                    removeVolatilePathsTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedVolatilePathServiceValidationException);

            this.loggingBrokerMock.Verify(
                broker => broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedVolatilePathServiceValidationException))),
                Times.Once);

            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
