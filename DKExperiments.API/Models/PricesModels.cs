using System.ComponentModel.DataAnnotations;
using DKExperiments.Core.Models;
using DKExperiments.API.Validation;

namespace DKExperiments.API.Models;

public record SinglePriceRequestModel(
	[Required, AvoidFutureDate] DateTime Timestamp,
	[Required, DeniedValues(Markets.Undefined)] Markets Market
);

public record PriceRangeRequestModel(
	[Required, AvoidFutureDate] DateTime Start,
	[BiggerDateOnly(nameof(PriceRangeRequestModel.Start))] DateTime End,
	[Required, DeniedValues(Markets.Undefined)] Markets Market
);
