using System;
using SeedWork.DDD;

public class NullableTenantEntity : SoftDeleteEntity, INullableTenantEntity
{
    public Guid? TenantId { get; set; }

    public bool HasTenantId => TenantId is not null;

    public void SetTenantId(Guid? tenantId) => TenantId = tenantId;
}