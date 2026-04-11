// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// GOOD EXAMPLE: Standard-compliant Storage Broker
// Demonstrates: local interface, partial class split, generic CRUD helpers in base partial,
// entity partials delegate to helpers only (no direct DbSet/DbContext access),
// Database.Migrate() in constructor, all methods async ValueTask.

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// ---------------------------------------------------------------
// StorageBroker.cs  — base partial
// Owns: IConfiguration, OnConfiguring, constructor + Migrate(), private CRUD helpers.
// Does NOT own: DbSet<> properties — each entity partial declares its own.
// ---------------------------------------------------------------

namespace MyProject.Brokers.Storages
{
    // arch-001: Implements a local interface
    // arch-004: Broker owns its configuration (IConfiguration injected here, not in entity partials)
    public partial class StorageBroker : EFxceptionsContext, IStorageBroker
    {
        // NO DbSet<> here. DbSet<Student> Students lives in StorageBroker.Students.cs.
        private readonly IConfiguration configuration;

        public StorageBroker(IConfiguration configuration)
        {
            this.configuration = configuration;

            // arch-004: Broker owns migration — Database.Migrate() runs at construction.
            // This must NOT be omitted; omitting it means the schema is never applied.
            this.Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString =
                this.configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);
        }

        // Generic CRUD helpers — entity partials NEVER call this.Entry(), this.Set<T>(),
        // this.FindAsync(), or this.SaveChangesAsync() directly. They use these helpers only.
        // This keeps entity partials decoupled from the concrete EF implementation.

        private async ValueTask<T> InsertAsync<T>(T entity) where T : class
        {
            this.Entry(entity).State = EntityState.Added;
            await this.SaveChangesAsync();

            return entity;
        }

        // SelectAllAsync<T>: wraps IQueryable in a ValueTask so the entity partial
        // can use the uniform `async/await` pattern across all operations.
        // AsNoTracking() is applied here — entity partials must not add it themselves.
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
// StorageBroker.Students.cs  — entity partial
// DbSet<Student> lives HERE, not in StorageBroker.cs.
// ---------------------------------------------------------------

namespace MyProject.Brokers.Storages
{
    public partial class StorageBroker
    {
        // arch-014: DbSet<Entity> is declared in the entity partial only.
        // WRONG: placing DbSet<Student> in StorageBroker.cs
        // CORRECT: DbSet<Student> Students lives in StorageBroker.Students.cs
        public DbSet<Student> Students { get; set; }

        // arch-006: Infrastructure language (Insert, not Add)
        // arch-009: Async ValueTask
        // arch-002: No flow control
        // arch-003: No exception handling — raw exceptions propagate to service
        public async ValueTask<Student> InsertStudentAsync(Student student) =>
            await this.InsertAsync(student);

        // arch-006: Infrastructure language (SelectAll, not RetrieveAll)
        // arch-009: Async ValueTask<IQueryable<T>> — NOT synchronous IQueryable<T>
        // WRONG:  public IQueryable<Student> SelectAllStudents() => this.Students.AsNoTracking();
        // WRONG:  public IQueryable<Student> SelectAllStudents() => this.SelectAll<Student>();
        // CORRECT: delegate to SelectAllAsync<T>() helper — do NOT touch this.Students directly
        public async ValueTask<IQueryable<Student>> SelectAllStudentsAsync() =>
            await this.SelectAllAsync<Student>();

        // arch-006: Infrastructure language (Select, not Retrieve)
        public async ValueTask<Student> SelectStudentByIdAsync(Guid studentId) =>
            await this.SelectAsync<Student>(studentId);

        // arch-006: Infrastructure language (Update, not Modify)
        public async ValueTask<Student> UpdateStudentAsync(Student student) =>
            await this.UpdateAsync(student);

        // arch-006: Infrastructure language (Delete, not Remove)
        public async ValueTask<Student> DeleteStudentAsync(Student student) =>
            await this.DeleteAsync(student);
    }
}

// ---------------------------------------------------------------
// IStorageBroker.Students.cs  — entity interface partial
// ---------------------------------------------------------------

namespace MyProject.Brokers.Storages
{
    // arch-001: Local interface — I{Resource}Broker
    public partial interface IStorageBroker
    {
        ValueTask<Student> InsertStudentAsync(Student student);
        ValueTask<IQueryable<Student>> SelectAllStudentsAsync();
        ValueTask<Student> SelectStudentByIdAsync(Guid studentId);
        ValueTask<Student> UpdateStudentAsync(Student student);
        ValueTask<Student> DeleteStudentAsync(Student student);
    }
}

// ---------------------------------------------------------------
// Program.cs — DI registration
// ---------------------------------------------------------------
//
// builder.Services.AddDbContext<StorageBroker>();
// builder.Services.AddTransient<IStorageBroker, StorageBroker>();
