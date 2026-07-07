// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.PromotedProperties;
using EventHighway.Core.Models.Services.Processings.EventCalls.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.EventCalls.V2
{
    internal partial class EventCallV2ProcessingService
    {
        private delegate ValueTask<EventCallV2> ReturningEventCallV2Function();
        private delegate ValueTask<IEnumerable<string>> ReturningPromotedPropertyKeysFunction();
        private delegate ValueTask<List<PromotedProperty>> ReturningPromotedPropertiesListFunction();

        private async ValueTask<IEnumerable<string>> TryCatch(
            ReturningPromotedPropertyKeysFunction returningPromotedPropertyKeysFunction)
        {
            try
            {
                return await returningPromotedPropertyKeysFunction();
            }
            catch (InvalidEventCallV2ProcessingException invalidEventCallV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidEventCallV2ProcessingException);
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventCallV2ProcessingException =
                    new TimeoutEventCallV2ProcessingException(
                        message: "Failed event call processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventCallV2ProcessingException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                var failedEventCallV2ProcessingServiceException =
                    new FailedEventCallV2ProcessingServiceException(
                        message: "Failed event call service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventCallV2ProcessingServiceException);
            }
        }

        private async ValueTask<List<PromotedProperty>> TryCatch(
            ReturningPromotedPropertiesListFunction returningPromotedPropertiesListFunction)
        {
            try
            {
                return await returningPromotedPropertiesListFunction();
            }
            catch (InvalidEventCallV2ProcessingException invalidEventCallV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidEventCallV2ProcessingException);
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventCallV2ProcessingException =
                    new TimeoutEventCallV2ProcessingException(
                        message: "Failed event call processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventCallV2ProcessingException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                var failedEventCallV2ProcessingServiceException =
                    new FailedEventCallV2ProcessingServiceException(
                        message: "Failed event call service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventCallV2ProcessingServiceException);
            }
        }

        private async ValueTask<EventCallV2> TryCatch(ReturningEventCallV2Function returningEventCallV2Function)
        {
            try
            {
                return await returningEventCallV2Function();
            }
            catch (NullEventCallV2ProcessingException nullEventCallV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventCallV2ProcessingException);
            }
            catch (EventCallV2ValidationException eventCallV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(eventCallV2ValidationException);
            }
            catch (EventCallV2DependencyValidationException eventCallV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(eventCallV2DependencyValidationException);
            }
            catch (EventCallV2DependencyException eventCallV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventCallV2DependencyException);
            }
            catch (EventCallV2ServiceException eventCallV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventCallV2ServiceException);
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventCallV2ProcessingException =
                    new TimeoutEventCallV2ProcessingException(
                        message: "Failed event call processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventCallV2ProcessingException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                var failedEventCallV2ProcessingServiceException =
                    new FailedEventCallV2ProcessingServiceException(
                        message: "Failed event call service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventCallV2ProcessingServiceException);
            }
        }

        private async ValueTask<EventCallV2ProcessingValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventCallV2ProcessingValidationException =
                new EventCallV2ProcessingValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventCallV2ProcessingValidationException);

            return eventCallV2ProcessingValidationException;
        }

        private async ValueTask<EventCallV2ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventCallV2ProcessingDependencyValidationException =
                new EventCallV2ProcessingDependencyValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventCallV2ProcessingDependencyValidationException);

            return eventCallV2ProcessingDependencyValidationException;
        }

        private async ValueTask<EventCallV2ProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventCallV2ProcessingDependencyException =
                new EventCallV2ProcessingDependencyException(
                    message: "Event call dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventCallV2ProcessingDependencyException);

            return eventCallV2ProcessingDependencyException;
        }

        private async ValueTask<EventCallV2ProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventCallV2ProcessingServiceException =
                new EventCallV2ProcessingServiceException(
                    message: "Event call service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventCallV2ProcessingServiceException);

            return eventCallV2ProcessingServiceException;
        }

        private async ValueTask<EventCallV2ProcessingDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var eventCallV2ProcessingDependencyException =
                new EventCallV2ProcessingDependencyException(
                    message: "Event call dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventCallV2ProcessingDependencyException);

            return eventCallV2ProcessingDependencyException;
        }

    }
}
