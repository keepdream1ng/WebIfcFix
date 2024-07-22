using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib.PipelineFilters;
public class FilterResetter : PipeFilter
{
	public static List<IfcBuiltElement> ExtractAllElements(DatabaseIfc db)
	{
		List<IfcBuiltElement> elements = db.Project.Extract<IfcBuiltElement>();
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
