// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Portal.Web.Models.Views.HealthDashboards.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.HealthDashboards
{
    public partial class HealthViewServiceTests
    {
        public static TheoryData<Xeption> DependencyExceptions()
        {
            var someInnerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new HealthStatusClientV2DependencyException(
                    message: GetRandomString(),
                    innerException: someInnerException,
                    data: new Hashtable()),

                new HealthStatusClientV2ServiceException(
                    message: GetRandomString(),
                    innerException: someInnerException,
                    data: new Hashtable()),
            };
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveIfDependencyValidationErrorOccursAndLogItAsync()
        {
            // given
            var someInnerException = new Xeption(message: GetRandomString());

            var clientValidationException =
                new HealthStatusClientV2ValidationException(
                    message: GetRandomString(),
                    innerException: someInnerException,
                    data: new Hashtable());

            var expectedHealthViewDependencyValidationException =
                new HealthViewDependencyValidationException(
                    innerException: clientValidationException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveHealthRagStatusV2Async(It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(clientValidationException);

            // when
            ValueTask<System.Collections.Generic.List<
                EventHighway.Portal.Web.Models.Views.HealthDashboards.HealthRagTile>>
                    retrieveTask =
                        this.healthViewService.RetrieveHealthRagTilesAsync(
                        TrafficPeriodV2.Day, System.DateTimeOffset.MinValue,
                        null,
                        TestContext.Current.CancellationToken);

            HealthViewDependencyValidationException actualException =
                await Assert.ThrowsAsync<HealthViewDependencyValidationException>(
                    async () => await retrieveTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedHealthViewDependencyValidationException);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveHealthRagStatusV2Async(It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            var expectedHealthViewDependencyException =
                new HealthViewDependencyException(innerException: dependencyException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveHealthRagStatusV2Async(It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            HealthViewDependencyException actualException =
                await Assert.ThrowsAsync<HealthViewDependencyException>(
                    async () => await this.healthViewService.RetrieveHealthRagTilesAsync(
                        TrafficPeriodV2.Day, System.DateTimeOffset.MinValue,
                        null,
                        TestContext.Current.CancellationToken));

            // then
            actualException.Should().BeEquivalentTo(expectedHealthViewDependencyException);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveHealthRagStatusV2Async(It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveIfServiceErrorOccursAndLogItAsync()
        {
            // given
            var serviceException = new System.Exception(message: GetRandomString());

            var failedHealthViewServiceException =
                new FailedHealthViewServiceException(innerException: serviceException);

            var expectedHealthViewServiceException =
                new HealthViewServiceException(
                    innerException: failedHealthViewServiceException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveHealthRagStatusV2Async(It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            HealthViewServiceException actualException =
                await Assert.ThrowsAsync<HealthViewServiceException>(
                    async () => await this.healthViewService.RetrieveHealthRagTilesAsync(
                        TrafficPeriodV2.Day, System.DateTimeOffset.MinValue,
                        null,
                        TestContext.Current.CancellationToken));

            // then
            actualException.Should().BeEquivalentTo(expectedHealthViewServiceException);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveHealthRagStatusV2Async(It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
