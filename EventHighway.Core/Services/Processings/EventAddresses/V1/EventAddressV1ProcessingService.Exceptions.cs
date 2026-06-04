// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V1.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.EventAddresses.V1
{
    internal partial class EventAddressV1ProcessingService
    {
        private delegate ValueTask<EventAddressV1> ReturningEventAddressV1Function();

        private async ValueTask<EventAddressV1> TryCatch(
            ReturningEventAddressV1Function returningEventAddressV1Function)
        {
            try
            {
                return await returningEventAddressV1Function();
            }
            catch (InvalidEventAddressV1ProcessingException
                invalidEventAddressV1ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventAddressV1ProcessingException);
            }
            catch (EventAddressV1ValidationException
                eventAddressV1ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventAddressV1ValidationException);
            }
            catch (EventAddressV1DependencyValidationException
                eventAddressV1DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventAddressV1DependencyValidationException);
            }
            catch (EventAddressV1DependencyException
                eventAddressV1DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventAddressV1DependencyException);
            }
            catch (EventAddressV1ServiceException
                eventAddressV1ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventAddressV1ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventAddressV1ProcessingServiceException =
                    new FailedEventAddressV1ProcessingServiceException(
                        message: "Failed event address service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventAddressV1ProcessingServiceException);
            }
        }

        private async ValueTask<EventAddressV1ProcessingValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventAddressV1ProcessingValidationException =
                new EventAddressV1ProcessingValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventAddressV1ProcessingValidationException);

            return eventAddressV1ProcessingValidationException;
        }

        private async ValueTask<EventAddressV1ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventAddressV1ProcessingDependencyValidationException =
                new EventAddressV1ProcessingDependencyValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventAddressV1ProcessingDependencyValidationException);

            return eventAddressV1ProcessingDependencyValidationException;
        }

        private async ValueTask<EventAddressV1ProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventAddressV1ProcessingDependencyException =
                new EventAddressV1ProcessingDependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventAddressV1ProcessingDependencyException);

            return eventAddressV1ProcessingDependencyException;
        }

        private async ValueTask<EventAddressV1ProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventAddressV1ProcessingServiceException =
                new EventAddressV1ProcessingServiceException(
                    message: "Event address service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventAddressV1ProcessingServiceException);

            return eventAddressV1ProcessingServiceException;
        }
    }
}
