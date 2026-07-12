// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventHandlers.V2.Exceptions;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventHandlers.Exceptions;
using Xeptions;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventHandlers
{
    public partial class EventHandlersViewService
    {
        private delegate ValueTask<T> ReturningEventHandlersFunction<T>();

        private async ValueTask<T> TryCatch<T>(
            ReturningEventHandlersFunction<T> returningEventHandlersFunction)
        {
            try
            {
                return await returningEventHandlersFunction();
            }
            catch (EventHandlerV2ClientValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    clientValidationException);
            }
            catch (EventHandlerV2ClientDependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (EventHandlerV2ClientServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedEventHandlersViewServiceException(
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceException);
            }
        }

        private async ValueTask<EventHandlersViewDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var eventHandlersViewDependencyValidationException =
                new EventHandlersViewDependencyValidationException(
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                eventHandlersViewDependencyValidationException);

            return eventHandlersViewDependencyValidationException;
        }

        private async ValueTask<EventHandlersViewDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var eventHandlersViewDependencyException =
                new EventHandlersViewDependencyException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventHandlersViewDependencyException);

            return eventHandlersViewDependencyException;
        }

        private async ValueTask<EventHandlersViewServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var eventHandlersViewServiceException =
                new EventHandlersViewServiceException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventHandlersViewServiceException);

            return eventHandlersViewServiceException;
        }
    }
}
