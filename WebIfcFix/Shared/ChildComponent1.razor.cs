using IfcFixLib;
using IfcFixLib.FilterStrategy;
using IfcFixLib.IfcPipelineDefinition;
using Newtonsoft.Json;

namespace WebIfcFix.Shared;

public class ChildComponent1Model : ComponentModel<ChildComponent1> 
{
    public string Input { get; set; } = String.Empty;

	public override IPipeFilter PipeFilter => ElementsFilter.Value;

	[JsonIgnore]
	public Lazy<ElementsFilter> ElementsFilter { get; set; } = new Lazy<ElementsFilter>( new ElementsFilter(
		new StringFilterStrategy()
		{
			FilteredString = String.Empty,
			StringChecker = new StringChecker(),
			StringValueGetter = new StringValueGetter()
		}));
}
