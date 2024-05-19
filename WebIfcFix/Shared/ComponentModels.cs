using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace WebIfcFix.Shared;

public abstract class SerializableModelBase
{
    public virtual string? ModelType { get; }
    public virtual string? ComponentTypeName { get; }
    public virtual Dictionary<string, object> Params() => new() { ["ModelBase"] = this };
}

public abstract class ComponentModel<T> : SerializableModelBase where T : IComponent
{
    public override string? ModelType  => this.GetType().FullName;

    // This property doesnt need to be serialized, json converter vill create derived class based on ModelType property.
    [JsonIgnore]
    public override string? ComponentTypeName => typeof(T).FullName;
}

public abstract class DynamicComponentWithModelBase<Tmodel> : ComponentBase, IComponent where Tmodel : SerializableModelBase
{
    private Tmodel? _model;

	[Parameter]
	[EditorRequired]
	public SerializableModelBase ModelBase
    {
        get => _model!;
        set { _model = value as Tmodel; }
    }

    public virtual Tmodel Model 
    {
        get => _model!;
        set { _model = value; }
    }
}
