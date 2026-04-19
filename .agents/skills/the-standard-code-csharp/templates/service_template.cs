// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Standard-compliant Service file skeleton
// Replace [Entity] / [Entities] / [Namespace] with actual values.
// e.g., [Entity]=Student, [Entities]=Students, [Namespace]=MyProject

using System;
using System.Linq;
using System.Threading.Tasks;
using Xeptions;

namespace [Namespace].Services.Foundations.[Entities]
{
    // cs-061: Singular + Service
    public partial class [Entity]Service : I[Entity]Service
    {
        // cs-070: camelCase fields
        // cs-072: will be referenced with this.
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        // cs-057/cs-058: Constructor params broken onto next line when > 120 chars
        public [Entity]Service(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker)
        {
            // cs-072: this. reference
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        // cs-040: Verb (Add)
        // cs-041: Async suffix
        public ValueTask<[Entity]> Add[Entity]Async([Entity] [entity]) =>
            TryCatch(async () =>
            {
                Validate[Entity]OnAdd([entity]);

                return await this.storageBroker.Insert[Entity]Async([entity]);
            });

        // cs-050: One-liner → fat arrow (method body is a single expression)
        public ValueTask<IQueryable<[Entity]>> RetrieveAll[Entities]() =>
            TryCatch(() =>
            {
                IQueryable<[Entity]> all[Entities] =
                    this.storageBroker.SelectAll[Entities]();

                return ValueTask.FromResult(all[Entities]);
            });

        // cs-042/cs-043: Parameter [entity]Id, method ByIdAsync
        public ValueTask<[Entity]> Retrieve[Entity]ByIdAsync(Guid [entity]Id) =>
            TryCatch(async () =>
            {
                Validate[Entity]Id([entity]Id);

                [Entity] maybe[Entity] =
                    await this.storageBroker.Select[Entity]ByIdAsync([entity]Id);

                ValidateStorage[Entity](maybe[Entity], [entity]Id);

                return maybe[Entity];
            });

        public ValueTask<[Entity]> Modify[Entity]Async([Entity] [entity]) =>
            TryCatch(async () =>
            {
                Validate[Entity]OnModify([entity]);

                [Entity] maybe[Entity] =
                    await this.storageBroker.Select[Entity]ByIdAsync([entity].Id);

                ValidateStorage[Entity](maybe[Entity], [entity].Id);
                ValidateAgainstStorage[Entity]OnModify([entity], maybe[Entity]);

                return await this.storageBroker.Update[Entity]Async([entity]);
            });

        public ValueTask<[Entity]> Remove[Entity]ByIdAsync(Guid [entity]Id) =>
            TryCatch(async () =>
            {
                Validate[Entity]Id([entity]Id);

                [Entity] maybe[Entity] =
                    await this.storageBroker.Select[Entity]ByIdAsync([entity]Id);

                ValidateStorage[Entity](maybe[Entity], [entity]Id);

                return await this.storageBroker.Delete[Entity]Async(maybe[Entity]);
            });
    }
}
