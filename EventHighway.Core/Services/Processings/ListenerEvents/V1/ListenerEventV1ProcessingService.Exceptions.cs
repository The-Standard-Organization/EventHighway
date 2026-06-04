// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V1.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.ListenerEvents.V1
{
    internal partial class ListenerEventV1ProcessingService
    {
        private delegate ValueTask<ListenerEventV1> ReturningListenerEventV1Function();
        private delegate ValueTask<IQueryable<ListenerEventV1>> ReturningListenerEventV1sFunction();

        private async ValueTask<ListenerEventV1> TryCatch(
            ReturningListenerEventV1Function returningListenerEventV1Function)
        {
            try
            {
                return await returningListenerEventV1Function();
            }
            catch (NullListenerEventV1ProcessingException
                nullListenerEventV1ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventV1ProcessingException);
            }
            catch (InvalidListenerEventV1ProcessingException
                invalidListenerEventV1ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidListenerEventV1ProcessingException);
            }
            catch (ListenerEventV1ValidationException
                listenerEventV1ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV1ValidationException);
            }
            catch (ListenerEventV1DependencyValidationException
                listenerEventV1DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV1DependencyValidationException);
            }
            catch (ListenerEventV1DependencyException
                listenerEventV1DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV1DependencyException);
            }
            catch (ListenerEventV1ServiceException
                listenerEventV1ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV1ServiceException);
            }
            catch (Exception exception)
            {
                var failedListenerEventV1ProcessingServiceException =
                    new FailedListenerEventV1ProcessingServiceException(
                        message: "Failed listener event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV1ProcessingServiceException);
            }
        }

        private async ValueTask<IQueryable<ListenerEventV1>> TryCatch(
            ReturningListenerEventV1sFunction returningListenerEventV1sFunction)
        {
            try
            {
                return await returningListenerEventV1sFunction();
            }
            catch (ListenerEventV1DependencyException
                listenerEventV1DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV1DependencyException);
            }
            catch (ListenerEventV1ServiceException
                listenerEventV1ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV1ServiceException);
            }
            catch (Exception exception)
            {
                var failedListenerEventV1ProcessingServiceException =
                    new FailedListenerEventV1ProcessingServiceException(
                        message: "Failed listener event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV1ProcessingServiceException);
            }
        }

        private async ValueTask<ListenerEventV1ProcessingValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var listenerEventV1ProcessingValidationException =
                new ListenerEventV1ProcessingValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV1ProcessingValidationException);

            return listenerEventV1ProcessingValidationException;
        }

        private async ValueTask<ListenerEventV1ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var listenerEventV1ProcessingDependencyValidationException =
                new ListenerEventV1ProcessingDependencyValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(listenerEventV1ProcessingDependencyValidationException);

            return listenerEventV1ProcessingDependencyValidationException;
        }

        private async ValueTask<ListenerEventV1ProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var listenerEventV1ProcessingDependencyException =
                new ListenerEventV1ProcessingDependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(listenerEventV1ProcessingDependencyException);

            return listenerEventV1ProcessingDependencyException;
        }

        private async ValueTask<ListenerEventV1ProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var listenerEventV1ProcessingServiceException =
                new ListenerEventV1ProcessingServiceException(
                    message: "Listener event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV1ProcessingServiceException);

            return listenerEventV1ProcessingServiceException;
        }
    }
}
