// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Views.ListenerEventArchives;
using EventHighway.Portal.Web.Services.Views.ListenerEventArchives;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.ListenerEventArchives
{
    public partial class ListenerEventArchivesViewServiceTests
    {
        private readonly Mock<IEventHighwayBroker> eventHighwayBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IListenerEventArchivesViewService listenerEventArchivesViewService;

        public ListenerEventArchivesViewServiceTests()
        {
            this.eventHighwayBrokerMock = new Mock<IEventHighwayBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.listenerEventArchivesViewService = new ListenerEventArchivesViewService(
                eventHighwayBroker: this.eventHighwayBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static ListenerEventArchiveV2 CreateRandomListenerEventArchive(
            DateTimeOffset createdDate) =>
            new ListenerEventArchiveV2
            {
                Id = Guid.NewGuid(),
                Status = ListenerEventArchiveStatusV2.Success,
                Response = GetRandomString(),
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString(),
                RemainingRetryAttempts = 2,
                RetryAttemptsAllowed = 5,
                NextRetryAttemptNotBefore = createdDate.AddMinutes(30),
                DispatchedDate = createdDate,
                EventV2Id = Guid.NewGuid(),
                EventAddressV2Id = Guid.NewGuid(),
                EventListenerV2Id = Guid.NewGuid(),
                EventArchiveV2Id = Guid.NewGuid(),
                EventParticipantV2Id = Guid.NewGuid(),
                CreatedDate = createdDate,
                UpdatedDate = createdDate,
                ArchivedDate = createdDate
            };

        private static ListenerEventArchiveView MapToView(
            ListenerEventArchiveV2 listenerEventArchive) =>
            new ListenerEventArchiveView
            {
                Id = listenerEventArchive.Id,
                Status = listenerEventArchive.Status.ToString(),
                Response = listenerEventArchive.Response,
                ResponseCode = listenerEventArchive.ResponseCode,
                ResponseMessage = listenerEventArchive.ResponseMessage,
                RemainingRetryAttempts = listenerEventArchive.RemainingRetryAttempts,
                RetryAttemptsAllowed = listenerEventArchive.RetryAttemptsAllowed,
                NextRetryAttemptNotBefore = listenerEventArchive.NextRetryAttemptNotBefore,
                DispatchedDate = listenerEventArchive.DispatchedDate,
                EventV2Id = listenerEventArchive.EventV2Id,
                EventAddressV2Id = listenerEventArchive.EventAddressV2Id,
                EventListenerV2Id = listenerEventArchive.EventListenerV2Id,
                EventArchiveV2Id = listenerEventArchive.EventArchiveV2Id,
                EventParticipantV2Id = listenerEventArchive.EventParticipantV2Id,
                CreatedDate = listenerEventArchive.CreatedDate,
                ArchivedDate = listenerEventArchive.ArchivedDate
            };
    }
}
