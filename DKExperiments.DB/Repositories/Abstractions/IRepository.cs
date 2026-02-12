namespace DKExperiments.DB.Repositories.Abstractions;

public interface IRepository<M, S>
{
	/// <summary>
	/// Load one record by specified filters
	/// </summary>
	/// <param name="searchModel">Filters model</param>
	/// <returns></returns>
	Task<M?> GetAsync(S searchModel, CancellationToken cancellationToken);

	/// <summary>
	/// Save an item in teh database - update or insert
	/// </summary>
	/// <param name="record">Record model</param>
	/// <returns></returns>
	Task<M?> SaveAsync(M record, bool timeout, CancellationToken cancellationToken);

	/// <summary>
	/// Load list of records by filters
	/// </summary>
	/// <param name="searchModel">Filters model</param>
	/// <returns></returns>
	Task<List<M>> ListAsync(S searchModel, CancellationToken cancellationToken);
}
