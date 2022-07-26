public class SoftDeleteEntity :  ISoftDeleteEntity, IAuditEntity
{
    public int CreatedByUser { get; private set; }

    public Instant CreatedDate { get; private set; }

    public int LastModifiedByUser { get; private set; }

    public Instant LastModifiedDate { get; private set; }

    public bool IsDeleted { get; private set; }

    public virtual void SetDeleted(bool isDeleted = true) => IsDeleted = isDeleted;

    public virtual void OnAdded(Instant createdDate, int createdByUser)
    {
        CreatedDate = createdDate;
        CreatedByUser = createdByUser;
    }

    public virtual void OnUpdated(Instant lastModifiedDate, int lastModifiedByUser)
    {
        LastModifiedByUser = lastModifiedByUser;
        LastModifiedDate = lastModifiedDate;
    }
}
