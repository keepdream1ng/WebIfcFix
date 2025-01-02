using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib.PipelineFilters;
public class FilterResetter : PipeFilter
{
	public static List<IfcElement> ExtractAllElements(DatabaseIfc db)
	{
		List<IfcElement> elements = db.Project.Extract<IfcElement>();
		// Removing assemblies to avoid duplication.
		for (int i = elements.Count - 1; i >= 0; i--)
		{
			if (elements[i] is IfcElementAssembly)
			{
				elements.RemoveAt(i);
			}
		}
		return elements;
	}

	protected override Task<DataIFC> ProcessDataAsync(DataIFC dataIFC, CancellationToken cancellationToken)
	{
		return Task.Run(() =>
		{
			var elements = ExtractAllElements(dataIFC.DatabaseIfc);
			cancellationToken.ThrowIfCancellationRequested();
			return new DataIFC(dataIFC.DatabaseIfc, elements);
		},
		cancellationToken);
	}
}
