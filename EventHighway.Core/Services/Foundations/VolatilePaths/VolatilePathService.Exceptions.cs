// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Text.Json;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.VolatilePaths.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.VolatilePaths
{
    internal partial class VolatilePathService
    {
        private delegate ValueTask<string> ReturningStringFunction();

        private async ValueTask<string> TryCatch(ReturningStringFunction returningStringFunction)
        {
            try
            {
                return await returningStringFunction();
            }
            catch (InvalidVolatilePathServiceException invalidVolatilePathServiceException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidVolatilePathServiceException);
            }
            catch (JsonException jsonException)
            {
                var failedJsonVolatilePathServiceException =
                    new FailedJsonVolatilePathServiceException(
                        message: "Failed jsonvolatile path service error occurred, contact support.",
                        innerException: jsonException);

                throw await CreateAndLogDependencyValidationExceptionAsync(failedJsonVolatilePathServiceException);
            }
        }

        private async ValueTask<VolatilePathServiceDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var volatilePathServiceDependencyValidationException =
                new VolatilePathServiceDependencyValidationException(
                    message: "Volatile path service dependency validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(volatilePathServiceDependencyValidationException);

            return volatilePathServiceDependencyValidationException;
        }

        private async ValueTask<VolatilePathServiceValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var volatilePathServiceValidationException =
                new VolatilePathServiceValidationException(
                    message: "Volatile path validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(volatilePathServiceValidationException);

            return volatilePathServiceValidationException;
        }
    }
}
