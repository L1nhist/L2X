using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using L2X.Core.Authentications;
using L2X.Core.Structures;

namespace L2X.Data.Extentions;

public class AuditingInterceptor(IAuthContext context) : SaveChangesInterceptor
{
    private readonly IAuthContext _context = context ?? throw new NullReferenceException("AuthUserContext must not be null.");

    public static bool HasChangedOwnedEntities(EntityEntry entry) =>
        entry.References.Any(r => r.TargetEntry != null && r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified || r.TargetEntry.State == EntityState.Modified));

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var now = Epoch.Now;
        foreach (var entry in context.ChangeTracker.Entries())
        {
            // Change fields before Insert
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is IMomentable mmt)
                {
                    mmt.CreatedAt = now;
                }
                if (entry.Entity is IAuditable adt)
                {
                    adt.Creator = _context?.User?.Username ?? "";
                }
            }

            // Change fields before Update
            if (entry.State == EntityState.Modified || HasChangedOwnedEntities(entry))
            {
                if (entry.Entity is IMomentable mmt)
                {
                    mmt.ModifiedAt = now;
                }
                if (entry.Entity is IAuditable adt)
                {
                    adt.Modifier = _context?.User?.Username ?? "";
                }
            }
        }
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}