// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Portal.Web.Models.Services.Domains.Foundations.Roles;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventHighway.Portal.Web.Brokers.Storages
{
    // Design-time factory so `dotnet ef migrations` can construct the context without the
    // application's runtime DI wiring (keeps the DATA migration independent of EXPOSERS).
    // It mirrors the runtime Identity configuration — crucially SchemaVersion Version3 — so the
    // generated migration includes the passkey schema, matching what the app maps at runtime.
    public class SecurityDbContextFactory : IDesignTimeDbContextFactory<SecurityDbContext>
    {
        public SecurityDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration =
                new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

            string connectionString =
                configuration.GetConnectionString("EventHighwaySecurityConnection")!;

            var services = new ServiceCollection();

            services.AddDbContext<SecurityDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentityCore<AppUser>(options =>
                options.Stores.SchemaVersion = IdentitySchemaVersions.Version3)
                    .AddRoles<AppRole>()
                    .AddEntityFrameworkStores<SecurityDbContext>();

            ServiceProvider provider = services.BuildServiceProvider();

            return provider.GetRequiredService<SecurityDbContext>();
        }
    }
}
