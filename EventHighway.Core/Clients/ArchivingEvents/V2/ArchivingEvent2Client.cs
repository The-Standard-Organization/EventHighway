// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2;
using Xeptions;

namespace EventHighway.Core.Clients.ArchivingEvents.V2
{
    internal class ArchivingEvent2Client : IArchivingEvent2Client
    {
        private readonly IArchivingEvent2OrchestrationService archivingEvent2OrchestrationService;

        public ArchivingEvent2Client(IArchivingEvent2OrchestrationService archivingEvent2OrchestrationService) =>
            this.archivingEvent2OrchestrationService = archivingEvent2OrchestrationService;

        public async ValueTask<IQueryable<EventV2>> RetrieveAllDeadEventV2sWithListenersAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.archivingEvent2OrchestrationService
                    .RetrieveAllDeadEventV2sWithListenersAsync(cancellationToken);
            }
            catch (ArchivingEvent2OrchestrationValidationException
                archivingEvent2OrchestrationValidationException)
            {
                throw CreateArchivingEvent2ClientValidationException(
                    archivingEvent2OrchestrationValidationException.InnerException as Xeption);
            }
            catch (ArchivingEvent2OrchestrationDependencyValidationException
                archivingEvent2OrchestrationDependencyValidationException)
            {
                throw CreateArchivingEvent2ClientValidationException(
                    archivingEvent2OrchestrationDependencyValidationException.InnerException as Xeption);
            }
            catch (ArchivingEvent2OrchestrationDependencyException
                archivingEvent2OrchestrationDependencyException)
            {
                throw CreateArchivingEvent2ClientDependencyException(
                    archivingEvent2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (ArchivingEvent2OrchestrationServiceException
                archivingEvent2OrchestrationServiceException)
            {
                throw CreateArchivingEvent2ClientDependencyException(
                    archivingEvent2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateArchivingEvent2ClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask RemoveEventV2AndListenerEventV2sAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await this.archivingEvent2OrchestrationService
                    .RemoveEventV2AndListenerEventV2sAsync(eventV2, cancellationToken);
            }
            catch (ArchivingEvent2OrchestrationValidationException
                archivingEvent2OrchestrationValidationException)
            {
                throw CreateArchivingEvent2ClientValidationException(
                    archivingEvent2OrchestrationValidationException.InnerException as Xeption);
            }
            catch (ArchivingEvent2OrchestrationDependencyValidationException
                archivingEvent2OrchestrationDependencyValidationException)
            {
                throw CreateArchivingEvent2ClientValidationException(
                    archivingEvent2OrchestrationDependencyValidationException.InnerException as Xeption);
            }
            catch (ArchivingEvent2OrchestrationDependencyException
                archivingEvent2OrchestrationDependencyException)
            {
                throw CreateArchivingEvent2ClientDependencyException(
                    archivingEvent2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (ArchivingEvent2OrchestrationServiceException
                archivingEvent2OrchestrationServiceException)
            {
                throw CreateArchivingEvent2ClientDependencyException(
                    archivingEvent2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateArchivingEvent2ClientServiceException(exception as Xeption);
            }
        }

        private static ArchivingEvent2ClientValidationException
            CreateArchivingEvent2ClientValidationException(Xeption innerException)
        {
            return new ArchivingEvent2ClientValidationException(
                message: "Archiving event client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException.Data);
        }

        private static ArchivingEvent2ClientDependencyException
            CreateArchivingEvent2ClientDependencyException(Xeption innerException)
        {
            return new ArchivingEvent2ClientDependencyException(
                message: "Archiving event client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException.Data);
        }

        private static ArchivingEvent2ClientServiceException
            CreateArchivingEvent2ClientServiceException(Xeption innerException)
        {
            return new ArchivingEvent2ClientServiceException(
                message: "Archiving event client service error occurred, contact support.",
                innerException: innerException,
                data: innerException.Data);
        }
    }
}
