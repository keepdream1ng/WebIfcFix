using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib.PipelineFilters;
public class ValueSetter(IValueSetterStrategy ValueSetterStrategy) : PipeFilter
{
	protected override Task<DataIFC> ProcessDataAsync(DataIFC dataIFC, CancellationToken cancellationToken)
	{
		Task.Run(() =>
		{
			foreach (IfcBuiltElement element in dataIFC.Elements)
			{
				cancellationToken.ThrowIfCancellationRequested();
				ValueSetterStrategy.SetValue(element);
			}
		},
		cancellationToken);

		return Task.FromResult(dataIFC);
	}
}
