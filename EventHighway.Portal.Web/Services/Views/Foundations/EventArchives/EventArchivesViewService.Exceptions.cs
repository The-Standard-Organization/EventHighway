// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Clients.EventArchives.V2.Exceptions;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventArchives.Exceptions;
using Xeptions;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventArchives
{
    public partial class EventArchivesViewService
    {
        private delegate ValueTask ReturningNothingFunction();
        private delegate ValueTask<T> ReturningEventArchivesFunction<T>();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (ArchivingEventV2ClientValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    clientValidationException);
            }
            catch (ArchivingEventV2ClientDependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (ArchivingEventV2ClientServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedEventArchivesViewServiceException(innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceException);
            }
        }

        private async ValueTask<T> TryCatch<T>(
            ReturningEventArchivesFunction<T> returningEventArchivesFunction)
        {
            try
            {
                return await returningEventArchivesFunction();
            }
            catch (EventArchiveV2ClientValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    clientValidationException);
            }
            catch (EventArchiveV2ClientDependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (EventArchiveV2ClientServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedEventArchivesViewServiceException(innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceException);
            }
        }

        private async ValueTask<EventArchivesViewDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var dependencyValidationException =
                new EventArchivesViewDependencyValidationException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(dependencyValidationException);

            return dependencyValidationException;
        }

        private async ValueTask<EventArchivesViewDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var dependencyException =
                new EventArchivesViewDependencyException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(dependencyException);

            return dependencyException;
        }

        private async ValueTask<EventArchivesViewServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var serviceException =
                new EventArchivesViewServiceException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(serviceException);

            return serviceException;
        }
    }
}
