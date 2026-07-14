// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EventHighway.PostgreSql.Interceptors
{
    internal sealed class DateTimeOffsetTruncationInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            if (eventData.Context is not null)
            {
                TruncateDateTimeOffsets(eventData.Context);
            }

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (eventData.Context is not null)
            {
                TruncateDateTimeOffsets(eventData.Context);
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void TruncateDateTimeOffsets(DbContext context)
        {
            foreach (EntityEntry entry in context.ChangeTracker.Entries())
            {
                if (entry.State is not (EntityState.Added or EntityState.Modified))
                {
                    continue;
                }

                foreach (PropertyEntry property in entry.Properties)
                {
                    if (property.CurrentValue is DateTimeOffset value)
                    {
                        property.CurrentValue = value.AddTicks(
                            -(value.Ticks % TimeSpan.TicksPerMicrosecond));
                    }
                }
            }
        }
    }
}
