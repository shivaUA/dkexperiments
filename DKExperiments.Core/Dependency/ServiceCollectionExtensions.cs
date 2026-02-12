using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;
using DKExperiments.Core.Calculations;
using DKExperiments.Core.Calculations.Abstractions;
using DKExperiments.Core.Services;
using DKExperiments.Core.Services.Abstractions;

namespace DKExperiments.Core.Dependency;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCoreServices(this IServiceCollection services)
	{
		// Calculation services
		services.AddScoped<ICalculator, Calculator>();

		// Price parsing services
		// Specify as many as you need, they will then be loaded and processed one by one to get an average price (if that is needed)
		services.AddKeyedScoped<IPricesParcer, BitstampParser>("IPricesParcer");
		services.AddKeyedScoped<IPricesParcer, BitfinexParser>("IPricesParcer");

		// Main price manager
		services.AddScoped<IPricesManager, PricesManager>();

		// Configure HttpClient with Resilience strategy
		services.AddHttpClient().AddResiliencePipeline("coreHttpClient", x => x
			.AddRetry(
				new RetryStrategyOptions
				{
					ShouldHandle = new PredicateBuilder()
						.Handle<HttpRequestException>()
						.Handle<TimeoutException>(),
					Delay = TimeSpan.FromMilliseconds(500),
					MaxRetryAttempts = 3,
					BackoffType = DelayBackoffType.Linear,
					UseJitter = true
				})
			.AddTimeout(TimeSpan.FromSeconds(20)));

		return services;
	}
}
