// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventCalls.V1.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.EventCalls.V1
{
    internal partial class EventCallV1ProcessingService
    {
        private delegate ValueTask<EventCallV1> ReturningEventCallV1Function();

        private async ValueTask<EventCallV1> TryCatch(ReturningEventCallV1Function returningEventCallV1Function)
        {
            try
            {
                return await returningEventCallV1Function();
            }
            catch (NullEventCallV1ProcessingException nullEventCallV1ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventCallV1ProcessingException);
            }
            catch (EventCallV1ValidationException eventCallV1ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(eventCallV1ValidationException);
            }
            catch (EventCallV1DependencyValidationException eventCallV1DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(eventCallV1DependencyValidationException);
            }
            catch (EventCallV1DependencyException eventCallV1DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventCallV1DependencyException);
            }
            catch (EventCallV1ServiceException eventCallV1ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventCallV1ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventCallV1ProcessingServiceException =
                    new FailedEventCallV1ProcessingServiceException(
                        message: "Failed event call service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventCallV1ProcessingServiceException);
            }
        }

        private async ValueTask<EventCallV1ProcessingValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventCallV1ProcessingValidationException =
                new EventCallV1ProcessingValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventCallV1ProcessingValidationException);

            return eventCallV1ProcessingValidationException;
        }

        private async ValueTask<EventCallV1ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventCallV1ProcessingDependencyValidationException =
                new EventCallV1ProcessingDependencyValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventCallV1ProcessingDependencyValidationException);

            return eventCallV1ProcessingDependencyValidationException;
        }

        private async ValueTask<EventCallV1ProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventCallV1ProcessingDependencyException =
                new EventCallV1ProcessingDependencyException(
                    message: "Event call dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventCallV1ProcessingDependencyException);

            return eventCallV1ProcessingDependencyException;
        }

        private async ValueTask<EventCallV1ProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventCallV1ProcessingServiceException =
                new EventCallV1ProcessingServiceException(
                    message: "Event call service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventCallV1ProcessingServiceException);

            return eventCallV1ProcessingServiceException;
        }
    }
}
