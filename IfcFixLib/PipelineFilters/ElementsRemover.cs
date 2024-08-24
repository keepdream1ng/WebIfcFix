using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib.PipelineFilters;
public class ElementsRemover : PipeFilter
{
	protected override async Task<DataIFC> ProcessDataAsync(DataIFC dataIFC, CancellationToken cancellationToken)
	{
		List<IfcBuiltElement> allElements = FilterResetter.ExtractAllElements(dataIFC.DatabaseIfc);
		List<IfcBuiltElement> elementsToKeep = allElements
			.Except(dataIFC.Elements)
			.ToList();
		DatabaseIfc newDb = await DbDuplicator.DuplicateDbWithElementsAsync(dataIFC.DatabaseIfc, elementsToKeep, cancellationToken);
		return new DataIFC(newDb, elementsToKeep);
	}
}
