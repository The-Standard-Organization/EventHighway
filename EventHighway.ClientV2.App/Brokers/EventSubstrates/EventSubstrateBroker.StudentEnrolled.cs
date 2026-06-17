// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.ClientV2.App.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker
    {
        private Guid studentEnrolledAddressId;

        public async ValueTask RegisterStudentEnrolledAddressAsync(
            EventAddressV2 address,
            CancellationToken cancellationToken = default)
        {
            await this.eventHighwayClient.V2.EventAddressV2Client
                .RegisterEventAddressV2Async(address);

            this.studentEnrolledAddressId = address.Id;
        }

        public async ValueTask RegisterStudentEnrolledListenerAsync(
            EventListenerV2 listener,
            CancellationToken cancellationToken = default)
        {
            listener.EventAddressId = this.studentEnrolledAddressId;

            await this.eventHighwayClient.V2.EventListenerV2Client
                .RegisterEventListenerV2Async(listener);
        }

        public async ValueTask<EventV2> RaiseStudentEnrolledAsync(
            string content,
            CancellationToken cancellationToken = default)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            var @event = new EventV2
            {
                Id = Guid.NewGuid(),
                EventName = "Student Enrolled Event",
                Type = EventTypeV2.Scheduled,
                Content = content,
                EventAddressId = this.studentEnrolledAddressId,
                ScheduledDate = now.AddSeconds(-1),
                CreatedDate = now,
                UpdatedDate = now
            };

            await this.eventHighwayClient.V2.EventV2Client
                .SubmitEventV2Async(@event);

            return @event;
        }
    }
}
