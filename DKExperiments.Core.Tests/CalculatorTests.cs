using DKExperiments.Core.Calculations;
using DKExperiments.Tests.Base;

namespace DKExperiments.Core.Tests;

[Collection("Config collection")]
public class CalculatorTests(DKConfigFixture fixture)
{
	[Fact]
	public void TestCalculatorAVG()
	{
		var calc = new Calculator(fixture.Config);
		var avg = calc.CalculateAvg([10, 20]);

		Assert.Equal(15, avg);
	}

	[Fact]
	public void TestBasicAVG()
	{
		var func = new BasicAvgFunction();
		var avg = func.Calculate([10, 20]);

		Assert.Equal(15, avg);
	}
}