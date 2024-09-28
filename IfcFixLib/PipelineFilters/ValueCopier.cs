using GeometryGym.Ifc;
using IfcFixLib.FilterStrategy;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib.PipelineFilters;
public class ValueCopier(StringValueGetter ValueGetter, StringValueSetter ValueSetter) : PipeFilter
{
	protected override Task<DataIFC> ProcessDataAsync(DataIFC dataIFC, CancellationToken cancellationToken)
	{
		Task.Run(() =>
		{
			foreach (IfcBuiltElement element in dataIFC.Elements)
			{
				cancellationToken.ThrowIfCancellationRequested();
				string valueToCopy = ValueGetter.GetValue(element);
				if (ValueSetter.Value != valueToCopy)
				{
					ValueSetter.Value = valueToCopy;
				}
				ValueSetter.SetValue(element);
			}
		},
		cancellationToken);

		return Task.FromResult(dataIFC);
	}
}
