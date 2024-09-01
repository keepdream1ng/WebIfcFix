using GeometryGym.Ifc;
using IfcFixLib.PipelineFilters;

namespace IfcFixLib.Tests;
public class ElementsRemoverTests(TestFileFixture testFile) : IClassFixture<TestFileFixture>
{
	[Fact]
	public async Task ProcessAsync_ShouldRemoveElementsFromDb()
	{
		// Arrange
		string ifcPath = Path.Combine(Directory.GetCurrentDirectory(), "test.ifc");
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcBuiltElement> allElements = db.Project.Extract<IfcBuiltElement>();
		List<IfcBuiltElement> beams = allElements
			.Where(x => x.Name.Contains("beam", StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		ElementsRemover remover = new();
		List<IfcBuiltElement> expected = allElements.Except(beams).ToList();

		remover.Input = new DataIFC(db, beams);

		// Act
		await remover.ProcessAsync(CancellationToken.None);
		List<IfcBuiltElement> actual = remover.Output!.DatabaseIfc.Project.Extract<IfcBuiltElement>();
		string actualStepString = remover.Output!.DatabaseIfc.ToString(FormatIfcSerialization.STEP);

		// Assert
		Assert.All(actual, ifcElement =>
		{
			Assert.Single(expected.Where(x => x.GlobalId == ifcElement.GlobalId));
			Assert.Empty(beams.Where(x => x.GlobalId == ifcElement.GlobalId));
		});
		Assert.Equal(expected.Count, actual.Count);
		Assert.All(beams, beam =>
		{
			Assert.DoesNotContain(beam.GlobalId, actualStepString);
		});
	}

	[Fact]
	public async Task ProcessAsync_ShouldNotDeleteAllAssembliesParts()
	{
		// Arrange
		string ifcPath = Path.Combine(Directory.GetCurrentDirectory(), "test.ifc");
		using Stream stream = new MemoryStream(testFile.TestIfcBytes);
		using StreamReader reader = new StreamReader(stream);
		DatabaseIfc db = new DatabaseIfc(reader);
		List<IfcBuiltElement> allElements = db.Project.Extract<IfcBuiltElement>();
		List<IfcBuiltElement> beamOnLevel0 = allElements
			.Where(x => x.Name.Equals("TestBeam0", StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		IfcBuiltElement beamOnLevel1000 = allElements
			.Single(x => x.Name.Equals("TestBeam1000", StringComparison.InvariantCultureIgnoreCase));

		ElementsRemover remover = new();
		List<IfcBuiltElement> expected = allElements.Except(beamOnLevel0).ToList();

		remover.Input = new DataIFC(db, beamOnLevel0);

		// Act
		await remover.ProcessAsync(CancellationToken.None);
		List<IfcBuiltElement> actual = remover.Output!.DatabaseIfc.Project.Extract<IfcBuiltElement>();
		string actualStepString = remover.Output!.DatabaseIfc.ToString(FormatIfcSerialization.STEP);

		// Assert
		Assert.All(actual, ifcElement =>
		{
			Assert.Single(expected.Where(x => x.GlobalId == ifcElement.GlobalId));
			Assert.Empty(beamOnLevel0.Where(x => x.GlobalId == ifcElement.GlobalId));
		});
		Assert.Equal(expected.Count, actual.Count);
		Assert.All(beamOnLevel0[0].Decomposes.RelatedObjects, part =>
		{
			Assert.DoesNotContain(part.GlobalId, actualStepString);
		});
		Assert.All(beamOnLevel1000.Decomposes.RelatedObjects, part =>
		{
			Assert.Contains(part.GlobalId, actualStepString);
		});
	}
}
