using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib.PipelineFilters;
public class DummyFilter : PipeFilter
{
	protected override Task<DataIFC> ProcessDataAsync(DataIFC dataIFC, CancellationToken cancellationToken)
	{
		return Task.FromResult(dataIFC);
	}
}
