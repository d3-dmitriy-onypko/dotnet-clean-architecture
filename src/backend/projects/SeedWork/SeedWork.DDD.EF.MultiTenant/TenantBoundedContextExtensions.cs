using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SeedWork.DDD.EF.MultiTenant;

public static class TenantBoundedContextExtensions
{
    public static void SetTenantIdForEntities(this DbContext context, Guid tenantId)
    {
        var entities = context.ChangeTracker.Entries().Where(x => x.State is EntityState.Added or EntityState.Modified)
            .Select(x => x.Entity).OfType<ITenantEntity>().ToArray();

        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            if (entity.HasTenantId)
            {
                entity.SetTenantId(tenantId);
            }
        }
    }
}