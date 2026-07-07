// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Coordinations.HealthChecks.V2
{
    internal partial class HealthV2CoordinationService
    {
        private delegate ValueTask<HealthReportV2> ReturningHealthReportV2Function();

        private async ValueTask<HealthReportV2> TryCatch(
            ReturningHealthReportV2Function returningHealthReportV2Function)
        {
            try
            {
                return await returningHealthReportV2Function();
            }
            catch (InvalidHealthV2CoordinationException invalidHealthV2CoordinationException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidHealthV2CoordinationException);
            }
        }

        private async ValueTask<HealthV2CoordinationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var healthV2CoordinationValidationException =
                new HealthV2CoordinationValidationException(
                    message: "Health coordination validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(healthV2CoordinationValidationException);

            return healthV2CoordinationValidationException;
        }
    }
}
