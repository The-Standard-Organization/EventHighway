// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V1
{
    public partial class ListenerEventArchiveV1ServiceTests
    {
        [Fact]
        public async Task ShouldAddListenerEventArchiveV1Async()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            ListenerEventArchiveV1 randomListenerEventArchiveV1 =
                CreateRandomListenerEventArchiveV1(
                    randomDateTimeOffset);

            ListenerEventArchiveV1 inputListenerEventArchiveV1 =
                randomListenerEventArchiveV1;

            ListenerEventArchiveV1 storageListenerEventArchiveV1 =
                inputListenerEventArchiveV1;

            ListenerEventArchiveV1 expectedListenerEventArchiveV1 =
                storageListenerEventArchiveV1.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertListenerEventArchiveV1Async(
                    inputListenerEventArchiveV1))
                        .ReturnsAsync(storageListenerEventArchiveV1);

            // when
            ListenerEventArchiveV1 actualListenerEventArchiveV1 =
                await this.listenerEventArchiveV1Service
                    .AddListenerEventArchiveV1Async(
                        inputListenerEventArchiveV1);

            // then
            actualListenerEventArchiveV1.Should().BeEquivalentTo(
                expectedListenerEventArchiveV1);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV1Async(
                    inputListenerEventArchiveV1),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
