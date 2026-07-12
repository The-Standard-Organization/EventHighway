// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ListenerEvents.V2.Exceptions;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEvents.Exceptions;
using Xeptions;

namespace EventHighway.Portal.Web.Services.Views.Foundations.ListenerEvents
{
    public partial class ListenerEventsViewService
    {
        private delegate ValueTask<T> ReturningListenerEventsFunction<T>();

        private async ValueTask<T> TryCatch<T>(
            ReturningListenerEventsFunction<T> returningListenerEventsFunction)
        {
            try
            {
                return await returningListenerEventsFunction();
            }
            catch (ListenerEventV2ClientValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    clientValidationException);
            }
            catch (ListenerEventV2ClientDependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (ListenerEventV2ClientServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedListenerEventsViewServiceException(innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceException);
            }
        }

        private async ValueTask<ListenerEventsViewDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var dependencyValidationException =
                new ListenerEventsViewDependencyValidationException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(dependencyValidationException);

            return dependencyValidationException;
        }

        private async ValueTask<ListenerEventsViewDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var dependencyException =
                new ListenerEventsViewDependencyException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(dependencyException);

            return dependencyException;
        }

        private async ValueTask<ListenerEventsViewServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var serviceException =
                new ListenerEventsViewServiceException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(serviceException);

            return serviceException;
        }
    }
}
