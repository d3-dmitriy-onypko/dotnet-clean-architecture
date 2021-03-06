global using System;
global using System.Linq;
global using Microsoft.EntityFrameworkCore;

public class BoundedContext<T> : IAsyncDisposable, IDisposable
    where T : DbContext
{
    public T? DbContext { get; private set; }
    private readonly MediatR.IPublisher _eventDispatcher;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IClock _clock;

    public BoundedContext(
        IDbContextFactory<T> factory,
        MediatR.IPublisher eventDispatcher,
        ICurrentUserProvider currentUserProvider,
        IClock clock)
    {
        DbContext = factory.CreateDbContext();
        _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public async ValueTask<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var rowsCount = 0;

        var entries = DbContext!.ChangeTracker.Entries().ToArray();

        var currentUserId = await _currentUserProvider.GetCurrentUserId();

        do
        {
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Detached)
                {
                    continue;
                }

                if (entry.Entity is IAuditEntity auditEntity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntity.OnAdded(_clock.UtcNow, currentUserId);
                            break;
                        case EntityState.Modified:
                            auditEntity.OnUpdated(_clock.UtcNow, currentUserId);
                            break;
                        case EntityState.Detached:
                            break;
                        case EntityState.Unchanged:
                            break;
                        case EntityState.Deleted:
                            break;
                        default:
                            break;
                    }
                }

                if (entry.State == EntityState.Deleted && entry.Entity is ISoftDeleteEntity softDeleteEntity)
                {
                    DbContext.Entry(entry).State = EntityState.Modified;
                    softDeleteEntity.SetDeleted(true);
                }

                if (entry.Entity is AggregateRoot aggregateRoot)
                {
                    foreach (var domainEvent in aggregateRoot.DomainEvents)
                    {
                        await _eventDispatcher.Publish(domainEvent, ct);
                    }

                    aggregateRoot.ClearDomainEvents();
                }

                rowsCount += await DbContext.SaveChangesAsync(ct);
            }

            entries = DbContext
                .ChangeTracker
                .Entries()
                .Where(f =>
                    (f.Entity is AggregateRoot a && a.DomainEvents.Any()) ||
                    (f.State != EntityState.Detached && f.State != EntityState.Unchanged))
                .ToArray();
        } while (entries.Any());

        return rowsCount;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            DbContext?.Dispose();
            DbContext = null;
        }
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (DbContext is not null)
        {
            await DbContext.DisposeAsync().ConfigureAwait(false);
        }

        DbContext = null;
    }
}
