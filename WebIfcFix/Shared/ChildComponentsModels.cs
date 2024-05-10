using IfcFixLib;

namespace WebIfcFix.Shared;

public interface IChildComponentModelBase
{
    string? ComponentTypeNane { get;}
    Dictionary<string, object> Params => new() { ["Model"] = this };
}
public abstract class ChildComponentModelBase
{
    public virtual string? ComponentTypeNane { get;}
    public virtual Dictionary<string, object> Params { get; private set; } 
}
public class ChildComponent1Model : ChildComponentModelBase, IChildComponentModelBase
{
    public string Input { get; set; } = String.Empty;
    public override string? ComponentTypeNane => typeof(ChildComponent1).FullName;
    public override Dictionary<string, object> Params => new() { ["Model"] = this };
}
public class ChildComponent2Model : ChildComponentModelBase, IChildComponentModelBase
{
    public ElementStringValueType StringValueType { get; set; } = ElementStringValueType.Name;
    public override string? ComponentTypeNane => typeof(ChildComponent2).FullName;
    public override Dictionary<string, object> Params => new() { ["Model"] = this };
}
