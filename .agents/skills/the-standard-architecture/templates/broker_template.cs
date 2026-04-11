// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Standard-compliant Storage Broker
// Replace [Entity]   with the entity name, PascalCase  (e.g., Student)
// Replace [Entities] with the plural entity name       (e.g., Students)
// Replace [entity]   with the entity name, camelCase   (e.g., student)
// Replace [Namespace] with your project namespace

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// ---------------------------------------------------------------
// StorageBroker.cs  — base partial
// Owns: configuration, OnConfiguring, constructor + Migrate(), generic CRUD helpers.
// Does NOT own: DbSet<> properties — those live in entity partials.
// ---------------------------------------------------------------

namespace [Namespace].Brokers.Storages
{
    public partial class StorageBroker : EFxceptionsContext, IStorageBroker
    {
        // NO DbSet<> properties here. Each entity partial declares its own DbSet.
        private readonly IConfiguration configuration;

        public StorageBroker(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString =
                this.configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);
        }

        // Generic helpers — used by entity partials; never exposed on the interface
        private async ValueTask<T> InsertAsync<T>(T entity) where T : class
        {
            this.Entry(entity).State = EntityState.Added;
            await this.SaveChangesAsync();

            return entity;
        }

        private ValueTask<IQueryable<T>> SelectAllAsync<T>() where T : class =>
            ValueTask.FromResult<IQueryable<T>>(this.Set<T>().AsNoTracking());

        private async ValueTask<T> SelectAsync<T>(Guid entityId) where T : class =>
            await this.FindAsync<T>(entityId);

        private async ValueTask<T> UpdateAsync<T>(T entity) where T : class
        {
            this.Entry(entity).State = EntityState.Modified;
            await this.SaveChangesAsync();

            return entity;
        }

        private async ValueTask<T> DeleteAsync<T>(T entity) where T : class
        {
            this.Entry(entity).State = EntityState.Deleted;
            await this.SaveChangesAsync();

            return entity;
        }
    }
}

// ---------------------------------------------------------------
// StorageBroker.[Entities].cs  — entity partial
// One file per entity. DbSet<> lives here, not in StorageBroker.cs.
// Delegates to generic helpers only — no direct EF calls.
// ---------------------------------------------------------------

namespace [Namespace].Brokers.Storages
{
    public partial class StorageBroker
    {
        // DbSet<> is declared in the entity partial, NOT in StorageBroker.cs.
        public DbSet<[Entity]> [Entities] { get; set; }

        public async ValueTask<[Entity]> Insert[Entity]Async([Entity] [entity]) =>
            await this.InsertAsync([entity]);

        public async ValueTask<IQueryable<[Entity]>> SelectAll[Entities]Async() =>
            await this.SelectAllAsync<[Entity]>();

        public async ValueTask<[Entity]> Select[Entity]ByIdAsync(Guid [entity]Id) =>
            await this.SelectAsync<[Entity]>([entity]Id);

        public async ValueTask<[Entity]> Update[Entity]Async([Entity] [entity]) =>
            await this.UpdateAsync([entity]);

        public async ValueTask<[Entity]> Delete[Entity]Async([Entity] [entity]) =>
            await this.DeleteAsync([entity]);
    }
}

// ---------------------------------------------------------------
// IStorageBroker.cs  — base interface partial
// ---------------------------------------------------------------

namespace [Namespace].Brokers.Storages
{
    public partial interface IStorageBroker { }
}

// ---------------------------------------------------------------
// IStorageBroker.[Entities].cs  — entity interface partial
// ---------------------------------------------------------------

namespace [Namespace].Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<[Entity]> Insert[Entity]Async([Entity] [entity]);
        ValueTask<IQueryable<[Entity]>> SelectAll[Entities]Async();
        ValueTask<[Entity]> Select[Entity]ByIdAsync(Guid [entity]Id);
        ValueTask<[Entity]> Update[Entity]Async([Entity] [entity]);
        ValueTask<[Entity]> Delete[Entity]Async([Entity] [entity]);
    }
}
