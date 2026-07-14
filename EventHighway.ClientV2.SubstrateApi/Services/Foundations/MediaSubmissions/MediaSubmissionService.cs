// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Brokers.Apis;
using EventHighway.ClientV2.SubstrateApi.Brokers.Configurations;
using EventHighway.ClientV2.SubstrateApi.Brokers.Loggings;
using EventHighway.ClientV2.SubstrateApi.Models.Brokers.Configurations;
using EventHighway.ClientV2.SubstrateApi.Models.MediaSubmissions;

namespace EventHighway.ClientV2.SubstrateApi.Services.Foundations.MediaSubmissions
{
    // The send button's side of the round trip. It carries the UI's JSON to the public /submit
    // intake over HTTP and maps the answer — accepted or refused, and why — into a plain result
    // the view can report. Whether the submission then reaches the chat is up to the highway.
    //
    // It can also just *describe* that call, without making it, so the UI can show a reader how to
    // reproduce it in Postman. Both come from the same configuration, so what is on screen is
    // always what the app would actually send.
    public partial class MediaSubmissionService : IMediaSubmissionService
    {
        private const string SubmitMethod = "POST";
        private const string ContentTypeHeader = "Content-Type";

        private readonly IApiBroker apiBroker;
        private readonly IConfigurationBroker configurationBroker;
        private readonly ILoggingBroker loggingBroker;

        public MediaSubmissionService(
            IApiBroker apiBroker,
            IConfigurationBroker configurationBroker,
            ILoggingBroker loggingBroker)
        {
            this.apiBroker = apiBroker;
            this.configurationBroker = configurationBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<MediaSubmission> SubmitMediaItemAsync(
            string content,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateContent(content);

            SubstrateApiConfigurations configurations =
                this.configurationBroker.GetSubstrateApiConfigurations();

            ValidateConfigurations(configurations);

            HttpResponseMessage httpResponseMessage =
                await this.apiBroker.PostMediaItemAsync(
                    url: configurations.SubmitUrl,
                    participantId: configurations.ParticipantId,
                    participantSecret: configurations.ParticipantSecret,
                    content: content,
                    cancellationToken: cancellationToken);

            string responseBody =
                await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

            return new MediaSubmission
            {
                IsAccepted = httpResponseMessage.IsSuccessStatusCode,
                ResponseCode = ((int)httpResponseMessage.StatusCode).ToString(),
                Response = responseBody
            };
        });

        public ValueTask<MediaSubmissionEndpoint> RetrieveMediaSubmissionEndpointAsync() =>
        TryCatch(() =>
        {
            SubstrateApiConfigurations configurations =
                this.configurationBroker.GetSubstrateApiConfigurations();

            ValidateConfigurations(configurations);

            var mediaSubmissionEndpoint = new MediaSubmissionEndpoint
            {
                Method = SubmitMethod,
                Url = configurations.SubmitUrl,

                Headers = new List<MediaSubmissionHeader>
                {
                    new MediaSubmissionHeader
                    {
                        Name = ContentTypeHeader,
                        Value = ApiBroker.ContentType
                    },

                    new MediaSubmissionHeader
                    {
                        Name = ApiBroker.ParticipantHeader,
                        Value = configurations.ParticipantId,
                        IsCredential = true
                    },

                    new MediaSubmissionHeader
                    {
                        Name = ApiBroker.ParticipantSecretHeader,
                        Value = configurations.ParticipantSecret,
                        IsCredential = true
                    }
                }
            };

            return new ValueTask<MediaSubmissionEndpoint>(mediaSubmissionEndpoint);
        });
    }
}
