using IfcFixLib;

namespace WebIfcFix.Shared;

public interface IChildComponentModel
{
    Type ComponentType { get;} 
    Dictionary<string, object> Params => new() { ["Model"] = this };
}
public class ChildComponent1Model : IChildComponentModel
{
    public string Input { get; set; } = String.Empty;
    public Type ComponentType { get; private set; } = typeof(ChildComponent1);
}
public class ChildComponent2Model : IChildComponentModel
{
    public ElementStringValueType StringValueType { get; set; } = ElementStringValueType.Name;
    public Type ComponentType { get; private set; } = typeof(ChildComponent2);
}
