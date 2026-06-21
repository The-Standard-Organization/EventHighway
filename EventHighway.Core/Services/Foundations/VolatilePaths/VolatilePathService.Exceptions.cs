// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
