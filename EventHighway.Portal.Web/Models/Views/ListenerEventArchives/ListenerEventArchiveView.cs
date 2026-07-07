// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Portal.Web.Models.Views.ListenerEventArchives
{
    public class ListenerEventArchiveView
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public int RemainingRetryAttempts { get; set; }
        public int RetryAttemptsAllowed { get; set; }
        public DateTimeOffset? NextRetryAttemptNotBefore { get; set; }
        public DateTimeOffset? DispatchedDate { get; set; }
        public Guid EventV2Id { get; set; }
        public Guid EventAddressV2Id { get; set; }
        public Guid EventListenerV2Id { get; set; }
        public string? ListenerName { get; set; }
        public Guid EventArchiveV2Id { get; set; }
        public Guid? EventParticipantV2Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ArchivedDate { get; set; }
    }
}
