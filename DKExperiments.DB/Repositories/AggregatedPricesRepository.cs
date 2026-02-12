using System.Data;
using DKExperiments.DB.Models.Prices;
using DKExperiments.DB.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DKExperiments.DB.Repositories;

public class AggregatedPricesRepository(DKDBContext dbContext) : ResilientRepository, IAggregatedPricesRepository
{
	public Task<AggregatedPrice?> GetAsync(AggregatedPriceSearchModel searchModel, CancellationToken cancellationToken) =>
		dbContext.AggregatedPrices.FirstOrDefaultAsync(x =>
			(!searchModel.Start.HasValue || x.Timestamp >= searchModel.Start.Value)
			&& (!searchModel.End.HasValue || x.Timestamp <= searchModel.End.Value)
			&& (!searchModel.Market.HasValue || x.Market == searchModel.Market.Value),
			cancellationToken);

	public Task<List<AggregatedPrice>> ListAsync(AggregatedPriceSearchModel searchModel, CancellationToken cancellationToken) =>
		dbContext.AggregatedPrices.Where(x =>
			(!searchModel.Start.HasValue || x.Timestamp >= searchModel.Start.Value)
			&& (!searchModel.End.HasValue || x.Timestamp <= searchModel.End.Value)
			&& (!searchModel.Market.HasValue || x.Market == searchModel.Market.Value)
		).ToListAsync(cancellationToken);

	#region Save concurrency inplementation #1

	//public async Task<AggregatedPrice?> SaveAsync(AggregatedPrice record, bool timeout, CancellationToken cancellationToken)
	//{
	//	return await SaveRecordResilientAsync(
	//		x =>
	//			x.Entity is AggregatedPrice entity
	//			&& entity.Market == record.Market
	//			&& entity.Timestamp == record.Timestamp,
	//		async x => await SaveRecordAsync(record, timeout, cancellationToken),
	//		cancellationToken);
	//}

	//private async Task<AggregatedPrice?> SaveRecordAsync(AggregatedPrice record, bool timeout, CancellationToken cancellationToken)
	//{
	//	var item = await GetAsync(new AggregatedPriceSearchModel(record.Timestamp, record.Timestamp, record.Market), cancellationToken);

	//	if (item != null && item.Final)
	//	{
	//		// Legally can happen only when inserting data simultaneously in different requests
	//		return item;
	//	}

	//	var create = item == null;

	//	item ??= new AggregatedPrice();

	//	item.Timestamp = record.Timestamp;
	//	item.Market = record.Market;
	//	item.Final = record.Final;
	//	// Uncomment all later on if needed
	//	//item.Open = record.Open;
	//	//item.High = record.High;
	//	//item.Low = record.Low;
	//	item.Close = record.Close;
	//	item.LastUpdated = DateTime.UtcNow;

	//	if (create)
	//	{
	//		dbContext.AggregatedPrices.Add(item);
	//	}
	//	else
	//	{
	//		dbContext.AggregatedPrices.Update(item);
	//	}

	//	if (timeout)
	//	{
	//		await Task.Delay(1000, cancellationToken);
	//	}

	//	await dbContext.SaveChangesAsync(cancellationToken);

	//	return item;
	//}

	#endregion

	#region Save concurrency inplementation #2

	public async Task<AggregatedPrice?> SaveAsync(AggregatedPrice record, bool timeout, CancellationToken cancellationToken)
	{
		var item = await GetAsync(new AggregatedPriceSearchModel(record.Timestamp, record.Timestamp, record.Market), cancellationToken);

		if (item != null && item.Final)
		{
			// Legally can happen only when inserting data simultaneously in different requests
			return item;
		}

		var create = item == null;

		item ??= new AggregatedPrice();

		item.Timestamp = record.Timestamp;
		item.Market = record.Market;
		item.Final = record.Final;
		// Uncomment all later on if needed
		//item.Open = record.Open;
		//item.High = record.High;
		//item.Low = record.Low;
		item.Close = record.Close - (timeout ? 90000 : 0);
		item.LastUpdated = DateTime.UtcNow;

		if (create)
		{
			dbContext.AggregatedPrices.Add(item);
		}
		else
		{
			dbContext.AggregatedPrices.Update(item);
		}

		if (timeout)
		{
			await Task.Delay(1000, cancellationToken);
		}

		await dbContext.SaveChangesResilientAsync(
			x =>
				x.Entity is AggregatedPrice entity
				&& entity.Market == record.Market
				&& entity.Timestamp == record.Timestamp,
			cancellationToken);

		return item;
	}

	#endregion
}
