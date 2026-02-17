using System.Text.Json.Nodes;
using DKExperiments.API.Converters;
using DKExperiments.API.Endpoints;
using DKExperiments.Core.Dependency;
using DKExperiments.Core.Models;
using DKExperiments.DB;
using DKExperiments.DB.Dependency;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddDBServices(builder.Configuration) // Register database services: database, repositories
	.AddCoreServices(); // Register core services: price parsers, data handling services etc.

// Register our own DateTime converter to keep a proper format
builder.Services
	.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.Converters.Add(new DateTimeJsonConverter()));

builder.Services.Configure<ParsersConfigModel>(builder.Configuration.GetSection("PriceAPIs"));

// Setup Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services
	.AddSwaggerGen(opts =>
	{
		opts.SwaggerDoc("v2", new OpenApiInfo { Title = "Experiments API - V1", Version = "v1" });

		opts.EnableAnnotations();

		opts.MapType<Markets>(() => new OpenApiSchema
		{
			Type = JsonSchemaType.Integer,
			Example = JsonValue.Create((int)Markets.BTCUSD),
			Enum = [.. Enum.GetValues<Markets>()
				.Where(x => x != Markets.Undefined)
				.Select(x => JsonValue.Create($"[{(int)x}] {x}") as JsonNode)],
		});

		opts.MapType<DateTime>(() => new OpenApiSchema
		{
			Type = JsonSchemaType.String,
			Example = JsonValue.Create(DateTime.UtcNow.ToString("yyyy-MM-dd HH")),
			Description = "Date with only hours, no minutes, seconds etc.",
			Format = "yyyy-MM-dd HH"
		});
	});

var app = builder.Build();

// Register minimal API
app
	.MapGroup("/prices")
	.MapPriceRoutes();

// Users endpoints
app
	.MapGroup("/users")
	.MapUserRoutes();

// Run DB migrations
DKDBContext.Migrate(app.Services);

// Configure Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v2/swagger.json", "Experiments API - V1");
});

app.Run();

public partial class Program { }
