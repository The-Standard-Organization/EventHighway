// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V1
{
    public partial class ListenerEventV1ServiceTests
    {
        [Fact]
        public async Task ShouldModifyListenerEventV1Async()
        {
            // given
            var mockSequence = new MockSequence();

            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            int randomDaysAgo =
                GetRandomNegativeNumber();

            ListenerEventV1 randomListenerEventV1 =
                CreateRandomListenerEventV1(dates: randomDateTimeOffset);

            ListenerEventV1 inputListenerEventV1 =
                randomListenerEventV1;

            inputListenerEventV1.CreatedDate =
                randomDateTimeOffset.AddDays(randomDaysAgo);

            ListenerEventV1 storageListenerEventV1 =
                inputListenerEventV1.DeepClone();

            int randomSecondsAgo =
                GetRandomNegativeNumber();

            DateTimeOffset storageUpdatedDate =
                randomDateTimeOffset.AddSeconds(
                    randomSecondsAgo);

            storageListenerEventV1.UpdatedDate =
                storageUpdatedDate;

            ListenerEventV1 persistedListenerEventV1 =
                inputListenerEventV1;

            ListenerEventV1 expectedListenerEventV1 =
                persistedListenerEventV1.DeepClone();

            Guid inputListenerEventV1Id =
                inputListenerEventV1.Id;

            this.dateTimeBrokerMock
                .InSequence(mockSequence).Setup(broker =>
                    broker.GetDateTimeOffsetAsync())
                        .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock
                .InSequence(mockSequence).Setup(broker =>
                    broker.SelectListenerEventByIdV1Async(
                        inputListenerEventV1Id))
                            .ReturnsAsync(
                                storageListenerEventV1);

            this.storageBrokerMock
                .InSequence(mockSequence).Setup(broker =>
                    broker.UpdateListenerEventV1Async(
                        inputListenerEventV1))
                            .ReturnsAsync(persistedListenerEventV1);

            // when
            ListenerEventV1 actualListenerEventV1 =
                await this.listenerEventV1Service
                    .ModifyListenerEventV1Async(
                        inputListenerEventV1);

            // then
            actualListenerEventV1.Should().BeEquivalentTo(
                expectedListenerEventV1);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventByIdV1Async(
                    inputListenerEventV1Id),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateListenerEventV1Async(
                    inputListenerEventV1),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
