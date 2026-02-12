using DKExperiments.API.Models;
using DKExperiments.Core.Models;
using DKExperiments.Core.Services.Abstractions;

namespace DKExperiments.API.Endpoints;

internal static class PricesEndpoints
{
	public static RouteGroupBuilder MapPriceRoutes(this RouteGroupBuilder routes)
	{
		routes.MapPost("/load-single", LoadSingle)
			.WithName("Load single hour price")
			.WithDescription("Load price at a specific moment. Note that if price is not loaded yet - external APIs will be called which might increase response time")
			.Produces<PriceInfo>(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest)
			.Produces(StatusCodes.Status404NotFound);

		routes.MapPost("/load-single-timeout", LoadSingleTimeout)
			.WithName("Load single hour price with timeout before saving")
			.WithDescription("Load price at a specific moment. Note that if price is not loaded yet - external APIs will be called which might increase response time. This is a test one needed for EF concurrency testing")
			.Produces<PriceInfo>(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest)
			.Produces(StatusCodes.Status404NotFound);

		routes.MapPost("/load-range", LoadRange)
			.WithName("Load prices list")
			.WithDescription("Load list of prices with 1-hour step between two specified dates. No call to external API is going to be performed.")
			.Produces<PriceInfo>(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest);

		return routes;
	}

	public static async Task<IResult> LoadSingle(IPricesManager pricesManager, SinglePriceRequestModel model, CancellationToken cancellationToken)
	{
		var price = await pricesManager.LoadSingleAsync(model.Market, model.Timestamp, false, cancellationToken);

		return price == null ? Results.NotFound() : Results.Ok(price);
	}

	public static async Task<IResult> LoadSingleTimeout(IPricesManager pricesManager, SinglePriceRequestModel model, CancellationToken cancellationToken)
	{
		var price = await pricesManager.LoadSingleAsync(model.Market, model.Timestamp, true, cancellationToken);

		return price == null ? Results.NotFound() : Results.Ok(price);
	}

	public static async Task<IResult> LoadRange(IPricesManager pricesManager, PriceRangeRequestModel model, CancellationToken cancellationToken)
	{
		var res = await pricesManager.LoadRangeAsync(model.Market, model.Start, model.End, cancellationToken);

		return Results.Ok(res);
	}
}
