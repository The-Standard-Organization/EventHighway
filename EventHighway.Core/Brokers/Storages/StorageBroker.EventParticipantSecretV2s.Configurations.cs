// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventParticipantSecretV2s(
            EntityTypeBuilder<EventParticipantSecretV2> model)
        {
            model.ToTable("EventParticipantSecretV2s");
            model.HasKey(eventParticipantSecretV2 => eventParticipantSecretV2.Id);

            model.HasIndex(eventParticipantSecretV2 => new
            {
                eventParticipantSecretV2.EventParticipantV2Id,
                eventParticipantSecretV2.Secret
            })
            .IsUnique();

            model.Property(eventParticipantSecretV2 => eventParticipantSecretV2.Secret)
                .HasMaxLength(450);

            model.HasOne(eventParticipantSecretV2 => eventParticipantSecretV2.EventParticipantV2)
                .WithMany(eventParticipantV2 => eventParticipantV2.EventParticipantSecretV2s)
                .HasForeignKey(eventParticipantSecretV2 => eventParticipantSecretV2.EventParticipantV2Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
