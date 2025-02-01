using GeometryGym.Ifc;
using IfcFixLib.PipelineFilters;

namespace IfcFixLib.Tests;
public class ElementsRemoverTests(TestFileFixture testFile) : IClassFixture<TestFileFixture>
{
	[Fact]
	public async Task ProcessAsync_ShouldRemoveElementsFromDb()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcElement> allElements = FilterResetter.ExtractAllElements(db);
		List<IfcElement> beams = allElements
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		ElementsRemoverOptions options = new();
		ElementsRemover remover = new ElementsRemover(options);
		List<IfcElement> expected = allElements.Except(beams).ToList();

		remover.Input = new DataIFC(db, beams);

		// Act
		await remover.ProcessAsync(CancellationToken.None);
		List<IfcElement> actual = FilterResetter.ExtractAllElements(remover.Output!.DatabaseIfc);
		string actualStepString = remover.Output!.DatabaseIfc.ToString(FormatIfcSerialization.STEP);

		// Assert
		Assert.All(actual, ifcElement =>
		{
			Assert.Single(expected, x => x.GlobalId == ifcElement.GlobalId);
			Assert.DoesNotContain(beams, x => x.GlobalId == ifcElement.GlobalId);
		});
		Assert.Equal(expected.Count, actual.Count);
		Assert.All(beams, beam =>
		{
			Assert.DoesNotContain(beam.GlobalId, actualStepString);
		});
	}

	[Fact]
	public async Task ProcessAsync_ShouldNotDeleteAllAssembliesParts_WhithoutOption()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcElement> allElements = FilterResetter.ExtractAllElements(db);
		List<IfcElement> beamOnLevel0 = allElements
			.Where(x => x is not IfcElementAssembly)
			.Where(x => x.Name.Equals("TestBeam0", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		IfcElement beamOnLevel1000 = allElements
			.Where(x => x is not IfcElementAssembly)
			.Single(x => x.Name.Equals("TestBeam1000", StringComparison.InvariantCultureIgnoreCase));

		ElementsRemoverOptions options = new();
		options.RemoveWholeAssembly = false;
		ElementsRemover remover = new ElementsRemover(options);
		List<IfcElement> expected = allElements.Except(beamOnLevel0).ToList();

		remover.Input = new DataIFC(db, beamOnLevel0);

		// Act
		await remover.ProcessAsync(CancellationToken.None);
		List<IfcElement> actual = FilterResetter.ExtractAllElements(remover.Output!.DatabaseIfc);
		string actualStepString = remover.Output!.DatabaseIfc.ToString(FormatIfcSerialization.STEP);

		// Assert
		Assert.Equal(expected.Count, actual.Count);
		Assert.All(actual, ifcElement =>
		{
			Assert.Single(expected, x => x.GlobalId == ifcElement.GlobalId);
			Assert.DoesNotContain(beamOnLevel0, x => x.GlobalId == ifcElement.GlobalId);
		});
		Assert.All(beamOnLevel1000.Decomposes.RelatedObjects, part =>
		{
			Assert.Contains(part.GlobalId, actualStepString);
		});
	}

	[Fact]
	public async Task ProcessAsync_ShouldDeleteAllSelectedAssembliesParts_IfOptionProvided()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcElement> allElements = FilterResetter.ExtractAllElements(db);
		List<IfcElement> beamElements = allElements
			.Where(x => x is IfcBeam)
			.ToList();

		ElementsRemoverOptions options = new();
		options.RemoveWholeAssembly = true;
		ElementsRemover remover = new ElementsRemover(options);
		List<IfcElement> expected = allElements
			.Where(x => !x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		remover.Input = new DataIFC(db, beamElements);

		// Act
		await remover.ProcessAsync(CancellationToken.None);
		List<IfcElement> actual = FilterResetter.ExtractAllElements(remover.Output!.DatabaseIfc);
		string actualStepString = remover.Output!.DatabaseIfc.ToString(FormatIfcSerialization.STEP);

		var diff = actual
			.Where(actualElem => !expected.Any(expectedElem => expectedElem.GlobalId == actualElem.GlobalId))
			.ToList();

		// Assert
		Assert.Equal(expected.Count, actual.Count);
		Assert.Empty(diff);
		Assert.All(actual, ifcElement =>
		{
			Assert.Single(expected, x => x.GlobalId == ifcElement.GlobalId);
			Assert.DoesNotContain(beamElements, x => x.GlobalId == ifcElement.GlobalId);
		});
		Assert.All(beamElements, beam =>
		{
			Assert.DoesNotContain(beam.GlobalId, actualStepString);
		});
	}

	[Fact]
	public async Task ProcessAsync_ShouldDeleteAllSelectedAssembliesParts_IfOptionProvided_WithFilteredAssemblies()
	{
		// Arrange
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcElement> allElements = FilterResetter.ExtractAllElements(db);
		List<IfcElement> beamAssemblies = allElements
			.Where(x => x is IfcElementAssembly)
			.Where(x => x.Name.Contains("beam", StringComparison.CurrentCultureIgnoreCase))
			.ToList();

		ElementsRemoverOptions options = new();
		options.RemoveWholeAssembly = true;
		ElementsRemover remover = new ElementsRemover(options);
		List<IfcElement> expected = allElements
			.Where(x => !x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.Where(x => x.Decomposes is null ||
				!beamAssemblies.Any(assembly => assembly.Guid == x.Decomposes.RelatingObject.Guid))
			.ToList();

		remover.Input = new DataIFC(db, beamAssemblies);

		// Act
		await remover.ProcessAsync(CancellationToken.None);
		List<IfcElement> actual = FilterResetter.ExtractAllElements(remover.Output!.DatabaseIfc);
		string actualStepString = remover.Output!.DatabaseIfc.ToString(FormatIfcSerialization.STEP);

		var diff = actual
			.Where(actualElem => !expected.Any(expectedElem => expectedElem.GlobalId == actualElem.GlobalId))
			.ToList();

		// Assert
		Assert.Equal(expected.Count, actual.Count);
		Assert.Empty(diff);
		Assert.All(actual, ifcElement =>
		{
			Assert.Single(expected, x => x.GlobalId == ifcElement.GlobalId);
			Assert.DoesNotContain(beamAssemblies, x => x.GlobalId == ifcElement.GlobalId);
		});
		Assert.All(beamAssemblies, beam =>
		{
			Assert.DoesNotContain(beam.GlobalId, actualStepString);
		});
	}
}
