using IfcFixLib.IfcPipelineDefinition;
using IfcFixLib.PipelineFilters;
using WebIfcFix.Shared;

namespace WebIfcFix.Filters;

public class ElementsFilterResetterModel : ComponentModel<ElementsFilterResetter>
{
	public override IPipeFilter PipeFilter => _filter;
	private readonly FilterResetter _filter = new FilterResetter();

	public override IComponentInformation ComponentInformation { get; init; } = new ElementsFilterResetterInfo();
}

public class ElementsFilterResetterInfo : ComponentInformation<ElementsFilterResetterModel>
{
    public override string FilterName => "Reset elements filter";

    public override string FilterInstructions => "This component will reset elements filter by once again selecting all elements in the current ifc file (database)";
}
