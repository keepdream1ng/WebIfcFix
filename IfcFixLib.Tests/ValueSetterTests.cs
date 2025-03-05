using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;
using IfcFixLib.PipelineFilters;

namespace IfcFixLib.Tests;
public class ValueSetterTests(TestFileFixture testFile) : IClassFixture<TestFileFixture>
{
	[Fact]
	public async Task ProcessAsync_ShouldUpdateDatabaseElements_Name()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcElement> allElements = FilterResetter.ExtractAllElements(db);
		List<IfcElement> beams = allElements
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		StringValueSetter strategy = new();
		strategy.ValueType = ElementStringValueType.Name;
		string expected = "updatedBeamName";
		strategy.Value = expected;
		ValueSetter setter = new ValueSetter(strategy);

		setter.Input = new DataIFC(db, beams);
		var filterResetter = new FilterResetter();
		var dublicator = new DbDuplicator();
		var dbSerializer = new DbSerializer(IfcFormatOutput.STEP);
		setter.PipeInto(filterResetter)
			.PipeInto(dublicator)
			.PipeInto(dbSerializer);

		// Act
		await setter.ProcessAsync(CancellationToken.None);

		string actualStepString = dbSerializer.Output!;
		DatabaseIfc updatedDb = DatabaseIfc.ParseString(actualStepString);
		List<IfcElement> actual = FilterResetter.ExtractAllElements(updatedDb);

		List<IfcElement> actualUpdatedBeams = actual
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		// Assert
		Assert.Contains(expected, actualStepString);
		Assert.All(actualUpdatedBeams, ifcElement =>
		{
			Assert.Equal(expected, ifcElement.Name);
		});
	}

	[Fact]
	public async Task ProcessAsync_ShouldUpdateDatabaseElements_Tag()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcElement> allElements = FilterResetter.ExtractAllElements(db);
		List<IfcElement> beams = allElements
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		StringValueSetter strategy = new();
		strategy.ValueType = ElementStringValueType.Tag;
		string expected = "updatedBeamTag";
		strategy.Value = expected;
		ValueSetter setter = new ValueSetter(strategy);

		setter.Input = new DataIFC(db, beams);
		var filterResetter = new FilterResetter();
		var dublicator = new DbDuplicator();
		var dbSerializer = new DbSerializer(IfcFormatOutput.STEP);
		setter.PipeInto(filterResetter)
			.PipeInto(dublicator)
			.PipeInto(dbSerializer);

		// Act
		await setter.ProcessAsync(CancellationToken.None);

		string actualStepString = dbSerializer.Output!;
		DatabaseIfc updatedDb = DatabaseIfc.ParseString(actualStepString);
		List<IfcElement> actual = FilterResetter.ExtractAllElements(updatedDb);

		List<IfcElement> actualUpdatedBeams = actual
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		// Assert
		Assert.Contains(expected, actualStepString);
		Assert.All(actualUpdatedBeams, ifcElement =>
		{
			Assert.Equal(expected, ifcElement.Tag);
		});
	}

	[Fact]
	public async Task ProcessAsync_ShouldUpdateDatabaseElements_Description()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcElement> allElements = FilterResetter.ExtractAllElements(db);
		List<IfcElement> beams = allElements
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		StringValueSetter strategy = new();
		strategy.ValueType = ElementStringValueType.Description;
		string expected = "updatedBeamDescription";
		strategy.Value = expected;
		ValueSetter setter = new ValueSetter(strategy);

		setter.Input = new DataIFC(db, beams);
		var filterResetter = new FilterResetter();
		var dublicator = new DbDuplicator();
		var dbSerializer = new DbSerializer(IfcFormatOutput.STEP);
		setter.PipeInto(filterResetter)
			.PipeInto(dublicator)
			.PipeInto(dbSerializer);

		// Act
		await setter.ProcessAsync(CancellationToken.None);

		string actualStepString = dbSerializer.Output!;
		DatabaseIfc updatedDb = DatabaseIfc.ParseString(actualStepString);
		List<IfcElement> actual = FilterResetter.ExtractAllElements(updatedDb);

		List<IfcElement> actualUpdatedBeams = actual
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		// Assert
		Assert.Contains(expected, actualStepString);
		Assert.All(actualUpdatedBeams, ifcElement =>
		{
			Assert.Equal(expected, ifcElement.Description);
		});
	}

	[Fact]
	public async Task ProcessAsync_ShouldUpdateDatabaseElements_ExistingProperty_WithPrefixAndPostfix()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		string originalIfcString = reader.ReadToEnd();
		DatabaseIfc db = DatabaseIfc.ParseString(originalIfcString);
		List<IfcElement> allElements = FilterResetter.ExtractAllElements(db);
		List<IfcElement> builtElements = allElements
			.Where(x => x is IfcBuiltElement)
			.ToList();

		DatabaseIfc notModifiedDb = DatabaseIfc.ParseString(originalIfcString);
		List<IfcElement> nonModiedElements = FilterResetter.ExtractAllElements(notModifiedDb);

		StringValueSetter strategy = new();
		strategy.ValueType = ElementStringValueType.Property;
		strategy.PropertyName = "Top elevation";
		string expectedPrefix = "testPrefix";
		string expectedPostfix = "testPostfix";
		strategy.Value = expectedPrefix + "_{VALUE}_" + expectedPostfix;
		ValueSetter setter = new ValueSetter(strategy);

		setter.Input = new DataIFC(db, builtElements);
		var dublicator = new DbDuplicator();
		var resetter = new FilterResetter();
		var dbSerializer = new DbSerializer(IfcFormatOutput.STEP);
		setter.PipeInto(resetter)
			.PipeInto(dublicator)
			.PipeInto(dbSerializer);

		// Act
		await setter.ProcessAsync(CancellationToken.None);

		string actualStepString = dbSerializer.Output!;
		DatabaseIfc updatedDb = DatabaseIfc.ParseString(actualStepString);
		List<IfcElement> actual = FilterResetter.ExtractAllElements(updatedDb);

		List<IfcElement> actualUpdatedBuiltElements = actual
			.Where(x => x is IfcBuiltElement)
			.ToList();

		List<IfcElement> actualUpdatedElementsOnLvl1000 = actualUpdatedBuiltElements
			.Where(x => x.Name.Contains("1000"))
			.ToList();

		var propertyLvl1000 = actualUpdatedElementsOnLvl1000.First().FindProperty(strategy.PropertyName) as IfcPropertySingleValue;

		// Assert
		Assert.Contains(expectedPrefix, actualStepString);
		Assert.Contains(expectedPostfix, actualStepString);
		Assert.All(actualUpdatedBuiltElements, ifcElement =>
		{
			var oldElementVersion = nonModiedElements.Single(x => x.GlobalId == ifcElement.GlobalId);
			var oldElementProperty = oldElementVersion.FindProperty(strategy.PropertyName) as IfcPropertySingleValue;
			string expected = strategy.Value.Replace("{VALUE}", oldElementProperty!.NominalValue.ValueString);
			var updatedElementProperty = ifcElement.FindProperty(strategy.PropertyName) as IfcPropertySingleValue;
			Assert.Equal(expected, updatedElementProperty!.NominalValue.ValueString);
		});
		Assert.Equal(actualUpdatedElementsOnLvl1000.Count, propertyLvl1000!.PartOfPset.Count);
	}

	[Fact]
	public async Task ProcessAsync_ShouldUpdateDatabaseElements_ExistingProperty()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcElement> allElements = FilterResetter.ExtractAllElements(db);
		List<IfcElement> beams = allElements
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		StringValueSetter strategy = new();
		strategy.ValueType = ElementStringValueType.Property;
		strategy.PropertyName = "Finish";
		string expected = "newFinishPropertyContent";
		strategy.Value = expected;
		ValueSetter setter = new ValueSetter(strategy);

		setter.Input = new DataIFC(db, beams);
		var dublicator = new DbDuplicator();
		var resetter = new FilterResetter();
		var dbSerializer = new DbSerializer(IfcFormatOutput.STEP);
		setter.PipeInto(resetter)
			.PipeInto(dublicator)
			.PipeInto(dbSerializer);

		// Act
		await setter.ProcessAsync(CancellationToken.None);

		string actualStepString = dbSerializer.Output!;
		DatabaseIfc updatedDb = DatabaseIfc.ParseString(actualStepString);
		List<IfcElement> actual = FilterResetter.ExtractAllElements(updatedDb);

		List<IfcElement> actualUpdatedBeams = actual
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		List<IfcElement> actualAllTheRest = actual
			.Except(actualUpdatedBeams)
			.ToList();

		// Assert
		Assert.Contains(expected, actualStepString);
		Assert.All(actualUpdatedBeams, ifcElement =>
		{
			var elementProperty = ifcElement.FindProperty(strategy.PropertyName) as IfcPropertySingleValue;
			Assert.Equal(expected, elementProperty!.NominalValue.ValueString);
		});
		Assert.All(actualAllTheRest, ifcElement =>
		{
			var elementProperty = ifcElement.FindProperty(strategy.PropertyName) as IfcPropertySingleValue;
			if (elementProperty is not null)
			{
				Assert.NotEqual(expected, elementProperty!.NominalValue.ValueString);
			}
		});
	}

	[Fact]
	public async Task ProcessAsync_ShouldUpdateDatabaseElements_NewProperty()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcElement> allElements = FilterResetter.ExtractAllElements(db);
		List<IfcElement> beams = allElements
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		StringValueSetter strategy = new();
		strategy.ValueType = ElementStringValueType.Property;
		string expected = "newPropertyContent";
		string expectedPropertyName = "MyNewProperty";
		strategy.PropertyName = expectedPropertyName;
		strategy.Value = expected;
		ValueSetter setter = new ValueSetter(strategy);

		setter.Input = new DataIFC(db, beams);
		var filterResetter = new FilterResetter();
		var dublicator = new DbDuplicator();
		var dbSerializer = new DbSerializer(IfcFormatOutput.STEP);
		setter.PipeInto(filterResetter)
			.PipeInto(dublicator)
			.PipeInto(dbSerializer);

		// Act
		await setter.ProcessAsync(CancellationToken.None);
		string actual = dbSerializer.Output!;
		DatabaseIfc updatedDb = DatabaseIfc.ParseString(actual);
		List<IfcElement> actualElements = FilterResetter.ExtractAllElements(updatedDb);

		List<IfcElement> actualUpdatedBeams = actualElements 
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		List<IfcElement> actualAllTheRest = actualElements 
			.Except(actualUpdatedBeams)
			.ToList();

		// Assert
		Assert.Contains(expected, actual);
		Assert.Contains(expectedPropertyName, actual);

		Assert.All(actualUpdatedBeams, ifcElement =>
		{
			var elementProperty = ifcElement.FindProperty(expectedPropertyName) as IfcPropertySingleValue;
			Assert.Equal(expected, elementProperty!.NominalValue.ValueString);
		});
		Assert.All(actualAllTheRest, ifcElement =>
		{
			Assert.Null((IfcPropertySingleValue)ifcElement.FindProperty(expectedPropertyName));
		});
	}
}
