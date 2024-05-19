using IfcFixLib;

namespace WebIfcFix.Shared;

public class ChildComponent2Model : ComponentModel<ChildComponent2> 
{
    public ElementStringValueType StringValueType { get; set; } = ElementStringValueType.Name;
}
