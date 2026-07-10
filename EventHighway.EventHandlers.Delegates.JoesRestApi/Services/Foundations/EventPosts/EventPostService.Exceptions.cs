// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Delegates.JoesRestApi.Models.Foundations.EventPosts.Exceptions;
using Xeptions;

namespace EventHighway.EventHandlers.Delegates.JoesRestApi.Services.Foundations.EventPosts
{
    internal partial class EventPostService
    {
        private delegate ValueTask<EventHandlerResult> ReturningEventHandlerResultFunction();

        private async ValueTask<EventHandlerResult> TryCatch(
            ReturningEventHandlerResultFunction returningEventHandlerResultFunction)
        {
            try
            {
                return await returningEventHandlerResultFunction();
            }
            catch (InvalidEventPostException invalidEventPostException)
            {
                throw CreateValidationException(invalidEventPostException);
            }
            catch (HttpRequestException httpRequestException)
            {
                throw CreateDependencyException(httpRequestException);
            }
            catch (TaskCanceledException taskCanceledException)
            {
                throw CreateDependencyException(taskCanceledException);
            }
            catch (Exception exception)
            {
                throw CreateServiceException(exception);
            }
        }

        private static EventPostValidationException CreateValidationException(
            Xeption innerException) =>
            new EventPostValidationException(
                message: "Event post validation error occurred, fix the errors and try again.",
                innerException: innerException);

        private static EventPostDependencyException CreateDependencyException(Exception exception)
        {
            var failedEventPostDependencyException =
                new FailedEventPostDependencyException(
                    message: "Failed to reach Joes REST API, contact support.",
                    innerException: exception,
                    data: exception.Data);

            return new EventPostDependencyException(
                message: "Event post dependency error occurred, contact support.",
                innerException: failedEventPostDependencyException);
        }

        private static EventPostServiceException CreateServiceException(Exception exception)
        {
            var failedEventPostServiceException =
                new FailedEventPostServiceException(
                    message: "Failed event post service error occurred, contact support.",
                    innerException: exception,
                    data: exception.Data);

            return new EventPostServiceException(
                message: "Event post service error occurred, contact support.",
                innerException: failedEventPostServiceException);
        }
    }
}
