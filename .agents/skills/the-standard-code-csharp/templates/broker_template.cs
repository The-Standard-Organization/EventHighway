// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Standard-compliant Broker
// Replace [Entity] / [Entities] / [Resource] / [Namespace] with actual values.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace [Namespace].Brokers.Storages
{
    // cs-062: Singular + Broker
    // StorageBroker partial — main DbContext file
    public partial class StorageBroker : DbContext, IStorageBroker
    {
        // cs-070: camelCase field
        private readonly IConfiguration configuration;

        public StorageBroker(IConfiguration configuration)
        {
            // cs-072: this.
            this.configuration = configuration;
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // cs-072: this.
            string connectionString =
                this.configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}

// ---------------------------------------------------------------
// StorageBroker.[Entity].cs — entity-specific partial
// ---------------------------------------------------------------

namespace [Namespace].Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<[Entity]> [Entities] { get; set; }

        // cs-040: Verb (Insert)
        // cs-041: Async
        // cs-042/cs-043: Parameter [entity], method Insert[Entity]Async
        // cs-050: One-liner → fat arrow
        public async ValueTask<[Entity]> Insert[Entity]Async([Entity] [entity]) =>
            await this.InsertAsync([entity]);

        // cs-050: One-liner → fat arrow
        public IQueryable<[Entity]> SelectAll[Entities]() =>
            this.SelectAll<[Entity]>();

        // cs-042/cs-043: Parameter [entity]Id, method Select[Entity]ByIdAsync
        public async ValueTask<[Entity]> Select[Entity]ByIdAsync(Guid [entity]Id) =>
            await this.SelectAsync<[Entity]>([entity]Id);

        public async ValueTask<[Entity]> Update[Entity]Async([Entity] [entity]) =>
            await this.UpdateAsync([entity]);

        public async ValueTask<[Entity]> Delete[Entity]Async([Entity] [entity]) =>
            await this.DeleteAsync([entity]);
    }
}

// ---------------------------------------------------------------
// IStorageBroker.cs — partial interface
// ---------------------------------------------------------------

namespace [Namespace].Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<[Entity]> Insert[Entity]Async([Entity] [entity]);
        IQueryable<[Entity]> SelectAll[Entities]();
        ValueTask<[Entity]> Select[Entity]ByIdAsync(Guid [entity]Id);
        ValueTask<[Entity]> Update[Entity]Async([Entity] [entity]);
        ValueTask<[Entity]> Delete[Entity]Async([Entity] [entity]);
    }
}
