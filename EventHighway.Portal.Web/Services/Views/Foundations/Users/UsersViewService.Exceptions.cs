// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Users.Exceptions;
using Xeptions;

namespace EventHighway.Portal.Web.Services.Views.Foundations.Users
{
    public partial class UsersViewService
    {
        private delegate ValueTask<T> ReturningUsersFunction<T>();
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask<T> TryCatch<T>(ReturningUsersFunction<T> returningUsersFunction)
        {
            try
            {
                return await returningUsersFunction();
            }
            catch (LastAdministratorUsersViewException lastAdministratorUsersViewException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    lastAdministratorUsersViewException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedUsersViewServiceException(innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceException);
            }
        }

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (LastAdministratorUsersViewException lastAdministratorUsersViewException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    lastAdministratorUsersViewException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedUsersViewServiceException(innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceException);
            }
        }

        private async ValueTask<UsersViewValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var validationException =
                new UsersViewValidationException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(validationException);

            return validationException;
        }

        private async ValueTask<UsersViewServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var serviceException =
                new UsersViewServiceException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(serviceException);

            return serviceException;
        }
    }
}
