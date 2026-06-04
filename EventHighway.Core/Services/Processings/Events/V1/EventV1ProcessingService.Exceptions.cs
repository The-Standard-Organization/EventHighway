// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.Events.V1.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.Events.V1
{
    internal partial class EventV1ProcessingService
    {
        private delegate ValueTask<IQueryable<EventV1>> ReturningEventV1sFunction();
        private delegate ValueTask<EventV1> ReturningEventV1Function();

        private async ValueTask<IQueryable<EventV1>> TryCatch(ReturningEventV1sFunction returningEventV1sFunction)
        {
            try
            {
                return await returningEventV1sFunction();
            }
            catch (EventV1DependencyException eventV1DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV1DependencyException);
            }
            catch (EventV1ServiceException eventV1ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV1ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV1ProcessingServiceException =
                    new FailedEventV1ProcessingServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventV1ProcessingServiceException);
            }
        }

        private async ValueTask<EventV1> TryCatch(ReturningEventV1Function returningEventV1Function)
        {
            try
            {
                return await returningEventV1Function();
            }
            catch (NullEventV1ProcessingException nullEventV1ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventV1ProcessingException);
            }
            catch (InvalidEventV1ProcessingException invalidEventV1ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventV1ProcessingException);
            }
            catch (EventV1ValidationException eventV1ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1ValidationException);
            }
            catch (EventV1DependencyValidationException eventV1DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1DependencyValidationException);
            }
            catch (EventV1DependencyException eventV1DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV1DependencyException);
            }
            catch (EventV1ServiceException eventV1ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV1ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV1ProcessingServiceException =
                    new FailedEventV1ProcessingServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventV1ProcessingServiceException);
            }
        }

        private async ValueTask<EventV1ProcessingValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventV1ProcessingValidationException =
                new EventV1ProcessingValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ProcessingValidationException);

            return eventV1ProcessingValidationException;
        }

        private async ValueTask<EventV1ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventV1ProcessingDependencyValidationException =
                new EventV1ProcessingDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV1ProcessingDependencyValidationException);

            return eventV1ProcessingDependencyValidationException;
        }

        private async ValueTask<EventV1ProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventV1ProcessingDependencyException =
                new EventV1ProcessingDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV1ProcessingDependencyException);

            return eventV1ProcessingDependencyException;
        }

        private async ValueTask<EventV1ProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventV1ProcessingServiceException =
                new EventV1ProcessingServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ProcessingServiceException);

            return eventV1ProcessingServiceException;
        }
    }
}
