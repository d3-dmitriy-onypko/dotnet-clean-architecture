public interface ISoftDeleteEntity
{
    bool IsDeleted { get; }

    void SetDeleted(bool isDeleted = true);
}
