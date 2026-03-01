// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1.Exceptions;
using RESTFulSense.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventCalls.V1
{
    internal partial class EventCallV1Service
    {
        private delegate ValueTask<EventCallV1> ReturningEventCallV1Function();

        private async ValueTask<EventCallV1> TryCatch(ReturningEventCallV1Function returningEventCallV1Function)
        {
            try
            {
                return await returningEventCallV1Function();
            }
            catch (NullEventCallV1Exception nullEventCallV1Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventCallV1Exception);
            }
            catch (NullHttpResponseMessageException nullHttpResponseMessageException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullHttpResponseMessageException);
            }
            catch (InvalidEventCallV1Exception invalidEventCallV1Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidEventCallV1Exception);
            }
            catch (HttpResponseUrlNotFoundException httpResponseUrlNotFoundException)
            {
                var failedEventCallV1ConfigurationException =
                    new FailedEventCallV1ConfigurationException(
                        message: "Failed event call configuration error occurred, contact support.",
                        innerException: httpResponseUrlNotFoundException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedEventCallV1ConfigurationException);
            }
            catch (HttpResponseUnauthorizedException httpResponseUnauthorizedException)
            {
                var failedEventCallV1ConfigurationException =
                    new FailedEventCallV1ConfigurationException(
                        message: "Failed event call configuration error occurred, contact support.",
                        innerException: httpResponseUnauthorizedException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedEventCallV1ConfigurationException);
            }
            catch (HttpResponseForbiddenException httpResponseForbiddenException)
            {
                var failedEventCallV1ConfigurationException =
                    new FailedEventCallV1ConfigurationException(
                        message: "Failed event call configuration error occurred, contact support.",
                        innerException: httpResponseForbiddenException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedEventCallV1ConfigurationException);
            }
            catch (HttpResponseMethodNotAllowedException httpResponseMethodNotAllowedException)
            {
                var failedEventCallV1ConfigurationException =
                    new FailedEventCallV1ConfigurationException(
                        message: "Failed event call configuration error occurred, contact support.",
                        innerException: httpResponseMethodNotAllowedException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedEventCallV1ConfigurationException);
            }
            catch (HttpResponseUnprocessableEntityException httpResponseUnprocessableEntityException)
            {
                var failedEventCallV1RequestException =
                    new FailedEventCallV1RequestException(
                        message: "Failed event call request error occurred, fix the errors and try again.",
                        innerException: httpResponseUnprocessableEntityException);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    failedEventCallV1RequestException);
            }
            catch (HttpResponseBadRequestException httpResponseBadRequestException)
            {
                var invalidEventCallV1Exception =
                    new InvalidEventCallV1Exception(
                        message: "Event call is invalid, fix the errors and try again.",
                        innerException: httpResponseBadRequestException,
                        data: httpResponseBadRequestException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    invalidEventCallV1Exception);
            }
            catch (HttpResponseConflictException httpResponseConflictException)
            {
                var alreadyExistsEventCallV1Exception =
                    new AlreadyExistsEventCallV1Exception(
                        message: "Event call with same id already exists, try again.",
                        innerException: httpResponseConflictException);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsEventCallV1Exception);
            }
            catch (HttpResponseFailedDependencyException httpResponseFailedDependencyException)
            {
                var invalidEventCallV1ReferenceException =
                    new InvalidEventCallV1ReferenceException(
                        message: "Invalid event call reference error occurred, fix the errors and try again.",
                        innerException: httpResponseFailedDependencyException);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    invalidEventCallV1ReferenceException);
            }
            catch (HttpResponseException httpResponseException)
            {
                var failedEventCallV1DependencyException =
                    new FailedEventCallV1DependencyException(
                        message: "Failed event call dependency error occurred, contact support.",
                        innerException: httpResponseException);

                throw await CreateAndLogDependencyExceptionAsync(
                    failedEventCallV1DependencyException);
            }
            catch (Exception serviceException)
            {
                var failedEventCallV1ServiceException =
                    new FailedEventCallV1ServiceException(
                        message: "Failed event call service error occurred, contact support.",
                        innerException: serviceException);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventCallV1ServiceException);
            }
        }

        private async ValueTask<EventCallV1ValidationException> CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var eventCallV1ValidationException =
                new EventCallV1ValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventCallV1ValidationException);

            return eventCallV1ValidationException;
        }

        private async ValueTask<EventCallV1DependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var eventCallV1DependencyException =
                new EventCallV1DependencyException(
                    message: "Event call dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(eventCallV1DependencyException);

            return eventCallV1DependencyException;
        }

        private async ValueTask<EventCallV1DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventCallV1DependencyValidationException =
                new EventCallV1DependencyValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventCallV1DependencyValidationException);

            return eventCallV1DependencyValidationException;
        }

        private async ValueTask<EventCallV1DependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventCallV1DependencyException =
                new EventCallV1DependencyException(
                    message: "Event call dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventCallV1DependencyException);

            return eventCallV1DependencyException;
        }

        private async ValueTask<EventCallV1ServiceException> CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var eventCallV1ServiceException =
                new EventCallV1ServiceException(
                    message: "Event call service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventCallV1ServiceException);

            return eventCallV1ServiceException;
        }
    }
}
