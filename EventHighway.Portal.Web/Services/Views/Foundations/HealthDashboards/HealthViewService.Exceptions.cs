// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.HealthDashboards;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.HealthDashboards.Exceptions;
using Xeptions;

namespace EventHighway.Portal.Web.Services.Views.Foundations.HealthDashboards
{
    public partial class HealthViewService
    {
        private delegate ValueTask<T> ReturningHealthFunction<T>();

        private async ValueTask<T> TryCatch<T>(
            ReturningHealthFunction<T> returningHealthFunction)
        {
            try
            {
                return await returningHealthFunction();
            }
            catch (HealthStatusClientV2ValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(clientValidationException);
            }
            catch (HealthTrafficClientV2ValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(clientValidationException);
            }
            catch (HealthAddressClientV2ValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(clientValidationException);
            }
            catch (HealthLoopClientV2ValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(clientValidationException);
            }
            catch (HealthDuplicateClientV2ValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(clientValidationException);
            }
            catch (HealthRetryClientV2ValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(clientValidationException);
            }
            catch (HealthParticipantClientV2ValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(clientValidationException);
            }
            catch (HealthStatusClientV2DependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (HealthTrafficClientV2DependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (HealthAddressClientV2DependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (HealthLoopClientV2DependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (HealthDuplicateClientV2DependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (HealthRetryClientV2DependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (HealthParticipantClientV2DependencyException clientDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientDependencyException);
            }
            catch (HealthStatusClientV2ServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (HealthTrafficClientV2ServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (HealthAddressClientV2ServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (HealthLoopClientV2ServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (HealthDuplicateClientV2ServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (HealthRetryClientV2ServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (HealthParticipantClientV2ServiceException clientServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(clientServiceException);
            }
            catch (Exception exception)
            {
                var failedHealthViewServiceException =
                    new FailedHealthViewServiceException(innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(
                    failedHealthViewServiceException);
            }
        }

        private async ValueTask<HealthViewDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var healthViewDependencyValidationException =
                new HealthViewDependencyValidationException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(healthViewDependencyValidationException);

            return healthViewDependencyValidationException;
        }

        private async ValueTask<HealthViewDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var healthViewDependencyException =
                new HealthViewDependencyException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(healthViewDependencyException);

            return healthViewDependencyException;
        }

        private async ValueTask<HealthViewServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var healthViewServiceException =
                new HealthViewServiceException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(healthViewServiceException);

            return healthViewServiceException;
        }
    }
}
