// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V2
{
    public partial class ListenerEventV2ServiceTests
    {
        [Fact]
        public async Task ShouldBulkModifyListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset now = DateTimeOffset.MaxValue;

            List<ListenerEventV2> randomListenerEventV2s =
                CreateRandomRestoreListenerEventV2s();

            List<ListenerEventV2> inputListenerEventV2s = randomListenerEventV2s;

            List<ListenerEventV2> expectedListenerEventV2s =
                inputListenerEventV2s.Select(item =>
                {
                    ListenerEventV2 clonedListenerEventV2 = item.DeepClone();
                    clonedListenerEventV2.UpdatedDate = now;

                    return clonedListenerEventV2;
                }).ToList();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkUpdateListenerEventV2sAsync(
                    It.Is<List<ListenerEventV2>>(actual =>
                        SameListenerEventV2sAs(expectedListenerEventV2s, actual)),
                            randomCancellationToken))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2Service.BulkModifyListenerEventV2sAsync(
                    inputListenerEventV2s,
                        randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkUpdateListenerEventV2sAsync(
                    It.Is<List<ListenerEventV2>>(actual =>
                        SameListenerEventV2sAs(expectedListenerEventV2s, actual)),
                            randomCancellationToken),
                                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
