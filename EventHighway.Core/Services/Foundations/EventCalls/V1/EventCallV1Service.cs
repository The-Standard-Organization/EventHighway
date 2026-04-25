// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
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

        public ValueTask<EventCallV1> RunEventCallAsync(EventCallV1 eventCall) =>
        TryCatch(async () =>
        {
            ValidateEventCallOnRun(eventCall);

            string response =
                await apiBroker.PostAsync(
                    content: eventCall.Content,
                    url: eventCall.Endpoint,
                    secret: eventCall.Secret);

            eventCall.Response = response;

            return eventCall;
        });

        public ValueTask<EventCallV1> RunEventCallAsyncV1(EventCallV1 eventCall) =>
        TryCatch(async () =>
        {
            ValidateEventCallOnRun(eventCall);

            HttpResponseMessage httpResponseMessage =
                await apiBroker.PostAsyncV1(
                    content: eventCall.Content,
                    url: eventCall.Endpoint,
                    secret: eventCall.Secret);

            ValidateHttpResponseMessageIsNotNull(httpResponseMessage);
            await MapToEventCallV1Async(eventCall, httpResponseMessage);

            return eventCall;
        });

        private static async ValueTask MapToEventCallV1Async(
            EventCallV1 eventCall,
            HttpResponseMessage httpResponseMessage)
        {
            eventCall.Response =
                await httpResponseMessage.Content
                    .ReadAsStringAsync();

            eventCall.ResponseReasonPhrase =
                httpResponseMessage.ReasonPhrase;

            eventCall.IsSuccess =
                httpResponseMessage.IsSuccessStatusCode;
        }
    }
}
