using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib.PipelineFilters;
public class FilterResetter : PipeFilter
{
	public static List<IfcElement> ExtractAllElements(DatabaseIfc db)
	{
		List<IfcElement> elements = db.Project.Extract<IfcElement>();
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
