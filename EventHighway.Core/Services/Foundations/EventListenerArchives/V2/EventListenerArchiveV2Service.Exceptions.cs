// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventListenerArchives.V2
{
    internal partial class EventListenerArchiveV2Service
    {
        private delegate ValueTask<EventListenerArchiveV2> ReturningEventListenerArchiveV2Function();

        private async ValueTask<EventListenerArchiveV2> TryCatch(
            ReturningEventListenerArchiveV2Function returningEventListenerArchiveV2Function)
        {
            try
            {
                return await returningEventListenerArchiveV2Function();
            }
            catch (NullEventListenerArchiveV2Exception nullEventListenerArchiveV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventListenerArchiveV2Exception);
            }
            catch (InvalidEventListenerArchiveV2Exception invalidEventListenerArchiveV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventListenerArchiveV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventListenerArchiveV2Exception =
                    new FailedStorageEventListenerArchiveV2Exception(
                        message: "Failed event listener archive storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventListenerArchiveV2Exception);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsEventListenerArchiveV2Exception =
                    new AlreadyExistsEventListenerArchiveV2Exception(
                        message: "Event listener archive with the same id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsEventListenerArchiveV2Exception);
            }
        }

        private async ValueTask<EventListenerArchiveV2ValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventListenerArchiveV2ValidationException =
                new EventListenerArchiveV2ValidationException(
                    message: "Event listener archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerArchiveV2ValidationException);

            return eventListenerArchiveV2ValidationException;
        }

        private async ValueTask<EventListenerArchiveV2DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var eventListenerArchiveV2DependencyValidationException =
                new EventListenerArchiveV2DependencyValidationException(
                    message: "Event listener archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerArchiveV2DependencyValidationException);

            return eventListenerArchiveV2DependencyValidationException;
        }

        private async ValueTask<EventListenerArchiveV2DependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var eventListenerArchiveV2DependencyException =
                new EventListenerArchiveV2DependencyException(
                    message: "Event listener archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(eventListenerArchiveV2DependencyException);

            return eventListenerArchiveV2DependencyException;
        }
    }
}
