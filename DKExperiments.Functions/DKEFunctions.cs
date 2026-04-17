using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DKExperiments.Functions;

public class DKEFunctions(ILogger<DKEFunctions> logger)
{
	private readonly ILogger<DKEFunctions> _logger = logger;

	[Function("HttpFunction")]
	public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "test-func")] HttpRequest req)
	{
		_logger.LogInformation("C# HTTP trigger function processed a request.");
		return new OkObjectResult
		(
			new
			{
				Result = "Welcome to Azure Functions!",
				Timestamp = DateTime.UtcNow,
				req.ContentLength
			}
		);
	}
}