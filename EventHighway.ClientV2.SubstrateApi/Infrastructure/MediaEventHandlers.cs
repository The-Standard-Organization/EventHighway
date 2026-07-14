// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.ClientV2.Seed;
using EventHighway.ClientV2.SubstrateApi.Brokers.Loggings;
using EventHighway.ClientV2.SubstrateApi.Models.MediaItems;
using EventHighway.EventHandlers;
using EventHighway.EventHandlers.Delegates.JoesRestApi.Clients;
using Microsoft.Extensions.DependencyInjection;
using WireMock.Server;

namespace EventHighway.ClientV2.SubstrateApi.Infrastructure
{
    /// <summary>
    /// The handlers this app wires into the substrate. SofaBox logs; Joe and Ann forward each
    /// release to a REST API (here, the WireMock stand-in); FlakyBox always fails; and SubstrateApi
    /// — the one this app exists for — posts every release, unfiltered, to the /receive endpoint
    /// that feeds the chat UI.
    /// </summary>
    /// <remarks>
    /// Joe and SubstrateApi both deliver through the SAME packaged delegate client; they differ
    /// only in the configuration section each one reads. Joe's points at the WireMock stand-in,
    /// SubstrateApi's at a real localhost address — which is the whole trick: nothing about the
    /// delivery is special-cased for being local.
    /// </remarks>
    public sealed class MediaEventHandlers
    {
        public const string SubstrateApiDelegateClientKey = "SubstrateApi";

        public DelegateEventHandler SofaBox { get; }
        public DelegateEventHandler Joe { get; }
        public DelegateEventHandler Ann { get; }
        public DelegateEventHandler FlakyBox { get; }
        public DelegateEventHandler SubstrateApi { get; }

        public MediaEventHandlers(
            WireMockServer wireMock,
            IJoesRestApiDelegateClient joesRestApiDelegateClient,

            [FromKeyedServices(SubstrateApiDelegateClientKey)]
            IJoesRestApiDelegateClient substrateApiDelegateClient,

            ILoggingBroker loggingBroker)
        {
            this.SofaBox = new DelegateEventHandler(
                SeedIdentifiers.SofaBoxHandler,
                (content, cancellationToken) =>
                {
                    MediaItem item = MediaItemSerializer.Deserialize(content);

                    loggingBroker.LogInformation(
                        $"[SofaBox] New Release - {item.Title} " +
                        $"({item.Type} with rating of {item.Rating})");

                    return ValueTask.FromResult(new EventHandlerResult
                    {
                        IsSuccess = true,
                        Response = item.Title,
                        ResponseCode = "200",
                        ResponseMessage = "OK"
                    });
                },
                name: "SofaBox");

            // A downstream that is always unavailable. Used to seed partial-success events:
            // the reliable listeners succeed while this one errors, leaving a mix of statuses.
            this.FlakyBox = new DelegateEventHandler(
                SeedIdentifiers.FlakyBoxHandler,
                (content, cancellationToken) =>
                {
                    MediaItem item = MediaItemSerializer.Deserialize(content);

                    loggingBroker.LogInformation(
                        $"[FlakyBox] FAILED to deliver - {item.Title} " +
                        $"({item.Type} with rating of {item.Rating})");

                    return ValueTask.FromResult(new EventHandlerResult
                    {
                        IsSuccess = false,
                        Response = "downstream unavailable",
                        ResponseCode = "503",
                        ResponseMessage = "Service Unavailable"
                    });
                },
                name: "FlakyBox");

            // Joe's deliveries run through the referenced delegate client library — the
            // registered function IS the client's exposed method; identity stays here.
            this.Joe = new DelegateEventHandler(
                SeedIdentifiers.JoeHandler,
                joesRestApiDelegateClient.PostToJoesRestApiAsync,
                name: "Joe");

            // The chat's own line into the highway. Same handler Id in every app that registers it,
            // so a release dispatched by BasicApp or SubstrateApp lands on this same UI.
            this.SubstrateApi = new DelegateEventHandler(
                SeedIdentifiers.SubstrateApiHandler,
                substrateApiDelegateClient.PostToJoesRestApiAsync,
                name: "SubstrateApi");

            this.Ann = CreateRestHandler(
                SeedIdentifiers.AnnHandler, "Ann", wireMock, loggingBroker);
        }

        private static DelegateEventHandler CreateRestHandler(
            Guid handlerId,
            string label,
            WireMockServer wireMock,
            ILoggingBroker loggingBroker) =>
            new DelegateEventHandler(
                handlerId,
                async (content, cancellationToken) =>
                {
                    MediaItem item = MediaItemSerializer.Deserialize(content);
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

                    loggingBroker.LogInformation(
                        $"[{label}] New Release - {item.Title} " +
                        $"({item.Type} with rating of {item.Rating})");

                    return new EventHandlerResult
                    {
                        IsSuccess = response.IsSuccessStatusCode,
                        Response = responseBody,
                        ResponseCode = ((int)response.StatusCode).ToString(),
                        ResponseMessage = response.ReasonPhrase ?? string.Empty
                    };
                },
                name: label);
    }
}
