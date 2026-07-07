// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventArchives.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.EventArchives.V2
{
    internal partial class EventArchiveV2ProcessingService
    {
        private delegate ValueTask ReturningNothingFunction();
        private delegate ValueTask<IQueryable<EventArchiveV2>> ReturningEventArchiveV2sFunction();
        private delegate ValueTask<EventArchiveV2> ReturningEventArchiveV2Function();
        private delegate ValueTask<IEnumerable<EventArchiveV2>> ReturningEventArchiveV2EnumerableFunction();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventArchiveV2ProcessingException =
                    new TimeoutEventArchiveV2ProcessingException(
                        message: "Failed event archive processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventArchiveV2ProcessingException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventArchiveV2ProcessingException
                nullEventArchiveV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventArchiveV2ProcessingException);
            }
            catch (EventArchiveV2ValidationException
                eventArchiveV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV2ValidationException);
            }
            catch (EventArchiveV2DependencyValidationException
                eventArchiveV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV2DependencyValidationException);
            }
            catch (EventArchiveV2DependencyException
                eventArchiveV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV2DependencyException);
            }
            catch (EventArchiveV2ServiceException
                eventArchiveV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventArchiveV2ProcessingServiceException =
                    new FailedEventArchiveV2ProcessingServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventArchiveV2ProcessingServiceException);
            }
        }

        private async ValueTask<IQueryable<EventArchiveV2>> TryCatch(
            ReturningEventArchiveV2sFunction returningEventArchiveV2sFunction)
        {
            try
            {
                return await returningEventArchiveV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventArchiveV2ProcessingException =
                    new TimeoutEventArchiveV2ProcessingException(
                        message: "Failed event archive processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventArchiveV2ProcessingException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (EventArchiveV2DependencyException eventArchiveV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventArchiveV2DependencyException);
            }
            catch (EventArchiveV2ServiceException eventArchiveV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventArchiveV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventArchiveV2ProcessingServiceException =
                    new FailedEventArchiveV2ProcessingServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventArchiveV2ProcessingServiceException);
            }
        }

        private async ValueTask<EventArchiveV2> TryCatch(
            ReturningEventArchiveV2Function returningEventArchiveV2Function)
        {
            try
            {
                return await returningEventArchiveV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventArchiveV2ProcessingException =
                    new TimeoutEventArchiveV2ProcessingException(
                        message: "Failed event archive processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventArchiveV2ProcessingException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventArchiveV2ProcessingException
                nullEventArchiveV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventArchiveV2ProcessingException);
            }
            catch (InvalidEventArchiveV2ProcessingException
                invalidEventArchiveV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventArchiveV2ProcessingException);
            }
            catch (EventArchiveV2ValidationException
                eventArchiveV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV2ValidationException);
            }
            catch (EventArchiveV2DependencyValidationException
                eventArchiveV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV2DependencyValidationException);
            }
            catch (EventArchiveV2DependencyException
                eventArchiveV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV2DependencyException);
            }
            catch (EventArchiveV2ServiceException
                eventArchiveV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventArchiveV2ProcessingServiceException =
                    new FailedEventArchiveV2ProcessingServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventArchiveV2ProcessingServiceException);
            }
        }

        private async ValueTask<IEnumerable<EventArchiveV2>> TryCatch(
            ReturningEventArchiveV2EnumerableFunction returningEventArchiveV2EnumerableFunction)
        {
            try
            {
                return await returningEventArchiveV2EnumerableFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventArchiveV2ProcessingException =
                    new TimeoutEventArchiveV2ProcessingException(
                        message: "Failed event archive processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventArchiveV2ProcessingException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventArchiveV2ProcessingException
                nullEventArchiveV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventArchiveV2ProcessingException);
            }
            catch (InvalidEventArchiveV2ProcessingException
                invalidEventArchiveV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventArchiveV2ProcessingException);
            }
            catch (EventArchiveV2ValidationException
                eventArchiveV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV2ValidationException);
            }
            catch (EventArchiveV2DependencyValidationException
                eventArchiveV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV2DependencyValidationException);
            }
            catch (EventArchiveV2DependencyException
                eventArchiveV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV2DependencyException);
            }
            catch (EventArchiveV2ServiceException
                eventArchiveV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventArchiveV2ProcessingServiceException =
                    new FailedEventArchiveV2ProcessingServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventArchiveV2ProcessingServiceException);
            }
        }

        private async ValueTask<EventArchiveV2ProcessingValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV2ProcessingValidationException =
                new EventArchiveV2ProcessingValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV2ProcessingValidationException);

            return eventArchiveV2ProcessingValidationException;
        }

        private async ValueTask<EventArchiveV2ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventArchiveV2ProcessingDependencyValidationException =
                new EventArchiveV2ProcessingDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventArchiveV2ProcessingDependencyValidationException);

            return eventArchiveV2ProcessingDependencyValidationException;
        }

        private async ValueTask<EventArchiveV2ProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV2ProcessingDependencyException =
                new EventArchiveV2ProcessingDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventArchiveV2ProcessingDependencyException);

            return eventArchiveV2ProcessingDependencyException;
        }

        private async ValueTask<EventArchiveV2ProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV2ProcessingServiceException =
                new EventArchiveV2ProcessingServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV2ProcessingServiceException);

            return eventArchiveV2ProcessingServiceException;
        }

        private async ValueTask<EventArchiveV2ProcessingDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var eventArchiveV2ProcessingDependencyException =
                new EventArchiveV2ProcessingDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV2ProcessingDependencyException);

            return eventArchiveV2ProcessingDependencyException;
        }

    }
}
