using DKExperiments.DB.Models.Prices;

namespace DKExperiments.DB.Repositories.Abstractions;

// These interfaces can be used to implement a much cleaner architecture by splitting up real database implementation from abstractions
// But it only makes sense to do if tehre's a need in having a possibility to switch between different data sources

// P.S.: I have created them just in case

public interface IAggregatedPricesRepository : IRepository<AggregatedPrice, AggregatedPriceSearchModel>
{ }
