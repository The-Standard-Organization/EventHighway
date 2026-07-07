// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Clients.EventHighways.V2;
using EventHighway.Core.Models.Configurations;
using EventHighway.Portal.Web.Brokers.DateTimes;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Identities;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Components.Account;
using EventHighway.Portal.Web.Data;
using EventHighway.Portal.Web.Models.Foundations.Roles;
using EventHighway.Portal.Web.Models.Foundations.Users;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EventHighway.Portal.Web.Services.Views.EventAddresses;
using EventHighway.Portal.Web.Services.Views.EventListeners;
using EventHighway.Portal.Web.Services.Views.EventParticipants;
using EventHighway.Portal.Web.Services.Views.EventParticipantSecrets;
using EventHighway.Portal.Web.Services.Views.EventArchives;
using EventHighway.Portal.Web.Services.Views.Events;
using EventHighway.Portal.Web.Services.Views.HealthDashboards;
using EventHighway.Portal.Web.Services.Views.ListenerEventArchives;
using EventHighway.Portal.Web.Services.Views.ListenerEvents;
using EventHighway.Portal.Web.Services.Views.Replays;
using EventHighway.Portal.Web.Services.Views.Users;

namespace EventHighway.Portal.Web.Infrastructure
{
    public static class PortalRegistration
    {
        // LocalDB "instance startup" transient (0x89c5010a) is not in EF Core's default
        // transient-error set, so add it explicitly to the Identity DbContext retry policy.
        private static readonly int[] LocalDbTransientErrorNumbers = new[] { -1983577846 };

        public static IServiceCollection AddPortalBrokers(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Serialized single construction (no migration race) that retries after a failed first
            // attempt (recovers from a LocalDB cold start without an app restart). See ClientV2Provider.
            services.AddSingleton(_ => new ClientV2Provider(() => CreateClientV2(configuration)));

            services.AddSingleton<IEventHighwayBroker, EventHighwayBroker>();
            services.AddTransient<IDateTimeBroker, DateTimeBroker>();
            services.AddTransient<ILoggingBroker, LoggingBroker>();

            return services;
        }

        public static IServiceCollection AddPortalIdentity(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            string securityConnectionString =
                configuration.GetConnectionString("EventHighwaySecurityConnection")
                    ?? throw new InvalidOperationException(
                        "Missing connection string 'EventHighwaySecurityConnection'.");

            services.AddCascadingAuthenticationState();
            services.AddScoped<IdentityRedirectManager>();

            services.AddScoped<
                AuthenticationStateProvider,
                IdentityRevalidatingAuthenticationStateProvider>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
                .AddIdentityCookies();

            services.AddDbContext<SecurityDbContext>(options =>
                options.UseSqlServer(
                    securityConnectionString,
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(3),
                        errorNumbersToAdd: LocalDbTransientErrorNumbers)));

            services.AddIdentityCore<AppUser>(options =>
            {
                // Schema Version3 adds the passkey store the Manage > Passkeys page requires
                // (without it the page throws because IdentityUserPasskey is not in the model).
                options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;

                // Default credentials are intentionally weak for first-run/demo (Spec Section 6.3).
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 1;
            })
                .AddRoles<AppRole>()
                .AddEntityFrameworkStores<SecurityDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services.AddSingleton<IEmailSender<AppUser>, IdentityNoOpEmailSender>();

            services.AddTransient<IIdentityBroker, IdentityBroker>();

            return services;
        }

        public static IServiceCollection AddPortalViewServices(
            this IServiceCollection services)
        {
            services.AddTransient<IHealthViewService, HealthViewService>();
            services.AddTransient<IEventParticipantsViewService, EventParticipantsViewService>();
            services.AddTransient<
                IEventParticipantSecretsViewService,
                EventParticipantSecretsViewService>();
            services.AddTransient<IEventAddressesViewService, EventAddressesViewService>();
            services.AddTransient<IEventListenersViewService, EventListenersViewService>();
            services.AddTransient<IListenerEventsViewService, ListenerEventsViewService>();
            services.AddTransient<
                IListenerEventArchivesViewService,
                ListenerEventArchivesViewService>();
            services.AddTransient<IEventsViewService, EventsViewService>();
            services.AddTransient<IEventArchivesViewService, EventArchivesViewService>();
            services.AddTransient<IReplayViewService, ReplayViewService>();
            services.AddTransient<IUsersViewService, UsersViewService>();

            return services;
        }

        // The EventHighway V2 client is constructed once (it builds its own in-process
        // service provider and migrates the Core database) and registered as a singleton,
        // mirroring SubstrateAppRegistration.CreateEventSubstrateBroker.
        private static IClientV2 CreateClientV2(IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("EventHighwayCoreConnection")
                    ?? throw new InvalidOperationException(
                        "Missing connection string 'EventHighwayCoreConnection'.");

            var eventHighwayConfiguration = new EventHighwayConfiguration();

            var eventHighwayClient =
                new EventHighwayClient(connectionString, eventHighwayConfiguration);

            return eventHighwayClient.V2;
        }
    }
}
