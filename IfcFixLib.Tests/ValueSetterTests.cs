﻿using GeometryGym.Ifc;
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
		List<IfcBuiltElement> allElements = db.Project.Extract<IfcBuiltElement>();
		List<IfcBuiltElement> beams = allElements
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		StringValueSetter strategy = new();
		strategy.ValueType = ElementStringValueType.Name;
		string expected = "updatedBeamName";
		strategy.Value = expected;
		ValueSetter setter = new ValueSetter(strategy);

		setter.Input = new DataIFC(db, beams);

		// Act
		await setter.ProcessAsync(CancellationToken.None);

		List<IfcBuiltElement> actual = setter.Output!.DatabaseIfc.Project.Extract<IfcBuiltElement>();
		string actualStepString = setter.Output!.DatabaseIfc.ToString(FormatIfcSerialization.STEP);

		List<IfcBuiltElement> actualUpdatedBeams = actual
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
		List<IfcBuiltElement> allElements = db.Project.Extract<IfcBuiltElement>();
		List<IfcBuiltElement> beams = allElements
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		StringValueSetter strategy = new();
		strategy.ValueType = ElementStringValueType.Tag;
		string expected = "updatedBeamTag";
		strategy.Value = expected;
		ValueSetter setter = new ValueSetter(strategy);

		setter.Input = new DataIFC(db, beams);

		// Act
		await setter.ProcessAsync(CancellationToken.None);

		List<IfcBuiltElement> actual = setter.Output!.DatabaseIfc.Project.Extract<IfcBuiltElement>();
		string actualStepString = setter.Output!.DatabaseIfc.ToString(FormatIfcSerialization.STEP);

		List<IfcBuiltElement> actualUpdatedBeams = actual
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
		List<IfcBuiltElement> allElements = db.Project.Extract<IfcBuiltElement>();
		List<IfcBuiltElement> beams = allElements
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		StringValueSetter strategy = new();
		strategy.ValueType = ElementStringValueType.Description;
		string expected = "updatedBeamDescription";
		strategy.Value = expected;
		ValueSetter setter = new ValueSetter(strategy);

		setter.Input = new DataIFC(db, beams);

		// Act
		await setter.ProcessAsync(CancellationToken.None);

		List<IfcBuiltElement> actual = setter.Output!.DatabaseIfc.Project.Extract<IfcBuiltElement>();
		string actualStepString = setter.Output!.DatabaseIfc.ToString(FormatIfcSerialization.STEP);

		List<IfcBuiltElement> actualUpdatedBeams = actual
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
	public async Task ProcessAsync_ShouldUpdateDatabaseElements_ExistingProperty()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcBuiltElement> allElements = db.Project.Extract<IfcBuiltElement>();
		List<IfcBuiltElement> beams = allElements
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		StringValueSetter strategy = new();
		strategy.ValueType = ElementStringValueType.Property;
		strategy.PropertyName = "Class";
		string expected = "newClassPropertyContent";
		strategy.Value = expected;
		ValueSetter setter = new ValueSetter(strategy);

		setter.Input = new DataIFC(db, beams);

		// Act
		await setter.ProcessAsync(CancellationToken.None);

		List<IfcBuiltElement> actual = setter.Output!.DatabaseIfc.Project.Extract<IfcBuiltElement>();
		string actualStepString = setter.Output!.DatabaseIfc.ToString(FormatIfcSerialization.STEP);

		List<IfcBuiltElement> actualUpdatedBeams = actual
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		List<IfcBuiltElement> actualAllTheRest = actual
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
			Assert.NotEqual(expected, elementProperty!.NominalValue.ValueString);
		});
	}

	[Fact]
	public async Task ProcessAsync_ShouldUpdateDatabaseElements_NewProperty()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcBuiltElement> allElements = db.Project.Extract<IfcBuiltElement>();
		List<IfcBuiltElement> beams = allElements
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
		var dbSerializer = new DbSerializer(IfcFormatOutput.STEP);
		dbSerializer.SubscribeToOutput(setter);

		// Act
		await setter.ProcessAsync(CancellationToken.None);
		string actual = dbSerializer.Output!;

		List<IfcBuiltElement> actualElements = setter.Output!.DatabaseIfc.Project.Extract<IfcBuiltElement>();

		List<IfcBuiltElement> actualUpdatedBeams = actualElements 
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		List<IfcBuiltElement> actualAllTheRest = actualElements 
			.Except(actualUpdatedBeams)
			.ToList();

		// Assert
		Assert.Contains(expected, actual);
		Assert.Contains(expectedPropertyName, actual);

		Assert.All(actualUpdatedBeams, ifcElement =>
		{
			var elementProperty = ifcElement.FindProperty(strategy.PropertyName) as IfcPropertySingleValue;
			Assert.Equal(expected, elementProperty!.NominalValue.ValueString);
		});
		Assert.All(actualAllTheRest, ifcElement =>
		{
			Assert.Null((IfcPropertySingleValue)ifcElement.FindProperty(expectedPropertyName));
		});
	}
}
