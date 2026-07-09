// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.EventParticipants.V2
{
    internal partial class EventParticipantV2ProcessingService
    {
        private delegate ValueTask<EventParticipantV2> ReturningEventParticipantV2Function();
        private delegate ValueTask<IQueryable<EventParticipantV2>> ReturningEventParticipantV2sFunction();

        private async ValueTask<EventParticipantV2> TryCatch(
            ReturningEventParticipantV2Function returningEventParticipantV2Function)
        {
            try
            {
                return await returningEventParticipantV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventParticipantV2ProcessingException =
                    new TimeoutEventParticipantV2ProcessingException(
                        message: "Failed event participant processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventParticipantV2ProcessingException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventParticipantV2ProcessingException
                nullEventParticipantV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventParticipantV2ProcessingException);
            }
            catch (InvalidEventParticipantV2ProcessingException
                invalidEventParticipantV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventParticipantV2ProcessingException);
            }
            catch (EventParticipantV2ValidationException
                eventParticipantV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventParticipantV2ValidationException);
            }
            catch (EventParticipantV2DependencyValidationException
                eventParticipantV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventParticipantV2DependencyValidationException);
            }
            catch (EventParticipantV2DependencyException
                eventParticipantV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventParticipantV2DependencyException);
            }
            catch (EventParticipantV2ServiceException
                eventParticipantV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventParticipantV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventParticipantV2ProcessingServiceException =
                    new FailedEventParticipantV2ProcessingServiceException(
                        message: "Failed event participant service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventParticipantV2ProcessingServiceException);
            }
        }

        private async ValueTask<IQueryable<EventParticipantV2>> TryCatch(
            ReturningEventParticipantV2sFunction returningEventParticipantV2sFunction)
        {
            try
            {
                return await returningEventParticipantV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventParticipantV2ProcessingException =
                    new TimeoutEventParticipantV2ProcessingException(
                        message: "Failed event participant processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventParticipantV2ProcessingException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (EventParticipantV2DependencyException
                eventParticipantV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventParticipantV2DependencyException);
            }
            catch (EventParticipantV2ServiceException
                eventParticipantV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventParticipantV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventParticipantV2ProcessingServiceException =
                    new FailedEventParticipantV2ProcessingServiceException(
                        message: "Failed event participant service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventParticipantV2ProcessingServiceException);
            }
        }

        private async ValueTask<EventParticipantV2ProcessingValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var eventParticipantV2ProcessingValidationException =
                new EventParticipantV2ProcessingValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantV2ProcessingValidationException);

            return eventParticipantV2ProcessingValidationException;
        }

        private async ValueTask<EventParticipantV2ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventParticipantV2ProcessingDependencyValidationException =
                new EventParticipantV2ProcessingDependencyValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventParticipantV2ProcessingDependencyValidationException);

            return eventParticipantV2ProcessingDependencyValidationException;
        }

        private async ValueTask<EventParticipantV2ProcessingDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var eventParticipantV2ProcessingDependencyException =
                new EventParticipantV2ProcessingDependencyException(
                    message: "Event participant dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventParticipantV2ProcessingDependencyException);

            return eventParticipantV2ProcessingDependencyException;
        }

        private async ValueTask<EventParticipantV2ProcessingServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var eventParticipantV2ProcessingServiceException =
                new EventParticipantV2ProcessingServiceException(
                    message: "Event participant service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantV2ProcessingServiceException);

            return eventParticipantV2ProcessingServiceException;
        }

        private async ValueTask<EventParticipantV2ProcessingDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var eventParticipantV2ProcessingDependencyException =
                new EventParticipantV2ProcessingDependencyException(
                    message: "Event participant dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantV2ProcessingDependencyException);

            return eventParticipantV2ProcessingDependencyException;
        }
    }
}
