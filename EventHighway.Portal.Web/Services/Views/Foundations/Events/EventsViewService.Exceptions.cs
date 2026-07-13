// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.Events.V2.Exceptions;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Events.Exceptions;
using Xeptions;

namespace EventHighway.Portal.Web.Services.Views.Foundations.Events
{
    public partial class EventsViewService
    {
        private delegate ValueTask<T> ReturningEventsFunction<T>();

        private async ValueTask<T> TryCatch<T>(
            ReturningEventsFunction<T> returningEventsFunction)
        {
            try
            {
                return await returningEventsFunction();
            }
            catch (EventV2ClientValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    clientValidationException);
            }
            catch (EventV2ClientDependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (EventV2ClientServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedEventsViewServiceException(innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceException);
            }
        }

        private async ValueTask<EventsViewDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var dependencyValidationException =
                new EventsViewDependencyValidationException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(dependencyValidationException);

            return dependencyValidationException;
        }

        private async ValueTask<EventsViewDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var dependencyException =
                new EventsViewDependencyException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(dependencyException);

            return dependencyException;
        }

        private async ValueTask<EventsViewServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var serviceException =
                new EventsViewServiceException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(serviceException);

            return serviceException;
        }
    }
}
