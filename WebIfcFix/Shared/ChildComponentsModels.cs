using IfcFixLib;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebIfcFix.Shared;

public abstract class ChildComponentModelBase
{
    public virtual string? ModelType { get; }
    public virtual string? ComponentTypeName { get; }
    public virtual Dictionary<string, object> Params() => new() { ["Model"] = this };
}

public abstract class ComponentModel<T> : ChildComponentModelBase where T : IComponent
{
    public override string? ModelType  => this.GetType().FullName;

    // This property doesnt need to be serialized, json converter vill create derived class based on ModelType property.
    [JsonIgnore]
    public override string? ComponentTypeName => typeof(T).FullName;
}

public class ChildComponent1Model : ComponentModel<ChildComponent1> 
{
    public string Input { get; set; } = String.Empty;
}
public class ChildComponent2Model : ComponentModel<ChildComponent2> 
{
    public ElementStringValueType StringValueType { get; set; } = ElementStringValueType.Name;
}
