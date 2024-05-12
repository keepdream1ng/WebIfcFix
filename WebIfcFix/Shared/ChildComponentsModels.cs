using IfcFixLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebIfcFix.Shared;

public interface IChildComponentModelBase
{
    string? ComponentTypeNane { get;}
    Dictionary<string, object> Params => new() { ["Model"] = this };
}
public class ChildComponentModelBase
{
    public virtual string? ComponentTypeNane { get; }
    public virtual string? ModelDerivedType { get; set; }
    public virtual Dictionary<string, object> Params() => new() { ["Model"] = this };
}

public class ChildComponentModelDerived : ChildComponentModelBase
{
    public override string? ModelDerivedType  => this.GetType().FullName;
}

public class ChildComponent1Model : ChildComponentModelDerived 
{
    public string Input { get; set; } = String.Empty;

    [JsonIgnore]
    public override string? ComponentTypeNane => typeof(ChildComponent1).FullName;
}
public class ChildComponent2Model : ChildComponentModelDerived 
{
    public ElementStringValueType StringValueType { get; set; } = ElementStringValueType.Name;

    [JsonIgnore]
    public override string? ComponentTypeNane => typeof(ChildComponent2).FullName;
}
