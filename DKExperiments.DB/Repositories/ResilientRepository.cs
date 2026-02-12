using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql;
using Polly;
using Polly.Retry;

namespace DKExperiments.DB.Repositories;

public class ResilientRepository
{
	protected static internal async Task<T?> SaveRecordResilientAsync<T>(
		Func<EntityEntry, bool> entityPredicate,
		Func<CancellationToken, ValueTask<T?>> executionFunction,
		CancellationToken cancellationToken)
	{
		var resiliencePipeline = new ResiliencePipelineBuilder()
			.AddRetry(
				new RetryStrategyOptions
				{
					ShouldHandle = new PredicateBuilder()
						.Handle((DbUpdateException ex) =>
						{
							var shouldRetry = ex.InnerException != null
								&& (ex.InnerException is PostgresException pex)
								&& ex.Entries.Single().State == EntityState.Added
								&& (pex.Routine?.Equals("_bt_check_unique", StringComparison.OrdinalIgnoreCase) ?? false);

							if (shouldRetry)
							{
								UpdateEntryState(ex.Entries, entityPredicate);
							}

							return shouldRetry;
						})
						.Handle((DbUpdateConcurrencyException ex) =>
						{
							UpdateEntryState(ex.Entries, entityPredicate);
							return true;
						}),
					Delay = TimeSpan.FromMilliseconds(200),
					MaxRetryAttempts = 1
				})
			.Build();

		var res = await resiliencePipeline.ExecuteAsync(executionFunction, cancellationToken);

		return res;
	}

	private static void UpdateEntryState(IReadOnlyList<EntityEntry> entries, Func<EntityEntry, bool> predicate)
	{
		var entry = entries.Where(predicate).Single();
		entry.State = EntityState.Detached;
	}
}
