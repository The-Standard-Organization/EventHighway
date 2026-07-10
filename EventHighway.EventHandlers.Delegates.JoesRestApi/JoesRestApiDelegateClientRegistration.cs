// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.EventHandlers.Delegates.JoesRestApi.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventHighway.EventHandlers.Delegates.JoesRestApi
{
    public static class JoesRestApiDelegateClientRegistration
    {
        public static IServiceCollection AddJoesRestApiDelegateClient(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IJoesRestApiDelegateClient>(
                _ => new JoesRestApiDelegateClient(configuration));

            return services;
        }
    }
}
