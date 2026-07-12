// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ReplayingEvents.V2.Exceptions;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Replays.Exceptions;
using Xeptions;

namespace EventHighway.Portal.Web.Services.Views.Foundations.Replays
{
    public partial class ReplayViewService
    {
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (ReplayingEventV2ClientValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    clientValidationException);
            }
            catch (ReplayingEventV2ClientDependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (ReplayingEventV2ClientServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedReplaysViewServiceException(innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceException);
            }
        }

        private async ValueTask<ReplaysViewDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var dependencyValidationException =
                new ReplaysViewDependencyValidationException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(dependencyValidationException);

            return dependencyValidationException;
        }

        private async ValueTask<ReplaysViewDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var dependencyException =
                new ReplaysViewDependencyException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(dependencyException);

            return dependencyException;
        }

        private async ValueTask<ReplaysViewServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var serviceException =
                new ReplaysViewServiceException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(serviceException);

            return serviceException;
        }
    }
}
