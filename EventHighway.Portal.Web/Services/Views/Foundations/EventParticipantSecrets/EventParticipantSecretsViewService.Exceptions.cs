// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventParticipantSecrets.V2.Exceptions;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipantSecrets.Exceptions;
using Xeptions;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventParticipantSecrets
{
    public partial class EventParticipantSecretsViewService
    {
        private delegate ValueTask<T> ReturningSecretsFunction<T>();

        private async ValueTask<T> TryCatch<T>(
            ReturningSecretsFunction<T> returningSecretsFunction)
        {
            try
            {
                return await returningSecretsFunction();
            }
            catch (EventParticipantSecretV2ClientValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    clientValidationException);
            }
            catch (EventParticipantSecretV2ClientDependencyValidationException
                clientDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    clientDependencyValidationException);
            }
            catch (EventParticipantSecretV2ClientDependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (EventParticipantSecretV2ClientServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedEventParticipantSecretsViewServiceException(
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceException);
            }
        }

        private async ValueTask<EventParticipantSecretsViewDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var dependencyValidationException =
                new EventParticipantSecretsViewDependencyValidationException(
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(dependencyValidationException);

            return dependencyValidationException;
        }

        private async ValueTask<EventParticipantSecretsViewDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var dependencyException =
                new EventParticipantSecretsViewDependencyException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(dependencyException);

            return dependencyException;
        }

        private async ValueTask<EventParticipantSecretsViewServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var serviceException =
                new EventParticipantSecretsViewServiceException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(serviceException);

            return serviceException;
        }
    }
}
