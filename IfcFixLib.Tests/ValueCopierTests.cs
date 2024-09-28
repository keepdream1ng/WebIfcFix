using GeometryGym.Ifc;
using IfcFixLib.PipelineFilters;
using IfcFixLib.IfcPipelineDefinition;
using IfcFixLib.FilterStrategy;

namespace IfcFixLib.Tests;

public class ValueCopierTests(TestFileFixture testFile) : IClassFixture<TestFileFixture>
{
	[Fact]
	public async Task ProcessAsync_ShouldUpdateDatabaseElements_Description_From_Tag()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcBuiltElement> allElements = db.Project.Extract<IfcBuiltElement>();
		List<IfcBuiltElement> beams = allElements
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		StringValueGetter valueGetter = new StringValueGetter();
		valueGetter.ValueType = ElementStringValueType.Tag;

		StringValueSetter valueSetter = new();
		valueSetter.ValueType = ElementStringValueType.Description;

		ValueCopier copier = new ValueCopier(valueGetter, valueSetter);

		copier.Input = new DataIFC(db, beams);
		var dublicator = new DbDuplicator();
		var resetter = new FilterResetter();
		var dbSerializer = new DbSerializer(IfcFormatOutput.STEP);
		copier.PipeInto(resetter)
			.PipeInto(dublicator)
			.PipeInto(dbSerializer);

		// Act
		await copier.ProcessAsync(CancellationToken.None);

		List<IfcBuiltElement> actual = dublicator.Output!.DatabaseIfc.Project.Extract<IfcBuiltElement>();

		List<IfcBuiltElement> actualUpdatedBeams = actual
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		List<IfcBuiltElement> actualAllTheRest = actual
			.Except(actualUpdatedBeams)
			.ToList();

		// Assert
		Assert.All(actualUpdatedBeams, ifcElement =>
		{
			Assert.Equal(ifcElement.Tag, ifcElement.Description);
		});
		Assert.All(actualAllTheRest, ifcElement =>
		{
			Assert.NotEqual(ifcElement.Tag, ifcElement.Description);
		});
	}

	[Fact]
	public async Task ProcessAsync_ShouldUpdateDatabaseElements_Property_From_Property()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcBuiltElement> allElements = db.Project.Extract<IfcBuiltElement>();
		List<IfcBuiltElement> beams = allElements
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		StringValueGetter valueGetter = new StringValueGetter();
		valueGetter.ValueType = ElementStringValueType.Property;
		valueGetter.PropertyName = "Class";

		StringValueSetter valueSetter = new();
		valueSetter.ValueType = ElementStringValueType.Property;
		valueSetter.PropertyName = "Finish";

		ValueCopier copier = new ValueCopier(valueGetter, valueSetter);

		copier.Input = new DataIFC(db, beams);
		var dublicator = new DbDuplicator();
		var resetter = new FilterResetter();
		var dbSerializer = new DbSerializer(IfcFormatOutput.STEP);
		copier.PipeInto(resetter)
			.PipeInto(dublicator)
			.PipeInto(dbSerializer);

		// Act
		await copier.ProcessAsync(CancellationToken.None);

		List<IfcBuiltElement> actual = dublicator.Output!.DatabaseIfc.Project.Extract<IfcBuiltElement>();

		List<IfcBuiltElement> actualUpdatedBeams = actual
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		List<IfcBuiltElement> actualAllTheRest = actual
			.Except(actualUpdatedBeams)
			.ToList();

		// Assert
		Assert.All(actualUpdatedBeams, ifcElement =>
		{
			var elementGetterProperty = ifcElement.FindProperty(valueGetter.PropertyName) as IfcPropertySingleValue;
			var elementSetterProperty = ifcElement.FindProperty(valueSetter.PropertyName) as IfcPropertySingleValue;
			Assert.Equal(elementGetterProperty!.NominalValue.ValueString, elementSetterProperty!.NominalValue.ValueString);
		});
		Assert.All(actualAllTheRest, ifcElement =>
		{
			var elementGetterProperty = ifcElement.FindProperty(valueGetter.PropertyName) as IfcPropertySingleValue;
			var elementSetterProperty = ifcElement.FindProperty(valueSetter.PropertyName) as IfcPropertySingleValue;
			Assert.NotEqual(elementGetterProperty!.NominalValue.ValueString, elementSetterProperty!.NominalValue.ValueString);
		});
	}
}
