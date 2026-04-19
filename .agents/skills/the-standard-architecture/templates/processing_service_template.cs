// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Standard-compliant Processing Service
// Replace [Entity] with entity name (e.g., Student)
// Replace [Namespace] with your project namespace

using System;
using System.Linq;
using System.Threading.Tasks;
using Xeptions;

namespace [Namespace].Services.Processings.[Entities]
{
    // [Entity]ProcessingService.cs — root file
    public partial class [Entity]ProcessingService : I[Entity]ProcessingService
    {
        private readonly I[Entity]Service [entity]Service;
        private readonly ILoggingBroker loggingBroker;

        public [Entity]ProcessingService(
            I[Entity]Service [entity]Service,
            ILoggingBroker loggingBroker)
        {
            this.[entity]Service = [entity]Service;
            this.loggingBroker = loggingBroker;
        }

        // Higher-order logic: EnsureExists
        public ValueTask<[Entity]> Ensure[Entity]ExistsAsync([Entity] [entity]) =>
            TryCatch(async () =>
            {
                Validate[Entity]OnEnsure([entity]);

                IQueryable<[Entity]> all[Entities] = this.[entity]Service.RetrieveAll[Entities]();

                bool is[Entity]Exists = all[Entities].Any(retrieved[Entity] =>
                    retrieved[Entity].Id == [entity].Id);

                return is[Entity]Exists switch
                {
                    false => await this.[entity]Service.Add[Entity]Async([entity]),
                    _ => await this.[entity]Service.Retrieve[Entity]ByIdAsync([entity].Id)
                };
            });

        // Higher-order logic: Upsert
        public ValueTask<[Entity]> Upsert[Entity]Async([Entity] [entity]) =>
            TryCatch(async () =>
            {
                Validate[Entity]OnUpsert([entity]);

                IQueryable<[Entity]> all[Entities] = this.[entity]Service.RetrieveAll[Entities]();

                bool is[Entity]Exists = all[Entities].Any(retrieved[Entity] =>
                    retrieved[Entity].Id == [entity].Id);

                return is[Entity]Exists switch
                {
                    true => await this.[entity]Service.Modify[Entity]Async([entity]),
                    false => await this.[entity]Service.Add[Entity]Async([entity])
                };
            });

        // Shifter: [Entity] → bool
        public ValueTask<bool> Verify[Entity]ExistsAsync(Guid [entity]Id) =>
            TryCatch(async () =>
            {
                Validate[Entity]Id([entity]Id);

                IQueryable<[Entity]> all[Entities] = this.[entity]Service.RetrieveAll[Entities]();

                return all[Entities].Any([entity] => [entity].Id == [entity]Id);
            });

        // Pass-through (no processing logic)
        public ValueTask<[Entity]> Retrieve[Entity]ByIdAsync(Guid [entity]Id) =>
            this.[entity]Service.Retrieve[Entity]ByIdAsync([entity]Id);

        // Pass-through (no processing logic)
        public ValueTask<[Entity]> Remove[Entity]ByIdAsync(Guid [entity]Id) =>
            this.[entity]Service.Remove[Entity]ByIdAsync([entity]Id);
    }
}

// ---------------------------------------------------------------
// [Entity]ProcessingService.Exceptions.cs
// ---------------------------------------------------------------

namespace [Namespace].Services.Processings.[Entities]
{
    public partial class [Entity]ProcessingService
    {
        private delegate ValueTask<[Entity]> Returning[Entity]Function();
        private delegate ValueTask<bool> ReturningBoolFunction();

        private async ValueTask<[Entity]> TryCatch(Returning[Entity]Function returning[Entity]Function)
        {
            try
            {
                return await returning[Entity]Function();
            }
            catch ([Entity]ValidationException [entity]ValidationException)
            {
                throw CreateAndLogValidationException([entity]ValidationException.InnerException as Xeption);
            }
            catch ([Entity]DependencyValidationException [entity]DependencyValidationException)
            {
                throw CreateAndLogDependencyValidationException(
                    [entity]DependencyValidationException.InnerException as Xeption);
            }
            catch ([Entity]DependencyException [entity]DependencyException)
            {
                throw CreateAndLogDependencyException([entity]DependencyException.InnerException as Xeption);
            }
            catch ([Entity]ServiceException [entity]ServiceException)
            {
                throw CreateAndLogServiceException([entity]ServiceException.InnerException as Xeption);
            }
            catch (Exception serviceException)
            {
                var failed[Entity]ProcessingServiceException =
                    new Failed[Entity]ProcessingServiceException(
                        message: "Failed [entity] processing service error occurred, contact support.",
                        innerException: serviceException);

                throw CreateAndLogServiceException(failed[Entity]ProcessingServiceException);
            }
        }

        private [Entity]ValidationException CreateAndLogValidationException(Xeption exception)
        {
            var [entity]ValidationException =
                new [Entity]ValidationException(
                    message: "[Entity] validation error occurred, fix the errors and try again.",
                    innerException: exception);

            this.loggingBroker.LogError([entity]ValidationException);

            return [entity]ValidationException;
        }

        private [Entity]DependencyValidationException CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var [entity]DependencyValidationException =
                new [Entity]DependencyValidationException(
                    message: "[Entity] dependency validation error occurred, fix the errors.",
                    innerException: exception);

            this.loggingBroker.LogError([entity]DependencyValidationException);

            return [entity]DependencyValidationException;
        }

        private [Entity]DependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var [entity]DependencyException =
                new [Entity]DependencyException(
                    message: "[Entity] dependency error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogError([entity]DependencyException);

            return [entity]DependencyException;
        }

        private [Entity]ServiceException CreateAndLogServiceException(Xeption exception)
        {
            var [entity]ServiceException =
                new [Entity]ServiceException(
                    message: "[Entity] service error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogError([entity]ServiceException);

            return [entity]ServiceException;
        }
    }
}
