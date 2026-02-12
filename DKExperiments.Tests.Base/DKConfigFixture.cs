using Microsoft.Extensions.Configuration;

namespace DKExperiments.Tests.Base;

public class DKConfigFixture : IDisposable
{
	public IConfiguration Config { get; }

	public DKConfigFixture()
	{
		var cb = new ConfigurationBuilder();
		cb.AddJsonFile("appsettings.json");
		Config = cb.Build();
	}

	public void Dispose() => GC.SuppressFinalize(this);
}
