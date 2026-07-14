// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi;
using EventHighway.ClientV2.SubstrateApi.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        // A rating rides the highway as a JSON string ("8.6") so it can be promoted and filtered
        // on. Accept it either way at the door, so both the chat box and a hand-written Postman
        // body with a plain number get through.
        options.JsonSerializerOptions.NumberHandling =
            JsonNumberHandling.AllowReadingFromString);

builder.Services.AddSubstrateApi(builder.Configuration);

WebApplication app = builder.Build();

await SetupSubstrateAsync(app);

// Plain HTTP, and no redirect to https: the address the highway delivers to is an ordinary
// localhost url that three separate processes have to agree on. Sending them to https would only
// give them a development certificate to argue about.
app.UseAntiforgery();
app.MapStaticAssets();
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

// LocalDB is prone to a cold-start stumble on the first connection of the day. The setup is
// idempotent, so it costs nothing to try again — and the app is useless without it, so a final
// failure is worth stopping for rather than serving a chat nothing can reach.
static async Task SetupSubstrateAsync(WebApplication app)
{
    const int maximumAttempts = 5;

    for (int attempt = 1; attempt <= maximumAttempts; attempt++)
    {
        try
        {
            await app.Services.UseSubstrateAsync();

            return;
        }
        catch (Exception exception) when (attempt < maximumAttempts)
        {
            app.Logger.LogWarning(
                exception,
                "Substrate setup attempt {Attempt}/{MaximumAttempts} failed; retrying.",
                attempt,
                maximumAttempts);

            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}
