// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ClientTests
    {
        [Fact]
        public async Task ShouldResolveServiceInNewScopePerOperationAsync()
        {
            // given
            var inputListenerEventArchiveV2Query = new ListenerEventArchiveV2Query();
            int expectedResolutionCount = 2;

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveListenerEventArchiveV2sByQueryAsync(
                    inputListenerEventArchiveV2Query, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<ListenerEventArchiveV2>());

            // when
            await this.listenerEventArchiveV2Client.RetrieveAllListenerEventArchiveV2sAsync(
                inputListenerEventArchiveV2Query);

            await this.listenerEventArchiveV2Client.RetrieveAllListenerEventArchiveV2sAsync(
                inputListenerEventArchiveV2Query);

            // then
            this.listenerEventArchiveServiceResolutionCount.Should()
                .Be(expectedResolutionCount);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventArchiveV2sByQueryAsync(
                    inputListenerEventArchiveV2Query, It.IsAny<CancellationToken>()),
                        Times.Exactly(2));

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
