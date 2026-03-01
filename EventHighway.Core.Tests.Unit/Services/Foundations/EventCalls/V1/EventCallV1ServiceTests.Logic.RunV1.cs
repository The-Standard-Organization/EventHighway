// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Net.Http;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventCalls.V1
{
    public partial class EventCallV1ServiceTests
    {
        [Fact]
        public async Task ShouldRunEventCallV1AsyncV1()
        {
            // given
            EventCallV1 randomEventCallV1 =
                CreateRandomEventCallV1();

            EventCallV1 inputEventCallV1 =
                randomEventCallV1;

            string randomHttpResponseMessageContent =
                GetRandomString();

            string retrievedHttpResponseMessageContent =
                randomHttpResponseMessageContent;

            string randomHttpResponseMessageReasonPhrase =
                GetRandomString();

            string retrievedHttpResponseMessageReasonPhrase =
                randomHttpResponseMessageReasonPhrase;

            HttpResponseMessage randomHttpResponseMessage =
                CreateRandomHttpResponseMessage(
                    retrievedHttpResponseMessageContent,
                    randomHttpResponseMessageReasonPhrase);

            HttpResponseMessage retrievedHttpResponseMessage =
                randomHttpResponseMessage;

            EventCallV1 expectedEventCallV1 =
                inputEventCallV1.DeepClone();

            expectedEventCallV1.Response =
                retrievedHttpResponseMessageContent;

            expectedEventCallV1.ResponseReasonPhrase =
                randomHttpResponseMessageReasonPhrase;

            this.apiBrokerMock.Setup(broker =>
                broker.PostAsyncV1(
                    inputEventCallV1.Content,
                    inputEventCallV1.Endpoint,
                    inputEventCallV1.Secret))
                        .ReturnsAsync(retrievedHttpResponseMessage);

            // when
            EventCallV1 actualEventCallV1 =
                await this.eventCallV1Service
                    .RunEventCallV1AsyncV1(inputEventCallV1);

            // then
            actualEventCallV1.Should().BeEquivalentTo(
                expectedEventCallV1);

            this.apiBrokerMock.Verify(broker =>
                broker.PostAsyncV1(
                    inputEventCallV1.Content,
                    inputEventCallV1.Endpoint,
                    inputEventCallV1.Secret),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
