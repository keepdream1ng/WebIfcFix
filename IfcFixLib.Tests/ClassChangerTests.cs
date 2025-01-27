
using GeometryGym.Ifc;
using IfcFixLib.FilterStrategy;
using IfcFixLib.IfcPipelineDefinition;
using IfcFixLib.PipelineFilters;

namespace IfcFixLib.Tests;
public class ClassChangerTests(TestFileFixture testFile) : IClassFixture<TestFileFixture>
{
	[Fact]
	public async Task ProcessAsync_Should_CreateSelectedClass_and_CopyProperties()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcElement> allElements = FilterResetter.ExtractAllElements(db);
		List<IfcElement> beam0 = allElements
			.Where(x => x is not IfcElementAssembly)
			.Where(x => x.Name.Equals("TestBeam0", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		IfcElement expectedBeam0 = beam0.First();
		string expectedPropertyName = "Top elevation";
		var expectedPropertyValue = beam0[0].FindProperty(expectedPropertyName) as IfcPropertySingleValue;
		var options = new ClassChangerOptions();
		string expectedClassName = "IfcFlowSegment";
		options.TargetClassName = expectedClassName; 

		var classChanger = new ClassChanger(options);

		classChanger.Input = new DataIFC(db, beam0);
		var resetter = new FilterResetter();
		var dbSerializer = new DbSerializer(IfcFormatOutput.STEP);
		classChanger.PipeInto(resetter)
			.PipeInto(dbSerializer);

		// Act
		await classChanger.ProcessAsync(CancellationToken.None);

		List<IfcElement> actual = FilterResetter.ExtractAllElements(DatabaseIfc.ParseString(dbSerializer.Output!));

		IfcElement actualUpdatedBeam0 = actual
			.Where(x => x is not IfcElementAssembly)
			.Single(x => x.Name.Equals("TestBeam0", StringComparison.InvariantCultureIgnoreCase));

		var actualPropertyValue = actualUpdatedBeam0.FindProperty(expectedPropertyName) as IfcPropertySingleValue;

		// Assert
		Assert.Equal(allElements.Count, actual.Count);
		Assert.Equal(expectedClassName, actualUpdatedBeam0.StepClassName);
		Assert.NotNull(actualPropertyValue);
		Assert.Equal(expectedPropertyValue!.NominalValue.ValueString, actualPropertyValue.NominalValue.ValueString);
		Assert.Equal(expectedBeam0.GlobalId, actualUpdatedBeam0.GlobalId);
	}

	[Fact]
	public async Task ProcessAsync_Should_ReturnError_for_WrongClassName()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcElement> allElements = FilterResetter.ExtractAllElements(db);

		var options = new ClassChangerOptions();
		string expectedClassName = "notIfcClass";
		options.TargetClassName = expectedClassName; 

		var classChanger = new ClassChanger(options);

		classChanger.Input = new DataIFC(db, allElements);
		var resetter = new FilterResetter();

		// Act
		// Assert
		await Assert.ThrowsAsync<ArgumentException>(async () =>
		await classChanger.ProcessAsync(CancellationToken.None));
	}
}
