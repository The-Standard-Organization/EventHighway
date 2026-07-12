// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventAddresses.V2.Exceptions;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventAddresses.Exceptions;
using Xeptions;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventAddresses
{
    public partial class EventAddressesViewService
    {
        private delegate ValueTask<T> ReturningAddressesFunction<T>();

        private async ValueTask<T> TryCatch<T>(
            ReturningAddressesFunction<T> returningAddressesFunction)
        {
            try
            {
                return await returningAddressesFunction();
            }
            catch (EventAddressV2ClientValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    clientValidationException);
            }
            catch (EventAddressV2ClientDependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (EventAddressV2ClientServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedEventAddressesViewServiceException(innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceException);
            }
        }

        private async ValueTask<EventAddressesViewDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var dependencyValidationException =
                new EventAddressesViewDependencyValidationException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(dependencyValidationException);

            return dependencyValidationException;
        }

        private async ValueTask<EventAddressesViewDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var dependencyException =
                new EventAddressesViewDependencyException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(dependencyException);

            return dependencyException;
        }

        private async ValueTask<EventAddressesViewServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var serviceException =
                new EventAddressesViewServiceException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(serviceException);

            return serviceException;
        }
    }
}
