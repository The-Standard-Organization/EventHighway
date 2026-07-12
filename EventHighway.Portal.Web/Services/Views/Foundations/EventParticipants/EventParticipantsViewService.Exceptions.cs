// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventParticipants.V2.Exceptions;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants.Exceptions;
using Xeptions;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventParticipants
{
    public partial class EventParticipantsViewService
    {
        private delegate ValueTask<T> ReturningParticipantsFunction<T>();

        private async ValueTask<T> TryCatch<T>(
            ReturningParticipantsFunction<T> returningParticipantsFunction)
        {
            try
            {
                return await returningParticipantsFunction();
            }
            catch (EventParticipantV2ClientValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    clientValidationException);
            }
            catch (EventParticipantV2ClientDependencyValidationException
                clientDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    clientDependencyValidationException);
            }
            catch (EventParticipantV2ClientDependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (EventParticipantV2ClientServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedEventParticipantsViewServiceException(
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceException);
            }
        }

        private async ValueTask<EventParticipantsViewDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var eventParticipantsViewDependencyValidationException =
                new EventParticipantsViewDependencyValidationException(
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                eventParticipantsViewDependencyValidationException);

            return eventParticipantsViewDependencyValidationException;
        }

        private async ValueTask<EventParticipantsViewDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var eventParticipantsViewDependencyException =
                new EventParticipantsViewDependencyException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantsViewDependencyException);

            return eventParticipantsViewDependencyException;
        }

        private async ValueTask<EventParticipantsViewServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var eventParticipantsViewServiceException =
                new EventParticipantsViewServiceException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantsViewServiceException);

            return eventParticipantsViewServiceException;
        }
    }
}
