using IfcFixLib;
using IfcFixLib.FilterStrategy;
using IfcFixLib.IfcPipelineDefinition;
using Newtonsoft.Json;

namespace WebIfcFix.Shared;

public class ChildComponent1Model : ComponentModel<ChildComponent1> 
{
    public string Input { get; set; } = String.Empty;

	public override IPipeFilter PipeFilter => _filter;
	private ElementsFilter _filter = new ElementsFilter(
		new StringFilterStrategy()
		{
			FilteredString = String.Empty,
			StringChecker = new StringChecker(),
			StringValueGetter = new StringValueGetter()
		});


	public override IComponentInformation ComponentInformation { get; init; } = new ChildComponent1Info();
}

public class ChildComponent1Info : ComponentInformation<ChildComponent1Model>
{
	public override string FilterName => "Component 1";

	public override string FilterInstructions => "Select in dropdown the property type and write your text criteria in the input";
}
