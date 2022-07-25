using System;
using SeedWork.DDD;

namespace SeedWork.DDD.EF.MultiTenant;
public class NullableTenantEntity : SoftDeleteEntity, INullableTenantEntity
{
    public Guid? TenantId { get; private set; }

    public bool HasTenantId => TenantId is not null;

    public void SetTenantId(Guid? tenantId) => TenantId = tenantId;
}