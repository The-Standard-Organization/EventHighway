// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Delegates.JoesRestApi.Brokers.Apis;
using EventHighway.EventHandlers.Delegates.JoesRestApi.Brokers.Configurations;
using EventHighway.EventHandlers.Delegates.JoesRestApi.Models.Foundations.EventPosts.Exceptions;
using EventHighway.EventHandlers.Delegates.JoesRestApi.Services.Foundations.EventPosts;
using Microsoft.Extensions.Configuration;

namespace EventHighway.EventHandlers.Delegates.JoesRestApi.Clients
{
    public class JoesRestApiDelegateClient : IJoesRestApiDelegateClient
    {
        private readonly IEventPostService eventPostService;

        public JoesRestApiDelegateClient(IConfiguration configuration)
        {
            var configurationBroker = new ConfigurationBroker(configuration);
            var apiBroker = new ApiBroker();

            this.eventPostService =
                new EventPostService(configurationBroker, apiBroker);
        }

        internal JoesRestApiDelegateClient(IEventPostService eventPostService) =>
            this.eventPostService = eventPostService;

        public async ValueTask<EventHandlerResult> PostToJoesRestApiAsync(
            string content,
            CancellationToken cancellationToken)
        {
            try
            {
                return await this.eventPostService.PostEventAsync(content, cancellationToken);
            }
            catch (EventPostValidationException eventPostValidationException)
            {
                return CreateFailureResult(
                    eventPostValidationException,
                    responseCode: "400",
                    responseMessage: "Bad Request");
            }
            catch (EventPostDependencyException eventPostDependencyException)
            {
                return CreateFailureResult(
                    eventPostDependencyException,
                    responseCode: "502",
                    responseMessage: "Bad Gateway");
            }
            catch (Exception exception)
            {
                return CreateFailureResult(
                    exception,
                    responseCode: "500",
                    responseMessage: "Internal Server Error");
            }
        }

        private static EventHandlerResult CreateFailureResult(
            Exception exception,
            string responseCode,
            string responseMessage) =>
            new EventHandlerResult
            {
                IsSuccess = false,
                Response = exception.InnerException?.Message ?? exception.Message,
                ResponseCode = responseCode,
                ResponseMessage = responseMessage
            };
    }
}
