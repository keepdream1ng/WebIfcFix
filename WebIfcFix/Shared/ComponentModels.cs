using IfcFixLib.IfcPipelineDefinition;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace WebIfcFix.Shared;

public abstract class SerializableModelBase
{
    public virtual string? ModelType { get; }
    public virtual string? ComponentTypeName { get; }
    public virtual Dictionary<string, object> GetParameters() => new()
    {
        [nameof(DynamicComponentWithModelBase<SerializableModelBase>.Model)] = this
    };

    [JsonIgnore]
    public abstract IPipeFilter PipeFilter { get; }

    [JsonIgnore]
    public virtual LinkedListNode<IPipeConnector>? PipelineNode { get; set; }

    [JsonIgnore]
    public abstract IComponentInformation ComponentInformation { get; init; }
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
	[Parameter]
	[EditorRequired]
    public virtual required Tmodel Model { get; set; }

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			Model.PipelineNode!.Value.StateChanged += OnPipelineNodeStateChange;
		}
	}
	protected virtual void OnPipelineNodeStateChange(object? sender, EventArgs eventArgs)
	{
		StateHasChanged();
	}
}

public abstract class ComponentInformation<Tmodel> : IComponentInformation where Tmodel : SerializableModelBase
{
    public virtual Type ModelType => typeof(Tmodel);

    public abstract string FilterName { get; }
    public abstract string FilterInstructions { get; }
}

public interface IComponentInformation
{
    Type ModelType { get; } 
    string FilterName { get; }
    string FilterInstructions { get; }
}
