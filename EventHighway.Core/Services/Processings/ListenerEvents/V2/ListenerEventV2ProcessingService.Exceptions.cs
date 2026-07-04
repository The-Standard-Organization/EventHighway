// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.ListenerEvents.V2
{
    internal partial class ListenerEventV2ProcessingService
    {
        private delegate ValueTask ReturningNothingFunction();
        private delegate ValueTask<ListenerEventV2> ReturningListenerEventV2Function();
        private delegate ValueTask<IQueryable<ListenerEventV2>> ReturningListenerEventV2sFunction();
        private delegate ValueTask<IEnumerable<ListenerEventV2>> ReturningListenerEventV2EnumerableFunction();

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

                var timeoutListenerEventV2ProcessingException =
                    new TimeoutListenerEventV2ProcessingException(
                        message: "Failed listener event processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                var listenerEventV2ProcessingDependencyException =
                    new ListenerEventV2ProcessingDependencyException(
                        message: "Listener event dependency error occurred, contact support.",
                        innerException: timeoutListenerEventV2ProcessingException);

                await this.loggingBroker.LogErrorAsync(listenerEventV2ProcessingDependencyException);
                throw listenerEventV2ProcessingDependencyException;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullListenerEventV2ProcessingException
                nullListenerEventV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventV2ProcessingException);
            }
            catch (InvalidListenerEventV2ProcessingException
                invalidListenerEventV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidListenerEventV2ProcessingException);
            }
            catch (ListenerEventV2ValidationException
                listenerEventV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ValidationException);
            }
            catch (ListenerEventV2DependencyValidationException
                listenerEventV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2DependencyValidationException);
            }
            catch (ListenerEventV2DependencyException
                listenerEventV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2DependencyException);
            }
            catch (ListenerEventV2ServiceException
                listenerEventV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedListenerEventV2ProcessingServiceException =
                    new FailedListenerEventV2ProcessingServiceException(
                        message: "Failed listener event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV2ProcessingServiceException);
            }
        }

        private async ValueTask<ListenerEventV2> TryCatch(
            ReturningListenerEventV2Function returningListenerEventV2Function)
        {
            try
            {
                return await returningListenerEventV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutListenerEventV2ProcessingException =
                    new TimeoutListenerEventV2ProcessingException(
                        message: "Failed listener event processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                var listenerEventV2ProcessingDependencyException =
                    new ListenerEventV2ProcessingDependencyException(
                        message: "Listener event dependency error occurred, contact support.",
                        innerException: timeoutListenerEventV2ProcessingException);

                await this.loggingBroker.LogErrorAsync(listenerEventV2ProcessingDependencyException);
                throw listenerEventV2ProcessingDependencyException;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullListenerEventV2ProcessingException
                nullListenerEventV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventV2ProcessingException);
            }
            catch (InvalidListenerEventV2ProcessingException
                invalidListenerEventV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidListenerEventV2ProcessingException);
            }
            catch (ListenerEventV2ValidationException
                listenerEventV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ValidationException);
            }
            catch (ListenerEventV2DependencyValidationException
                listenerEventV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2DependencyValidationException);
            }
            catch (ListenerEventV2DependencyException
                listenerEventV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2DependencyException);
            }
            catch (ListenerEventV2ServiceException
                listenerEventV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedListenerEventV2ProcessingServiceException =
                    new FailedListenerEventV2ProcessingServiceException(
                        message: "Failed listener event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV2ProcessingServiceException);
            }
        }

        private async ValueTask<IQueryable<ListenerEventV2>> TryCatch(
            ReturningListenerEventV2sFunction returningListenerEventV2sFunction)
        {
            try
            {
                return await returningListenerEventV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutListenerEventV2ProcessingException =
                    new TimeoutListenerEventV2ProcessingException(
                        message: "Failed listener event processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                var listenerEventV2ProcessingDependencyException =
                    new ListenerEventV2ProcessingDependencyException(
                        message: "Listener event dependency error occurred, contact support.",
                        innerException: timeoutListenerEventV2ProcessingException);

                await this.loggingBroker.LogErrorAsync(listenerEventV2ProcessingDependencyException);
                throw listenerEventV2ProcessingDependencyException;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (ListenerEventV2DependencyException
                listenerEventV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2DependencyException);
            }
            catch (ListenerEventV2ServiceException
                listenerEventV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedListenerEventV2ProcessingServiceException =
                    new FailedListenerEventV2ProcessingServiceException(
                        message: "Failed listener event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV2ProcessingServiceException);
            }
        }

        private async ValueTask<IEnumerable<ListenerEventV2>> TryCatch(
            ReturningListenerEventV2EnumerableFunction returningListenerEventV2EnumerableFunction)
        {
            try
            {
                return await returningListenerEventV2EnumerableFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutListenerEventV2ProcessingException =
                    new TimeoutListenerEventV2ProcessingException(
                        message: "Failed listener event processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                var listenerEventV2ProcessingDependencyException =
                    new ListenerEventV2ProcessingDependencyException(
                        message: "Listener event dependency error occurred, contact support.",
                        innerException: timeoutListenerEventV2ProcessingException);

                await this.loggingBroker.LogErrorAsync(listenerEventV2ProcessingDependencyException);
                throw listenerEventV2ProcessingDependencyException;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullListenerEventV2ProcessingException
                nullListenerEventV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventV2ProcessingException);
            }
            catch (InvalidListenerEventV2ProcessingException
                invalidListenerEventV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidListenerEventV2ProcessingException);
            }
            catch (ListenerEventV2ValidationException
                listenerEventV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ValidationException);
            }
            catch (ListenerEventV2DependencyValidationException
                listenerEventV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2DependencyValidationException);
            }
            catch (ListenerEventV2DependencyException
                listenerEventV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2DependencyException);
            }
            catch (ListenerEventV2ServiceException
                listenerEventV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedListenerEventV2ProcessingServiceException =
                    new FailedListenerEventV2ProcessingServiceException(
                        message: "Failed listener event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV2ProcessingServiceException);
            }
        }

        private async ValueTask<ListenerEventV2ProcessingValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var listenerEventV2ProcessingValidationException =
                new ListenerEventV2ProcessingValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV2ProcessingValidationException);

            return listenerEventV2ProcessingValidationException;
        }

        private async ValueTask<ListenerEventV2ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var listenerEventV2ProcessingDependencyValidationException =
                new ListenerEventV2ProcessingDependencyValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(listenerEventV2ProcessingDependencyValidationException);

            return listenerEventV2ProcessingDependencyValidationException;
        }

        private async ValueTask<ListenerEventV2ProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var listenerEventV2ProcessingDependencyException =
                new ListenerEventV2ProcessingDependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(listenerEventV2ProcessingDependencyException);

            return listenerEventV2ProcessingDependencyException;
        }

        private async ValueTask<ListenerEventV2ProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var listenerEventV2ProcessingServiceException =
                new ListenerEventV2ProcessingServiceException(
                    message: "Listener event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV2ProcessingServiceException);

            return listenerEventV2ProcessingServiceException;
        }
    }
}
