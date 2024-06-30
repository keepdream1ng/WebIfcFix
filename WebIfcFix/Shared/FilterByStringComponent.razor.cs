using IfcFixLib;
using IfcFixLib.FilterStrategy;
using IfcFixLib.IfcPipelineDefinition;

namespace WebIfcFix.Shared;

public class FilterByStringComponentModel : ComponentModel<FilterByStringComponent>
{
	public override IPipeFilter PipeFilter => _filter;
	public StringFilterStrategy FilterStrategy { get; set; }
	public override IComponentInformation ComponentInformation { get; init; }
	private ElementsFilter _filter;
    public FilterByStringComponentModel()
    {
		FilterStrategy = new StringFilterStrategy()
		{
			FilteredString = String.Empty,
			StringChecker = new StringChecker(),
			StringValueGetter = new StringValueGetter()
		};
		_filter = new ElementsFilter(FilterStrategy);
		ComponentInformation = new FilterByStringComponentInfo();
    }
}

public class FilterByStringComponentInfo : ComponentInformation<FilterByStringComponentModel>
{
	public override string FilterName => "Text property filter";

	public override string FilterInstructions => "Select the property you want to filter on, and type in text in the input field.";
}
