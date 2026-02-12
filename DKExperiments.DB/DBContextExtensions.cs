using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql;

namespace DKExperiments.DB;

public static class DBContextExtensions
{
	public static async Task<int> SaveChangesResilientAsync(
		this DbContext dbContext,
		Func<EntityEntry, bool> entrySearchPredicate,
		CancellationToken cancellationToken)
	{
		var changesSaved = false;
		var retries = 1;
		var res = 0;

		while (!changesSaved && retries > -1)
		{
			try
			{
				res = await dbContext.SaveChangesAsync(cancellationToken);

				changesSaved = true;
			}
			catch (Exception ex) when (
				ex is DbUpdateConcurrencyException
				||
				(
					ex is DbUpdateException dbuExc
					&& dbuExc.InnerException != null
					&& (dbuExc.InnerException is PostgresException pgex)
					&& dbuExc.Entries.Single().State == EntityState.Added
					&& (pgex.Routine?.Equals("_bt_check_unique", StringComparison.OrdinalIgnoreCase) ?? false)
				))
			{
				var entries = ex is DbUpdateException dbu ? dbu.Entries : ex is DbUpdateConcurrencyException dbuc ? dbuc.Entries : [];
				var shouldRetry = await UpdateEntriesBeforeRetryAsync(entries, entrySearchPredicate);

				if (shouldRetry && retries == 0)
				{
					throw;
				}

				await Task.Delay(300, cancellationToken);
				retries--;
			}
		}

		return res;
	}

	private static async Task<bool> UpdateEntriesBeforeRetryAsync(
		IReadOnlyList<EntityEntry> entries,
		Func<EntityEntry, bool> entrySearchPredicate)
	{
		// Get current entry from the list of all changes
		// Making a selection here in case we have multiple changes in the context
		var entry = entries.Where(entrySearchPredicate).Single();

		// Update state in case it is a new record and there was a PK error on insert
		// So the record won't be saved as new again and Update will be executed instead
		entry.State = entry.State == EntityState.Added ? EntityState.Modified : entry.State;

		// Get actual database field values
		var databaseValues = await entry.GetDatabaseValuesAsync();
		if (databaseValues == null)
		{
			return false;
		}

		// Choose an initial set of resolved values. In this case we 
		// make the default value the values currently in the database.
		var resolvedEntity = databaseValues.Clone();

		// Update all the fields 
		List<string> technicalFields = ["Version"];

		foreach (var prop in entry.CurrentValues.Properties.Where(p => !technicalFields.Contains(p.Name)))
		{
			resolvedEntity[prop.Name] = entry.CurrentValues[prop.Name];
		}

		// Update the original values with the database values and 
		// the current values with whatever the user chooses. 
		entry.OriginalValues.SetValues(databaseValues);
		entry.CurrentValues.SetValues(resolvedEntity);

		return true;
	}
}
