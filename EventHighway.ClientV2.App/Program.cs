// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.ClientV2.App.Brokers.EventSubstrates;
using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.EventHandlers;
using WireMock.Server;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        string connectionString = string.Concat(
            "Server=(localdb)\\MSSQLLocalDB;Database=EventHighwayDB;",
            "Trusted_Connection=True;MultipleActiveResultSets=true");

        DateTimeOffset now = DateTimeOffset.UtcNow;
        using WireMockServer wireMock = SetupWireMock();

        var delegateHandler1 = new DelegateEventHandler(
            Guid.NewGuid(),
            (content, _, cancellationToken) =>
            {
                int sum = content.Split(',')
                    .Select(p => int.TryParse(p.Trim(), out int n) ? n : 0)
                    .Sum();

                Console.WriteLine($"[Handler 1 - Sum Numbers] {content} => {sum}");

                return ValueTask.FromResult(new EventHandlerResult
                {
                    IsSuccess = true,
                    Response = sum.ToString(),
                    ResponseCode = "200",
                    ResponseMessage = "OK"
                });
            });

        var delegateHandler2 = new DelegateEventHandler(
            Guid.NewGuid(),
            (content, _, cancellationToken) =>
            {
                double[] scores = content.Split(',')
                    .Select(p => double.TryParse(p.Trim(), out double d) ? d : 0.0)
                    .ToArray();

                double average = scores.Length > 0 ? scores.Average() : 0;

                string grade =
                    average >= 90 ? "A" :
                    average >= 80 ? "B" :
                    average >= 70 ? "C" :
                    average >= 60 ? "D" : "F";

                string result = $"Average: {average:F1}, Grade: {grade}";
                Console.WriteLine($"[Handler 2 - Grade Calculator] {content} => {result}");

                return ValueTask.FromResult(new EventHandlerResult
                {
                    IsSuccess = true,
                    Response = result,
                    ResponseCode = "200",
                    ResponseMessage = "OK"
                });
            });

        var delegateHandler3 = new DelegateEventHandler(
            Guid.NewGuid(),
            async (content, _, cancellationToken) =>
            {
                string baseUrl = wireMock.Url ?? string.Empty;
                using var http = new HttpClient();

                var tokenPayload = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = "client",
                    ["client_secret"] = "secret",
                    ["scope"] = "enrollment",
                    ["grant_type"] = "client_credentials"
                });

                HttpResponseMessage tokenResponse =
                    await http.PostAsync($"{baseUrl}/token", tokenPayload, cancellationToken);

                string tokenJson =
                    await tokenResponse.Content.ReadAsStringAsync(cancellationToken);

                string token = JsonDocument.Parse(tokenJson)
                    .RootElement.GetProperty("access_token").GetString() ?? string.Empty;

                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var eventRequest = new StringContent(content, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await http.PostAsync($"{baseUrl}/events", eventRequest, cancellationToken);

                string responseBody =
                    await response.Content.ReadAsStringAsync(cancellationToken);

                Console.WriteLine(
                    $"[Handler 3 - REST via Delegate] {content} => {response.StatusCode} {responseBody}");

                return new EventHandlerResult
                {
                    IsSuccess = response.IsSuccessStatusCode,
                    Response = responseBody,
                    ResponseCode = ((int)response.StatusCode).ToString(),
                    ResponseMessage = response.ReasonPhrase ?? string.Empty
                };
            });

        var delegateHandler4 = new DelegateEventHandler(
            Guid.NewGuid(),
            (content, _, cancellationToken) =>
                PostWithBearerTokenAsync(content, wireMock.Url ?? string.Empty, cancellationToken));

        var restBearerHandler = new RestBearerEventHandler(Guid.NewGuid());

        // =========================================================
        // 1) Initialise the client and broker
        // =========================================================
        var client = new EventHighwayClient(connectionString);
        var broker = new EventSubstrateBroker(client);

        // =========================================================
        // 2) Register event handlers
        // =========================================================
        client.V2
            .RegisterEventHandler(delegateHandler1)
            .RegisterEventHandler(delegateHandler2)
            .RegisterEventHandler(delegateHandler3)
            .RegisterEventHandler(delegateHandler4)
            .RegisterEventHandler(restBearerHandler);

        // =========================================================
        // 3) Register Event Address 1: Student Enrolled
        // =========================================================

        await broker.RegisterStudentEnrolledAddressAsync(new EventAddressV2
        {
            Id = Guid.NewGuid(),
            Name = "Student Enrolled",
            Description = "Raised when a new student enrols in the system.",
            CreatedDate = now,
            UpdatedDate = now
        });

        // =========================================================
        // 4) Register Event Listeners for Event Address 1: Student Enrolled
        // =========================================================

        await broker.RegisterStudentEnrolledListenerAsync(new EventListenerV2
        {
            Id = Guid.NewGuid(),
            Name = "Sum Numbers Listener",
            Description = "Parses comma-separated numbers from the event content and sums them.",
            HandlerId = delegateHandler1.Id,
            HandlerName = delegateHandler1.Name,
            HandlerConfigurations = new List<HandlerConfiguration>(),
            CreatedDate = now,
            UpdatedDate = now
        });

        await broker.RegisterStudentEnrolledListenerAsync(new EventListenerV2
        {
            Id = Guid.NewGuid(),
            Name = "REST Notification Listener",
            Description = "Posts the event to an external REST endpoint using OAuth bearer auth.",
            HandlerId = restBearerHandler.Id,
            HandlerName = restBearerHandler.Name,
            HandlerConfigurations =
                CreateRestBearerConfigurations(wireMock.Url ?? string.Empty, scope: "enrollment", now),
            CreatedDate = now,
            UpdatedDate = now
        });

        await broker.RegisterStudentEnrolledListenerAsync(new EventListenerV2
        {
            Id = Guid.NewGuid(),
            Name = "REST via Delegate Listener",
            Description = "Replicates RestBearerEventHandler behaviour using a DelegateEventHandler.",
            HandlerId = delegateHandler3.Id,
            HandlerName = delegateHandler3.Name,
            HandlerConfigurations = new List<HandlerConfiguration>(),
            CreatedDate = now,
            UpdatedDate = now
        });

        // =========================================================
        // 5) Submit an event for Event Address 1: Student Enrolled
        // =========================================================

        EventV2 studentEvent =
            await broker.RaiseStudentEnrolledAsync("42,17,85");

        // =========================================================
        // 6) Register Event Address 2: Course Completed
        // =========================================================

        await broker.RegisterCourseCompletedAddressAsync(new EventAddressV2
        {
            Id = Guid.NewGuid(),
            Name = "Course Completed",
            Description = "Raised when a student completes a course.",
            CreatedDate = now,
            UpdatedDate = now
        });

        // =========================================================
        // 7) Register Event Listeners for Event Address 2: Course Completed
        // =========================================================

        await broker.RegisterCourseCompletedListenerAsync(new EventListenerV2
        {
            Id = Guid.NewGuid(),
            Name = "Failing REST Listener",
            Description = "Posts to a non-existent endpoint to demonstrate failure handling.",
            HandlerId = restBearerHandler.Id,
            HandlerName = restBearerHandler.Name,
            HandlerConfigurations =
                CreateRestBearerConfigurations("http://localhost:19999", scope: "courses", now),
            CreatedDate = now,
            UpdatedDate = now
        });

        await broker.RegisterCourseCompletedListenerAsync(new EventListenerV2
        {
            Id = Guid.NewGuid(),
            Name = "Grade Calculator Listener",
            Description = "Calculates the average score and letter grade from the event content.",
            HandlerId = delegateHandler2.Id,
            HandlerName = delegateHandler2.Name,
            HandlerConfigurations = new List<HandlerConfiguration>(),
            CreatedDate = now,
            UpdatedDate = now
        });

        await broker.RegisterCourseCompletedListenerAsync(new EventListenerV2
        {
            Id = Guid.NewGuid(),
            Name = "REST via Delegate Method Listener",
            Description = "Calls PostWithBearerTokenAsync on Program directly via a DelegateEventHandler.",
            HandlerId = delegateHandler4.Id,
            HandlerName = delegateHandler4.Name,
            HandlerConfigurations = new List<HandlerConfiguration>(),
            CreatedDate = now,
            UpdatedDate = now
        });

        // =========================================================
        // 8) Submit an event for Event Address 2: Course Completed
        // =========================================================

        EventV2 courseEvent =
            await broker.RaiseCourseCompletedAsync("88,92,75,95,83");

        // =========================================================
        // Fire all pending immediate events
        // =========================================================

        await broker.FirePendingEventsAsync();

        await PrintListenerEventResultsAsync(
            client,
            studentEvent.Id,
            courseEvent.Id);

        await PrintHealthSummaryAsync(client);
    }

    private static async ValueTask<EventHandlerResult> PostWithBearerTokenAsync(
        string content,
        string baseUrl,
        CancellationToken cancellationToken)
    {
        using var http = new HttpClient();

        var tokenPayload = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = "client",
            ["client_secret"] = "secret",
            ["scope"] = "courses",
            ["grant_type"] = "client_credentials"
        });

        HttpResponseMessage tokenResponse =
            await http.PostAsync($"{baseUrl}/token", tokenPayload, cancellationToken);

        string tokenJson =
            await tokenResponse.Content.ReadAsStringAsync(cancellationToken);

        string token = JsonDocument.Parse(tokenJson)
            .RootElement.GetProperty("access_token").GetString() ?? string.Empty;

        http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var eventRequest = new StringContent(content, Encoding.UTF8, "application/json");

        HttpResponseMessage response =
            await http.PostAsync($"{baseUrl}/events", eventRequest, cancellationToken);

        string responseBody =
            await response.Content.ReadAsStringAsync(cancellationToken);

        Console.WriteLine(
            $"[Handler 4 - REST via Delegate Method] {content} => {response.StatusCode} {responseBody}");

        return new EventHandlerResult
        {
            IsSuccess = response.IsSuccessStatusCode,
            Response = responseBody,
            ResponseCode = ((int)response.StatusCode).ToString(),
            ResponseMessage = response.ReasonPhrase ?? string.Empty
        };
    }

    private static async Task PrintHealthSummaryAsync(EventHighwayClient client)
    {
        IEnumerable<HealthCheckItemV2> summary =
            await client.V2.HealthV2Client.RetrieveHealthSummaryV2Async();

        Console.WriteLine("\n── Health Summary ──");

        string? currentGrouping = null;

        foreach (HealthCheckItemV2 item in summary)
        {
            if (item.Grouping != currentGrouping)
            {
                currentGrouping = item.Grouping;
                Console.WriteLine($"\n  {currentGrouping}");
            }

            Console.ForegroundColor = item.Status switch
            {
                nameof(HealthStatusV2.Green) => ConsoleColor.Green,
                nameof(HealthStatusV2.Amber) => ConsoleColor.Yellow,
                nameof(HealthStatusV2.Red) => ConsoleColor.Red,
                _ => ConsoleColor.Gray,
            };

            Console.WriteLine($"    [{item.Status,-6}] {item.Item}: {item.Value}");
            Console.ResetColor();
        }

        Console.WriteLine();
    }

    private static async Task PrintListenerEventResultsAsync(
        EventHighwayClient client,
        params Guid[] eventIds)
    {
        IQueryable<ListenerEventV2> all =
            await client.V2.ListenerEventV2Client.RetrieveAllListenerEventV2sAsync();

        foreach (Guid eventId in eventIds)
        {
            Console.WriteLine($"\n── Results for event {eventId} ──");

            foreach (ListenerEventV2 result in all.Where(le => le.EventId == eventId))
            {
                Console.WriteLine($"  Status   : {result.Status}");
                Console.WriteLine($"  Code     : {result.ResponseCode}");
                Console.WriteLine($"  Message  : {result.ResponseMessage}");
                Console.WriteLine($"  Response : {result.Response}");
                Console.WriteLine();
            }
        }
    }

    private static WireMockServer SetupWireMock()
    {
        var server = WireMockServer.Start();

        server
            .Given(WireMock.RequestBuilders.Request.Create().WithPath("/token").UsingPost())
            .RespondWith(WireMock.ResponseBuilders.Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"access_token\":\"demo-token\",\"token_type\":\"Bearer\",\"expires_in\":3600}"));

        server
            .Given(WireMock.RequestBuilders.Request.Create().WithPath("/events").UsingPost())
            .RespondWith(WireMock.ResponseBuilders.Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBody("Event received"));

        return server;
    }

    private static List<HandlerConfiguration> CreateRestBearerConfigurations(
        string baseUrl,
        string scope,
        DateTimeOffset now) =>
        new List<HandlerConfiguration>
        {
            new() { Id = Guid.NewGuid(), Name = "Url",
                Value = $"{baseUrl}/events", CreatedDate = now, UpdatedDate = now },
            new() { Id = Guid.NewGuid(), Name = "TokenUrl",
                Value = $"{baseUrl}/token", CreatedDate = now, UpdatedDate = now },
            new() { Id = Guid.NewGuid(), Name = "ClientId",
                Value = "client", CreatedDate = now, UpdatedDate = now },
            new() { Id = Guid.NewGuid(), Name = "ClientSecret",
                Value = "secret", CreatedDate = now, UpdatedDate = now },
            new() { Id = Guid.NewGuid(), Name = "Scope",
                Value = scope, CreatedDate = now, UpdatedDate = now },
            new() { Id = Guid.NewGuid(), Name = "GrantType",
                Value = "client_credentials", CreatedDate = now, UpdatedDate = now },
        };
}
