// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.Events.V2
{
    internal partial class EventV2ProcessingService
    {
        private delegate ValueTask ReturningNothingFunction();
        private delegate ValueTask<string> ReturningStringFunction();
        private delegate ValueTask<IQueryable<EventV2>> ReturningEventV2sFunction();
        private delegate ValueTask<EventV2> ReturningEventV2Function();

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

                var timeoutEventV2ProcessingException =
                    new TimeoutEventV2ProcessingException(
                        message: "Failed event processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                var eventV2ProcessingDependencyException =
                    new EventV2ProcessingDependencyException(
                        message: "Event dependency error occurred, contact support.",
                        innerException: timeoutEventV2ProcessingException);

                await this.loggingBroker.LogErrorAsync(eventV2ProcessingDependencyException);

                throw eventV2ProcessingDependencyException;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventV2ProcessingException nullEventV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventV2ProcessingException);
            }
            catch (EventV2ValidationException eventV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(eventV2ValidationException);
            }
            catch (EventV2DependencyValidationException eventV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(eventV2DependencyValidationException);
            }
            catch (EventV2DependencyException eventV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2DependencyException);
            }
            catch (EventV2ServiceException eventV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV2ProcessingServiceException =
                    new FailedEventV2ProcessingServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventV2ProcessingServiceException);
            }
        }

        private async ValueTask<string> TryCatch(ReturningStringFunction returningStringFunction)
        {
            try
            {
                return await returningStringFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventV2ProcessingException =
                    new TimeoutEventV2ProcessingException(
                        message: "Failed event processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                var eventV2ProcessingDependencyException =
                    new EventV2ProcessingDependencyException(
                        message: "Event dependency error occurred, contact support.",
                        innerException: timeoutEventV2ProcessingException);

                await this.loggingBroker.LogErrorAsync(eventV2ProcessingDependencyException);

                throw eventV2ProcessingDependencyException;
            }
            catch (NullEventV2ProcessingException nullEventV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventV2ProcessingException);
            }
            catch (EventV2ValidationException eventV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(eventV2ValidationException);
            }
            catch (EventV2DependencyValidationException eventV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(eventV2DependencyValidationException);
            }
            catch (EventV2DependencyException eventV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2DependencyException);
            }
            catch (EventV2ServiceException eventV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ServiceException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                var failedEventV2ProcessingServiceException =
                    new FailedEventV2ProcessingServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventV2ProcessingServiceException);
            }
        }

        private async ValueTask<IQueryable<EventV2>> TryCatch(ReturningEventV2sFunction returningEventV2sFunction)
        {
            try
            {
                return await returningEventV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventV2ProcessingException =
                    new TimeoutEventV2ProcessingException(
                        message: "Failed event processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                var eventV2ProcessingDependencyException =
                    new EventV2ProcessingDependencyException(
                        message: "Event dependency error occurred, contact support.",
                        innerException: timeoutEventV2ProcessingException);

                await this.loggingBroker.LogErrorAsync(eventV2ProcessingDependencyException);

                throw eventV2ProcessingDependencyException;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (EventV2DependencyException eventV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2DependencyException);
            }
            catch (EventV2ServiceException eventV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV2ProcessingServiceException =
                    new FailedEventV2ProcessingServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventV2ProcessingServiceException);
            }
        }

        private async ValueTask<EventV2> TryCatch(ReturningEventV2Function returningEventV2Function)
        {
            try
            {
                return await returningEventV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventV2ProcessingException =
                    new TimeoutEventV2ProcessingException(
                        message: "Failed event processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                var eventV2ProcessingDependencyException =
                    new EventV2ProcessingDependencyException(
                        message: "Event dependency error occurred, contact support.",
                        innerException: timeoutEventV2ProcessingException);

                await this.loggingBroker.LogErrorAsync(eventV2ProcessingDependencyException);

                throw eventV2ProcessingDependencyException;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventV2ProcessingException nullEventV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventV2ProcessingException);
            }
            catch (InvalidEventV2ProcessingException invalidEventV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventV2ProcessingException);
            }
            catch (EventV2ValidationException eventV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ValidationException);
            }
            catch (EventV2DependencyValidationException eventV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2DependencyValidationException);
            }
            catch (EventV2DependencyException eventV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2DependencyException);
            }
            catch (EventV2ServiceException eventV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV2ProcessingServiceException =
                    new FailedEventV2ProcessingServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventV2ProcessingServiceException);
            }
        }

        private async ValueTask<EventV2ProcessingValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventV2ProcessingValidationException =
                new EventV2ProcessingValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV2ProcessingValidationException);

            return eventV2ProcessingValidationException;
        }

        private async ValueTask<EventV2ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventV2ProcessingDependencyValidationException =
                new EventV2ProcessingDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV2ProcessingDependencyValidationException);

            return eventV2ProcessingDependencyValidationException;
        }

        private async ValueTask<EventV2ProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventV2ProcessingDependencyException =
                new EventV2ProcessingDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV2ProcessingDependencyException);

            return eventV2ProcessingDependencyException;
        }

        private async ValueTask<EventV2ProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventV2ProcessingServiceException =
                new EventV2ProcessingServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV2ProcessingServiceException);

            return eventV2ProcessingServiceException;
        }
    }
}
