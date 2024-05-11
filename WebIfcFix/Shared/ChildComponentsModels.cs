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
public class ChildComponentModelConverter : JsonConverter<ChildComponentModelBase>
{
    public override bool CanRead => true;
    public override bool CanWrite => false;

    public override ChildComponentModelBase ReadJson(JsonReader reader, Type objectType, ChildComponentModelBase existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        string? modelType = jsonObject.GetValue(nameof(ChildComponentModelBase.ModelDerivedType))?.ToString();
        if (modelType is null)
        {
            throw new JsonSerializationException($"Could not deserialize");
        }
        
        // Determine the concrete type to create based on the ModelDerivedType property
        ChildComponentModelBase? component;
        var type = Type.GetType(modelType);
        if (type is null)
        {
            throw new JsonSerializationException($"Unknown component type: {modelType}");
        }

        component = Activator.CreateInstance( type! ) as ChildComponentModelBase;
        if (component is null)
        {
            throw new JsonSerializationException($"Error on casting to {nameof(ChildComponentModelBase)}");
        }

        // Populate the properties of the concrete type
        serializer.Populate(jsonObject.CreateReader(), component);
        return component;
    }

    public override void WriteJson(JsonWriter writer, ChildComponentModelBase value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
