using IfcFixLib;
using IfcFixLib.IfcPipelineDefinition;

namespace WebIfcFix.Shared;

public class ChildComponent2Model : ComponentModel<ChildComponent2> 
{
    public ElementStringValueType StringValueType { get; set; } = ElementStringValueType.Name;

	public override IPipeFilter PipeFilter { get; init; } = new DbDuplicator();

	public override IComponentInformation ComponentInformation { get; init; } = new ChildComponent2Info();
}

public class ChildComponent2Info : ComponentInformation<ChildComponent2Model>
{
	public override string FilterName => "Component 2";

	public override string FilterInstructions => "do stuff";
}
