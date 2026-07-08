// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Views.UserEventParticipants.Exceptions;
using Xeptions;

namespace EventHighway.Portal.Web.Services.Views.UserEventParticipants
{
    public partial class UserEventParticipantsViewService
    {
        private delegate ValueTask<T> ReturningUserEventParticipantFunction<T>();
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask<T> TryCatch<T>(
            ReturningUserEventParticipantFunction<T> returningUserEventParticipantFunction)
        {
            try
            {
                return await returningUserEventParticipantFunction();
            }
            catch (NotFoundUserEventParticipantsViewException
                notFoundUserEventParticipantsViewException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    notFoundUserEventParticipantsViewException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedUserEventParticipantsViewServiceException(
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceException);
            }
        }

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (NotFoundUserEventParticipantsViewException
                notFoundUserEventParticipantsViewException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    notFoundUserEventParticipantsViewException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedUserEventParticipantsViewServiceException(
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceException);
            }
        }

        private async ValueTask<UserEventParticipantsViewValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var validationException =
                new UserEventParticipantsViewValidationException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(validationException);

            return validationException;
        }

        private async ValueTask<UserEventParticipantsViewServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var serviceException =
                new UserEventParticipantsViewServiceException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(serviceException);

            return serviceException;
        }
    }
}
