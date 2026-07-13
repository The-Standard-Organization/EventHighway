// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ListenerEventArchives.V2.Exceptions;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEventArchives.Exceptions;
using Xeptions;

namespace EventHighway.Portal.Web.Services.Views.Foundations.ListenerEventArchives
{
    public partial class ListenerEventArchivesViewService
    {
        private delegate ValueTask<T> ReturningListenerEventArchivesFunction<T>();

        private async ValueTask<T> TryCatch<T>(
            ReturningListenerEventArchivesFunction<T> returningListenerEventArchivesFunction)
        {
            try
            {
                return await returningListenerEventArchivesFunction();
            }
            catch (ListenerEventArchiveV2ClientValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    clientValidationException);
            }
            catch (ListenerEventArchiveV2ClientDependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (ListenerEventArchiveV2ClientServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedListenerEventArchivesViewServiceException(innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceException);
            }
        }

        private async ValueTask<ListenerEventArchivesViewDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var dependencyValidationException =
                new ListenerEventArchivesViewDependencyValidationException(
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(dependencyValidationException);

            return dependencyValidationException;
        }

        private async ValueTask<ListenerEventArchivesViewDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var dependencyException =
                new ListenerEventArchivesViewDependencyException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(dependencyException);

            return dependencyException;
        }

        private async ValueTask<ListenerEventArchivesViewServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var serviceException =
                new ListenerEventArchivesViewServiceException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(serviceException);

            return serviceException;
        }
    }
}
