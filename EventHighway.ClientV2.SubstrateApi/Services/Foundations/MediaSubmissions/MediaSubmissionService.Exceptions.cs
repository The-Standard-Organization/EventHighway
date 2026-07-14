// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Models.MediaSubmissions;
using EventHighway.ClientV2.SubstrateApi.Models.MediaSubmissions.Exceptions;
using Xeptions;

namespace EventHighway.ClientV2.SubstrateApi.Services.Foundations.MediaSubmissions
{
    public partial class MediaSubmissionService
    {
        private delegate ValueTask<MediaSubmission> ReturningMediaSubmissionFunction();
        private delegate ValueTask<MediaSubmissionEndpoint> ReturningMediaSubmissionEndpointFunction();

        private async ValueTask<MediaSubmission> TryCatch(
            ReturningMediaSubmissionFunction returningMediaSubmissionFunction)
        {
            try
            {
                return await returningMediaSubmissionFunction();
            }
            catch (InvalidMediaSubmissionException invalidMediaSubmissionException)
            {
                throw CreateAndLogValidationException(invalidMediaSubmissionException);
            }
            catch (HttpRequestException httpRequestException)
            {
                var failedMediaSubmissionDependencyException =
                    new FailedMediaSubmissionDependencyException(
                        message: "Failed media submission dependency error occurred, contact support.",
                        innerException: httpRequestException);

                throw CreateAndLogDependencyException(failedMediaSubmissionDependencyException);
            }
            catch (Exception exception)
            {
                var failedMediaSubmissionServiceException =
                    new FailedMediaSubmissionServiceException(
                        message: "Failed media submission service error occurred, contact support.",
                        innerException: exception);

                throw CreateAndLogServiceException(failedMediaSubmissionServiceException);
            }
        }

        private async ValueTask<MediaSubmissionEndpoint> TryCatch(
            ReturningMediaSubmissionEndpointFunction returningMediaSubmissionEndpointFunction)
        {
            try
            {
                return await returningMediaSubmissionEndpointFunction();
            }
            catch (InvalidMediaSubmissionException invalidMediaSubmissionException)
            {
                throw CreateAndLogValidationException(invalidMediaSubmissionException);
            }
            catch (Exception exception)
            {
                var failedMediaSubmissionServiceException =
                    new FailedMediaSubmissionServiceException(
                        message: "Failed media submission service error occurred, contact support.",
                        innerException: exception);

                throw CreateAndLogServiceException(failedMediaSubmissionServiceException);
            }
        }

        private MediaSubmissionValidationException CreateAndLogValidationException(Xeption exception)
        {
            var mediaSubmissionValidationException =
                new MediaSubmissionValidationException(
                    message: "Media submission validation error occurred, fix the errors and try again.",
                    innerException: exception);

            this.loggingBroker.LogError(mediaSubmissionValidationException);

            return mediaSubmissionValidationException;
        }

        private MediaSubmissionDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var mediaSubmissionDependencyException =
                new MediaSubmissionDependencyException(
                    message: "Media submission dependency error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogError(mediaSubmissionDependencyException);

            return mediaSubmissionDependencyException;
        }

        private MediaSubmissionServiceException CreateAndLogServiceException(Xeption exception)
        {
            var mediaSubmissionServiceException =
                new MediaSubmissionServiceException(
                    message: "Media submission service error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogError(mediaSubmissionServiceException);

            return mediaSubmissionServiceException;
        }
    }
}
