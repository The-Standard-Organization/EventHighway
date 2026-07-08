// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Portal.Web.Models.Foundations.Roles;
using EventHighway.Portal.Web.Models.Foundations.UserEventParticipants;
using EventHighway.Portal.Web.Models.Foundations.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Portal.Web.Data
{
    public class SecurityDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public SecurityDbContext(DbContextOptions<SecurityDbContext> options)
            : base(options)
        { }

        public DbSet<UserEventParticipant> UserEventParticipants { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserEventParticipant>(entity =>
            {
                entity.HasKey(association => association.Id);

                entity.HasIndex(association => new
                {
                    association.UserId,
                    association.EventParticipantId
                })
                    .IsUnique();

                entity.HasIndex(association => association.EventParticipantId);

                entity.HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey(association => association.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
