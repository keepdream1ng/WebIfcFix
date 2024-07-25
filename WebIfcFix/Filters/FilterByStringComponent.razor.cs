using IfcFixLib.FilterStrategy;
using IfcFixLib.IfcPipelineDefinition;
using IfcFixLib.PipelineFilters;
using WebIfcFix.Shared;

namespace WebIfcFix.Filters;

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
            FilteredString = string.Empty,
            StringChecker = new StringChecker(),
            StringValueGetter = new StringValueGetter()
        };
        _filter = new ElementsFilter(FilterStrategy);
        ComponentInformation = new FilterByStringComponentInfo();
    }
}

public class FilterByStringComponentInfo : ComponentInformation<FilterByStringComponentModel>
{
    public override string FilterName => "Filter by text property";

    public override string FilterInstructions => "Select the property you want to filter elements on, and type in text in the input field. You can use regex to craft really complex filter conditions, but it harder to understand and could lead to multiple checks of the same value.";
}
