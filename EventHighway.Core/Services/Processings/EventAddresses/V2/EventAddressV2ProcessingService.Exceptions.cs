// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.EventAddresses.V2
{
    internal partial class EventAddressV2ProcessingService
    {
        private delegate ValueTask<IQueryable<EventAddressV2>> ReturningEventAddressV2sFunction();
        private delegate ValueTask<EventAddressV2> ReturningEventAddressV2Function();

        private async ValueTask<IQueryable<EventAddressV2>> TryCatch(
            ReturningEventAddressV2sFunction returningEventAddressV2sFunction)
        {
            try
            {
                return await returningEventAddressV2sFunction();
            }
            catch (EventAddressV2DependencyException eventAddressV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventAddressV2DependencyException);
            }
            catch (EventAddressV2ServiceException eventAddressV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventAddressV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventAddressV2ProcessingServiceException =
                    new FailedEventAddressV2ProcessingServiceException(
                        message: "Failed event address service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventAddressV2ProcessingServiceException);
            }
        }

        private async ValueTask<EventAddressV2> TryCatch(
            ReturningEventAddressV2Function returningEventAddressV2Function)
        {
            try
            {
                return await returningEventAddressV2Function();
            }
            catch (NullEventAddressV2ProcessingException
                nullEventAddressV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventAddressV2ProcessingException);
            }
            catch (InvalidEventAddressV2ProcessingException
                invalidEventAddressV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventAddressV2ProcessingException);
            }
            catch (EventAddressV2ValidationException
                eventAddressV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventAddressV2ValidationException);
            }
            catch (EventAddressV2DependencyValidationException
                eventAddressV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventAddressV2DependencyValidationException);
            }
            catch (EventAddressV2DependencyException
                eventAddressV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventAddressV2DependencyException);
            }
            catch (EventAddressV2ServiceException
                eventAddressV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventAddressV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventAddressV2ProcessingServiceException =
                    new FailedEventAddressV2ProcessingServiceException(
                        message: "Failed event address service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventAddressV2ProcessingServiceException);
            }
        }

        private async ValueTask<EventAddressV2ProcessingValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventAddressV2ProcessingValidationException =
                new EventAddressV2ProcessingValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventAddressV2ProcessingValidationException);

            return eventAddressV2ProcessingValidationException;
        }

        private async ValueTask<EventAddressV2ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventAddressV2ProcessingDependencyValidationException =
                new EventAddressV2ProcessingDependencyValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventAddressV2ProcessingDependencyValidationException);

            return eventAddressV2ProcessingDependencyValidationException;
        }

        private async ValueTask<EventAddressV2ProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventAddressV2ProcessingDependencyException =
                new EventAddressV2ProcessingDependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventAddressV2ProcessingDependencyException);

            return eventAddressV2ProcessingDependencyException;
        }

        private async ValueTask<EventAddressV2ProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventAddressV2ProcessingServiceException =
                new EventAddressV2ProcessingServiceException(
                    message: "Event address service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventAddressV2ProcessingServiceException);

            return eventAddressV2ProcessingServiceException;
        }
    }
}
