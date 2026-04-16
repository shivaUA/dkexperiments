namespace DKExperiments.Tests.Base;

public sealed class DefaultHttpClientFactory : IHttpClientFactory
{
	public HttpClient CreateClient(string name) => new();
}