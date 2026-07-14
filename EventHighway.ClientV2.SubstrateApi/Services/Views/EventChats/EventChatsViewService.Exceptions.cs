// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Models.MediaSubmissions.Exceptions;
using EventHighway.ClientV2.SubstrateApi.Models.ReceivedEvents.Exceptions;
using EventHighway.ClientV2.SubstrateApi.Models.Services.Views.EventChats;
using EventHighway.ClientV2.SubstrateApi.Models.Services.Views.EventChats.Exceptions;
using Xeptions;

namespace EventHighway.ClientV2.SubstrateApi.Services.Views.EventChats
{
    public partial class EventChatsViewService
    {
        private delegate ValueTask<List<ReceivedEventView>> ReturningReceivedEventViewsFunction();
        private delegate ValueTask<MediaSubmissionView> ReturningMediaSubmissionViewFunction();
        private delegate ValueTask<SubmitEndpointView> ReturningSubmitEndpointViewFunction();
        private delegate ValueTask<string> ReturningStringFunction();

        private async ValueTask<List<ReceivedEventView>> TryCatch(
            ReturningReceivedEventViewsFunction returningReceivedEventViewsFunction)
        {
            try
            {
                return await returningReceivedEventViewsFunction();
            }
            catch (ReceivedEventValidationException receivedEventValidationException)
            {
                throw CreateAndLogValidationException(receivedEventValidationException);
            }
            catch (ReceivedEventServiceException receivedEventServiceException)
            {
                throw CreateAndLogDependencyException(receivedEventServiceException);
            }
            catch (Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }

        private async ValueTask<MediaSubmissionView> TryCatch(
            ReturningMediaSubmissionViewFunction returningMediaSubmissionViewFunction)
        {
            try
            {
                return await returningMediaSubmissionViewFunction();
            }
            catch (MediaSubmissionValidationException mediaSubmissionValidationException)
            {
                throw CreateAndLogValidationException(mediaSubmissionValidationException);
            }
            catch (MediaSubmissionDependencyException mediaSubmissionDependencyException)
            {
                throw CreateAndLogDependencyException(mediaSubmissionDependencyException);
            }
            catch (MediaSubmissionServiceException mediaSubmissionServiceException)
            {
                throw CreateAndLogDependencyException(mediaSubmissionServiceException);
            }
            catch (Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }

        private async ValueTask<SubmitEndpointView> TryCatch(
            ReturningSubmitEndpointViewFunction returningSubmitEndpointViewFunction)
        {
            try
            {
                return await returningSubmitEndpointViewFunction();
            }
            catch (MediaSubmissionValidationException mediaSubmissionValidationException)
            {
                throw CreateAndLogValidationException(mediaSubmissionValidationException);
            }
            catch (Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }

        private async ValueTask<string> TryCatch(
            ReturningStringFunction returningStringFunction)
        {
            try
            {
                return await returningStringFunction();
            }
            catch (Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }

        private EventChatsViewValidationException CreateAndLogValidationException(Xeption exception)
        {
            var eventChatsViewValidationException =
                new EventChatsViewValidationException(
                    message: "Event chats view validation error occurred, fix the errors and try again.",
                    innerException: exception);

            this.loggingBroker.LogError(eventChatsViewValidationException);

            return eventChatsViewValidationException;
        }

        private EventChatsViewDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var eventChatsViewDependencyException =
                new EventChatsViewDependencyException(
                    message: "Event chats view dependency error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogError(eventChatsViewDependencyException);

            return eventChatsViewDependencyException;
        }

        private EventChatsViewServiceException CreateAndLogServiceException(Exception exception)
        {
            var failedEventChatsViewServiceException =
                new FailedEventChatsViewServiceException(
                    message: "Failed event chats view service error occurred, contact support.",
                    innerException: exception);

            var eventChatsViewServiceException =
                new EventChatsViewServiceException(
                    message: "Event chats view service error occurred, contact support.",
                    innerException: failedEventChatsViewServiceException);

            this.loggingBroker.LogError(eventChatsViewServiceException);

            return eventChatsViewServiceException;
        }
    }
}
