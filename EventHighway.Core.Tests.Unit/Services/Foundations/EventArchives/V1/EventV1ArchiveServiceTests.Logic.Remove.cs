// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldRemoveEventV1ArchivesAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            DateTimeOffset inputDateTimeOffset =
                randomDateTimeOffset;

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteEventV1ArchivesAsync(
                    inputDateTimeOffset))
                        .ReturnsAsync(1);

            // when
            int actualEventV1Archive =
                await this.eventV1ArchiveService
                    .RemoveEventV1ArchivesAsync(
                        inputDateTimeOffset);

            // then
            actualEventV1Archive.Should().BeGreaterThanOrEqualTo(1);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventV1ArchivesAsync(
                    inputDateTimeOffset),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
