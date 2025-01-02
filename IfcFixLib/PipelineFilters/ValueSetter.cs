using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib.PipelineFilters;
public class ValueSetter(IValueSetterStrategy ValueSetterStrategy) : PipeFilter
{
	protected override async Task<DataIFC> ProcessDataAsync(DataIFC dataIFC, CancellationToken cancellationToken)
	{
		await Task.Run(() =>
		{
			foreach (IfcElement element in dataIFC.Elements)
			{
				cancellationToken.ThrowIfCancellationRequested();
				ValueSetterStrategy.SetValue(element);
			}
		},
		cancellationToken);

		return dataIFC;
	}
}
