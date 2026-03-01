// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Net.Http;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Apis;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;

namespace EventHighway.Core.Services.Foundations.EventCalls.V1
{
    internal partial class EventCallV1Service : IEventCallV1Service
    {
        private readonly IApiBroker apiBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventCallV1Service(
            IApiBroker apiBroker,
            ILoggingBroker loggingBroker)
        {
            this.apiBroker = apiBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventCallV1> RunEventCallV1Async(EventCallV1 eventCallV1) =>
        TryCatch(async () =>
        {
            ValidateEventCallV1OnRun(eventCallV1);

            string response =
                await apiBroker.PostAsync(
                    content: eventCallV1.Content,
                    url: eventCallV1.Endpoint,
                    secret: eventCallV1.Secret);

            eventCallV1.Response = response;

            return eventCallV1;
        });

        public ValueTask<EventCallV1> RunEventCallV1AsyncV1(EventCallV1 eventCallV1) =>
        TryCatch(async () =>
        {
            ValidateEventCallV1OnRun(eventCallV1);

            HttpResponseMessage httpResponseMessage =
                await apiBroker.PostAsyncV1(
                    content: eventCallV1.Content,
                    url: eventCallV1.Endpoint,
                    secret: eventCallV1.Secret);

            ValidateHttpResponseMessageIsNotNull(httpResponseMessage);
            await MapToEventCallV1Async(eventCallV1, httpResponseMessage);

            return eventCallV1;
        });

        private static async ValueTask MapToEventCallV1Async(
            EventCallV1 eventCallV1,
            HttpResponseMessage httpResponseMessage)
        {
            eventCallV1.Response =
                await httpResponseMessage.Content
                    .ReadAsStringAsync();

            eventCallV1.ResponseReasonPhrase =
                httpResponseMessage.ReasonPhrase;
        }
    }
}
