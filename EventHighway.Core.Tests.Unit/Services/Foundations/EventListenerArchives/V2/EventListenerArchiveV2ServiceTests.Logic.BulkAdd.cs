// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListenerArchives.V2
{
    public partial class EventListenerArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldBulkAddEventListenerArchiveV2sAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            IQueryable<EventListenerArchiveV2> randomEventListenerArchiveV2s =
                CreateRandomEventListenerArchiveV2s();

            List<EventListenerArchiveV2> inputEventListenerArchiveV2s =
                randomEventListenerArchiveV2s.ToList();

            List<EventListenerArchiveV2> expectedEventListenerArchiveV2s =
                inputEventListenerArchiveV2s.Select(item => item.DeepClone()).ToList();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            foreach (EventListenerArchiveV2 item in expectedEventListenerArchiveV2s)
            {
                item.ArchivedDate = randomDateTime;
            }

            this.storageBrokerMock.Setup(broker =>
                broker.InsertBulkEventListenerArchiveV2sAsync(
                    It.Is<List<EventListenerArchiveV2>>(actual =>
                        SameEventListenerArchiveV2sAs(expectedEventListenerArchiveV2s, actual)),
                            It.IsAny<CancellationToken>()))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<EventListenerArchiveV2> actualEventListenerArchiveV2s =
                await this.eventListenerArchiveV2Service.BulkAddEventListenerArchiveV2sAsync(
                    inputEventListenerArchiveV2s,
                        TestContext.Current.CancellationToken);

            // then
            actualEventListenerArchiveV2s.Should().BeEquivalentTo(expectedEventListenerArchiveV2s);

            foreach (EventListenerArchiveV2 item in inputEventListenerArchiveV2s)
            {
                Assert.Equal(randomDateTime, item.ArchivedDate);
            }

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(inputEventListenerArchiveV2s.Count + 1));

            this.storageBrokerMock.Verify(broker =>
                broker.InsertBulkEventListenerArchiveV2sAsync(
                    It.Is<List<EventListenerArchiveV2>>(actual =>
                        SameEventListenerArchiveV2sAs(expectedEventListenerArchiveV2s, actual)),
                            It.IsAny<CancellationToken>()),
                                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
