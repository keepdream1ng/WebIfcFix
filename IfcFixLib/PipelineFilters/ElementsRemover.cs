using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib.PipelineFilters;
public class ElementsRemover (ElementsRemoverOptions Options) : PipeFilter
{
	protected override async Task<DataIFC> ProcessDataAsync(DataIFC dataIFC, CancellationToken cancellationToken)
	{
		List<IfcElement> allElements = FilterResetter.ExtractAllElements(dataIFC.DatabaseIfc);
		List<IfcElement> elementsToRemove = dataIFC.Elements;
		HashSet<string> globalIdsToRemove = elementsToRemove.Select(x => x.GlobalId).ToHashSet();
		if (Options.RemoveWholeAssembly)
		{
			foreach (var el in elementsToRemove)
			{
				if (el is IfcElementAssembly)
				{
					foreach (IfcRelAggregates? connection in el.IsDecomposedBy)
					{
						if (connection is null) continue;
						foreach (var assemblyPart in connection.RelatedObjects)
						{
							globalIdsToRemove.Add(assemblyPart.GlobalId);
						}
					}
				}
				if (el.Decomposes is null) continue;
				globalIdsToRemove.Add(el.Decomposes.RelatingObject.GlobalId);
				foreach (var relatedObject in el.Decomposes.RelatedObjects)
				{
					globalIdsToRemove.Add(relatedObject.GlobalId);
				}
			}
		}
		List<IfcElement> elementsToKeep = allElements
			.Where(x => !globalIdsToRemove.Contains(x.GlobalId))
			.ToList();
		DatabaseIfc newDb = await DbDuplicator.DuplicateDbWithElementsAsync(dataIFC.DatabaseIfc, elementsToKeep, false, cancellationToken);
		return new DataIFC(newDb, elementsToKeep);
	}
}

public class ElementsRemoverOptions
{
	public bool RemoveWholeAssembly { get; set; } = false;
}
