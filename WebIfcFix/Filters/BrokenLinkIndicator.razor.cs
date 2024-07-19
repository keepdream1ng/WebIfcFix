using IfcFixLib.IfcPipelineDefinition;
using IfcFixLib.PipelineFilters;
using WebIfcFix.Shared;

namespace WebIfcFix.Filters;

public class BrokenLinkIndicatorModel : ComponentModel<BrokenLinkIndicator>
{
	public override IPipeFilter PipeFilter => _dummyFilter;
	private DummyFilter _dummyFilter = new();

	public override IComponentInformation ComponentInformation { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
}
