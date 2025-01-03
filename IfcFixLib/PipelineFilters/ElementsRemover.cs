using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib.PipelineFilters;
public class ElementsRemover : PipeFilter
{
	protected override async Task<DataIFC> ProcessDataAsync(DataIFC dataIFC, CancellationToken cancellationToken)
	{
		List<IfcElement> allElements = FilterResetter.ExtractAllElements(dataIFC.DatabaseIfc);
		List<IfcElement> elementsToKeep = allElements
			.Except(dataIFC.Elements)
			.ToList();
		DatabaseIfc newDb = await DbDuplicator.DuplicateDbWithElementsAsync(dataIFC.DatabaseIfc, elementsToKeep, false, cancellationToken);
		return new DataIFC(newDb, elementsToKeep);
	}
}
