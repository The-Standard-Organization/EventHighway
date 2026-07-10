// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Delegates.JoesRestApi.Brokers.Apis;
using EventHighway.EventHandlers.Delegates.JoesRestApi.Brokers.Configurations;
using EventHighway.EventHandlers.Delegates.JoesRestApi.Models.Brokers.Configurations;

namespace EventHighway.EventHandlers.Delegates.JoesRestApi.Services.Foundations.EventPosts
{
    internal partial class EventPostService : IEventPostService
    {
        private readonly IConfigurationBroker configurationBroker;
        private readonly IApiBroker apiBroker;

        public EventPostService(
            IConfigurationBroker configurationBroker,
            IApiBroker apiBroker)
        {
            this.configurationBroker = configurationBroker;
            this.apiBroker = apiBroker;
        }

        public ValueTask<EventHandlerResult> PostEventAsync(
            string content,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateContent(content);

            JoesRestApiConfigurations configurations =
                this.configurationBroker.GetJoesRestApiConfigurations();

            ValidateConfigurations(configurations);

            HttpResponseMessage httpResponseMessage =
                await this.apiBroker.PostEventAsync(
                    url: configurations.Url,
                    secret: configurations.Secret,
                    content: content,
                    cancellationToken: cancellationToken);

            string responseBody =
                await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

            return new EventHandlerResult
            {
                IsSuccess = httpResponseMessage.IsSuccessStatusCode,
                Response = responseBody,
                ResponseCode = ((int)httpResponseMessage.StatusCode).ToString(),
                ResponseMessage = httpResponseMessage.ReasonPhrase ?? string.Empty
            };
        });
    }
}
