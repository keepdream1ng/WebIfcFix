using IfcFixLib.FilterStrategy;
using IfcFixLib.IfcPipelineDefinition;
using IfcFixLib.PipelineFilters;
using WebIfcFix.Shared;

namespace WebIfcFix.Filters;

public class FilterByClassNameComponentModel : ComponentModel<FilterByClassNameComponent>
{
    public override IPipeFilter PipeFilter => _filter;
    public FilterClassNameStrategy FilterStrategy { get; set; }
    public override IComponentInformation ComponentInformation { get; init; }
    private ElementsFilter _filter;
    public FilterByClassNameComponentModel()
    {
        FilterStrategy = new FilterClassNameStrategy();
        _filter = new ElementsFilter(FilterStrategy);
        ComponentInformation = new FilterByClassNameComponentInfo();
    }
}

public class FilterByClassNameComponentInfo : ComponentInformation<FilterByClassNameComponentModel>
{
    public override string FilterName => "Filter by IFC class";

    public override string FilterInstructions => "Select the IFC class name for the elements you want to filter on and process in the next component.";
}
