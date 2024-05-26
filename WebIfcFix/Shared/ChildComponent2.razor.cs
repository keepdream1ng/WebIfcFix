using IfcFixLib;
using IfcFixLib.IfcPipelineDefinition;

namespace WebIfcFix.Shared;

public class ChildComponent2Model : ComponentModel<ChildComponent2> 
{
    public ElementStringValueType StringValueType { get; set; } = ElementStringValueType.Name;

	public override IPipeFilter PipeFilter => throw new NotImplementedException();
}
