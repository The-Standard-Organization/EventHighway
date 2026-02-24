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
    public partial class ListenerEventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldAddListenerEventV1ArchiveAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            ListenerEventV1Archive randomListenerEventV1Archive =
                CreateRandomListenerEventV1Archive(
                    randomDateTimeOffset);

            ListenerEventV1Archive inputListenerEventV1Archive =
                randomListenerEventV1Archive;

            ListenerEventV1Archive storageListenerEventV1Archive =
                inputListenerEventV1Archive;

            ListenerEventV1Archive expectedListenerEventV1Archive =
                storageListenerEventV1Archive.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertListenerEventV1ArchiveAsync(
                    inputListenerEventV1Archive))
                        .ReturnsAsync(storageListenerEventV1Archive);

            // when
            ListenerEventV1Archive actualListenerEventV1Archive =
                await this.listenerEventV1ArchiveService
                    .AddListenerEventV1ArchiveAsync(
                        inputListenerEventV1Archive);

            // then
            actualListenerEventV1Archive.Should().BeEquivalentTo(
                expectedListenerEventV1Archive);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1ArchiveAsync(
                    inputListenerEventV1Archive),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
