using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Common.Interfaces;
using QuanLyHoaLan.Domain.Interfaces.Services;

namespace QuanLyHoaLan.Infrastructure.Persistence.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeService _dateTime;

    public AuditableEntityInterceptor(ICurrentUser currentUser, IDateTimeService dateTime)
    {
        _currentUser = currentUser;
        _dateTime = dateTime;
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

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        Guid? currentUserId = _currentUser.UserId;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            var utcNow = _dateTime.UtcNow.UtcDateTime;
            
            // 1. Handle Auditing (Created/Updated)
            if (entry.Entity is IAuditableEntity auditable)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (auditable.CreatedBy == null || auditable.CreatedBy == Guid.Empty)
                            auditable.CreatedBy = currentUserId;
                        if (auditable.CreatedAt == default(DateTime))
                            auditable.CreatedAt = utcNow;
                        break;

                    case EntityState.Modified:
                        auditable.UpdatedBy = currentUserId;
                        auditable.UpdatedAt = utcNow;
                        break;
                }
            }

            // 2. Handle Soft Delete separately
            if (entry.State != EntityState.Deleted || entry.Entity is not ISoftDelete softDelete) continue;
            
            entry.State = EntityState.Modified;
            softDelete.IsDeleted = true;
            softDelete.DeletedAt = utcNow;
            softDelete.DeletedBy = currentUserId;
        }
    }
}
