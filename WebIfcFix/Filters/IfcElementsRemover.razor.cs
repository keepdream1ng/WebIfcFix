using IfcFixLib.IfcPipelineDefinition;
using IfcFixLib.PipelineFilters;
using WebIfcFix.Shared;

namespace WebIfcFix.Filters;

public class IfcElementsRemoverModel : ComponentModel<IfcElementsRemover>
{
	public override IPipeFilter PipeFilter => _remover;
    private ElementsRemover _remover = new ElementsRemover();

	public override IComponentInformation ComponentInformation { get; init; } = new IfcElementsRemoverInfo();
}

public class IfcElementsRemoverInfo : ComponentInformation<IfcElementsRemoverModel>
{
    public override string FilterName => "Remove from model";

    public override string FilterInstructions => "This component will remove filtered elements from ifc model (database) by creating new model and copying everything but those elements. Next filter component will be dealing with new model instead and rest of the elements.";
}
