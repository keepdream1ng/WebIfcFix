using GeometryGym.Ifc;
using IfcFixLib.FilterStrategy;
using IfcFixLib.PipelineFilters;

namespace IfcFixLib.Tests;
public class ElementsFilterTests(TestFileFixture testFile) : IClassFixture<TestFileFixture>
{
	[Fact]
	public async Task ProcessAsync_ShouldFilterElements_By_ElementClassName()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcElement> allElements = FilterResetter.ExtractAllElements(db);

		var strategy = new FilterClassNameStrategy();
		strategy.FilterInClassName = ClassNamesGetter.IfcClassNames
			.Where(name => name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.First();

		ElementsFilter filter = new ElementsFilter(strategy);
		filter.Input = new DataIFC(db, allElements);

		// Act
		await filter.ProcessAsync(CancellationToken.None);

		// Assert
		Assert.NotNull(filter.Output);
		Assert.All(filter.Output.Elements, ifcElement =>
		{
			Assert.Equal(ifcElement.StepClassName, strategy.FilterInClassName);
		});
	}
}
