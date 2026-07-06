// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.Events.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventV2s(EntityTypeBuilder<EventV2> model)
        {
            model.ToTable("EventV2s");
            model.HasKey(eventV2 => eventV2.Id);

            model.Property(eventV2 => eventV2.ContentHash)
                .IsRequired(false)
                .HasMaxLength(450);

            model.HasIndex(eventV2 => new
            {
                eventV2.EventAddressV2Id,
                eventV2.EventName,
                eventV2.ContentHash,
                eventV2.CreatedDate
            })
            .HasDatabaseName("IX_EventV2s_LoopDetection");

            model.HasIndex(eventV2 => new { eventV2.Status, eventV2.Type })
                .HasDatabaseName("IX_EventV2s_StatusType");

            model.HasIndex(eventV2 => new { eventV2.EventAddressV2Id, eventV2.CreatedDate })
                .IncludeProperties(eventV2 => new
                {
                    eventV2.Status,
                    eventV2.Type,
                    eventV2.EventParticipantV2Id
                })
                .HasDatabaseName("IX_EventV2s_AddressCreatedDate");

            model.HasIndex(eventV2 => eventV2.CreatedDate)
                .IncludeProperties(eventV2 => eventV2.Type)
                .HasDatabaseName("IX_EventV2s_CreatedDate");

            model.HasOne(eventV2 => eventV2.EventAddressV2)
                .WithMany(eventAddressV2 => eventAddressV2.EventV2s)
                .HasForeignKey(eventV2 => eventV2.EventAddressV2Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
