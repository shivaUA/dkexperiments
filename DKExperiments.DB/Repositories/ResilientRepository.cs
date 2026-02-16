using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql;
using Polly;
using Polly.Retry;

namespace DKExperiments.DB.Repositories;

public class ResilientRepository
{
    /// <summary>
    /// Executes a save operation with a resilience pipeline handling common EF/Postgres concurrency and unique constraint race conditions.
    /// </summary>
    /// <typeparam name="T">Return type of the save operation</typeparam>
    /// <param name="entityPredicate">Predicate used to locate the changed entry among entries reported by EF exceptions</param>
    /// <param name="executionFunction">The actual save operation to execute inside the pipeline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="maxRetryAttempts">Maximum number of retry attempts (default 1)</param>
    /// <param name="initialDelayMs">Initial delay in milliseconds between retries (default 200)</param>
    /// <returns>The result of the execution function or null if not available</returns>
    protected static internal async Task<T?> SaveRecordResilientAsync<T>(
        Func<EntityEntry, bool> entityPredicate,
        Func<CancellationToken, ValueTask<T?>> executionFunction,
        CancellationToken cancellationToken,
        int maxRetryAttempts = 1,
        int initialDelayMs = 200)
    {
        if (entityPredicate == null) throw new ArgumentNullException(nameof(entityPredicate));
        if (executionFunction == null) throw new ArgumentNullException(nameof(executionFunction));

        var resiliencePipeline = new ResiliencePipelineBuilder()
            .AddRetry(
                new RetryStrategyOptions
                {
                    // Retry on concurrency conflicts and unique-constraint-on-insert races
                    ShouldHandle = new PredicateBuilder()
                        .Handle((DbUpdateException ex) =>
                        {
                            var inner = ex.InnerException as PostgresException;
                            var state = inner?.SqlState;

                            // Postgres unique violation SQLSTATE is '23505'
                            var uniqueViolation = state == "23505";

                            var entry = ex.Entries.FirstOrDefault();
                            var isInsert = entry != null && entry.State == EntityState.Added;

                            var shouldRetry = inner != null && uniqueViolation && isInsert;

                            if (shouldRetry)
                            {
                                UpdateEntryStateSafe(ex.Entries, entityPredicate);
                            }

                            return shouldRetry;
                        })
                        .Handle((DbUpdateConcurrencyException ex) =>
                        {
                            UpdateEntryStateSafe(ex.Entries, entityPredicate);
                            return true;
                        }),
                    Delay = TimeSpan.FromMilliseconds(initialDelayMs),
                    MaxRetryAttempts = maxRetryAttempts
                })
            .Build();

        var res = await resiliencePipeline.ExecuteAsync(executionFunction, cancellationToken);

        return res;
    }

    /// <summary>
    /// Safely update entry state before retrying. If the target entry cannot be located nothing is changed.
    /// This avoids exceptions from Single() and makes the retry logic more robust when entry lists vary.
    /// </summary>
    private static void UpdateEntryStateSafe(IReadOnlyList<EntityEntry> entries, Func<EntityEntry, bool> predicate)
    {
        if (entries == null || predicate == null) return;

        var entry = entries.FirstOrDefault(predicate);
        if (entry == null) return;

        // Detach so that a subsequent retry can attempt an update instead of re-inserting the same entity
        entry.State = EntityState.Detached;
    }
}
