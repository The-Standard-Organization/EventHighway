// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.ListenerEventArchives.V1
{
    internal partial class ListenerEventArchiveV1ProcessingService
    {
        private delegate ValueTask<ListenerEventArchiveV1> ReturningListenerEventArchiveV1Function();

        private async ValueTask<ListenerEventArchiveV1> TryCatch(
            ReturningListenerEventArchiveV1Function returningListenerEventArchiveV1Function)
        {
            try
            {
                return await returningListenerEventArchiveV1Function();
            }
            catch (NullListenerEventArchiveV1ProcessingException nullListenerEventArchiveV1ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullListenerEventArchiveV1ProcessingException);
            }
            catch (ListenerEventArchiveV1ValidationException ListenerEventArchiveV1ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(ListenerEventArchiveV1ValidationException);
            }
            catch (ListenerEventArchiveV1DependencyValidationException ListenerEventArchiveV1DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(ListenerEventArchiveV1DependencyValidationException);
            }
            catch (ListenerEventArchiveV1DependencyException listenerEventArchiveV1DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventArchiveV1DependencyException);
            }
            catch (ListenerEventArchiveV1ServiceException listenerEventArchiveV1ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventArchiveV1ServiceException);
            }
            catch (Exception exception)
            {
                var failedListenerEventArchiveV1ProcessingServiceException =
                    new FailedListenerEventArchiveV1ProcessingServiceException(
                        message: "Failed listener event archive service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedListenerEventArchiveV1ProcessingServiceException);
            }
        }

        private async ValueTask<ListenerEventArchiveV1ProcessingValidationException>
            CreateAndLogValidationExceptionAsync(
                Xeption exception)
        {
            var ListenerEventArchiveV1ProcessingValidationException =
                new ListenerEventArchiveV1ProcessingValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(ListenerEventArchiveV1ProcessingValidationException);

            return ListenerEventArchiveV1ProcessingValidationException;
        }

        private async ValueTask<ListenerEventArchiveV1ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var ListenerEventArchiveV1ProcessingDependencyValidationException =
                new ListenerEventArchiveV1ProcessingDependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(ListenerEventArchiveV1ProcessingDependencyValidationException);

            return ListenerEventArchiveV1ProcessingDependencyValidationException;
        }

        private async ValueTask<ListenerEventArchiveV1ProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var ListenerEventArchiveV1ProcessingDependencyException =
                new ListenerEventArchiveV1ProcessingDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(ListenerEventArchiveV1ProcessingDependencyException);

            return ListenerEventArchiveV1ProcessingDependencyException;
        }

        private async ValueTask<ListenerEventArchiveV1ProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var ListenerEventArchiveV1ProcessingServiceException =
                new ListenerEventArchiveV1ProcessingServiceException(
                    message: "Listener event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(ListenerEventArchiveV1ProcessingServiceException);

            return ListenerEventArchiveV1ProcessingServiceException;
        }
    }
}
