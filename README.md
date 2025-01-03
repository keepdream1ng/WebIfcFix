# WebIfcFix

- create pipelines for editing IFC files in the browser, without installing any software.
- edit IFC file locally, the site doesn't have backend, all you do on the network is download a web assembly app, and your data is never sent elsewhere.
- share useful pipeline (workflow) with colleagues or the whole community with a link.

Work in progress version is here: https://keepdream1ng.github.io/WebIfcFix/ 

Let's walk-through adding a simple pipeline filter (a component that executes some code over `DataIFC` object and passes updated version to the next component) with custom logic:
1. Add a filter to the library by implementing `PipeFilter` class:
```csharp
using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib.PipelineFilters;
public class NameConcatenator(NameConcatenatorValueWrapper ValueWrapper) : PipeFilter
{
	protected override async Task<DataIFC> ProcessDataAsync(DataIFC dataIFC, CancellationToken ct)
	{
		await Task.Run(() =>
		{
			foreach (var element in dataIFC.Elements)
			{
				ct.ThrowIfCancellationRequested();
				element.Name = String.Concat(element.Name, ValueWrapper.Value);
			}

		},
		ct);

		return dataIFC;
	}
}

public class NameConcatenatorValueWrapper
{
	public string Value { get; set; } = String.Empty;
}
``` 

2. Add component model and info. Component model is the object that implements `ComponentModel` and will be shared with a link. Along with it We also create a `ComponentInformation` object, that holds instructions about the component work and automatically adds component to the tools in UI.
```csharp
using IfcFixLib.IfcPipelineDefinition;
using IfcFixLib.PipelineFilters;
using WebIfcFix.Shared;

namespace WebIfcFix.Filters;

public class NameConcatenatorComponentModel : ComponentModel<NameConcatenatorComponent>
{
	public override IPipeFilter PipeFilter => _concatenator;
	public NameConcatenatorValueWrapper ValueWrapper {  get; set; }
	private NameConcatenator _concatenator;
	public override IComponentInformation ComponentInformation { get; init; }
		= new NameConcatenatorInfo();
	public NameConcatenatorComponentModel()
	{
		ValueWrapper = new NameConcatenatorValueWrapper();
		_concatenator = new NameConcatenator(ValueWrapper);
	}
}

public class NameConcatenatorInfo : ComponentInformation<NameConcatenatorComponentModel>
{
	public override string FilterName => "Add to name";

	public override string FilterInstructions =>
		"Update elements name by concatenating to it the value from user input";
}
``` 

3. Add blazor component with needed user input elements. This one should be in the file `NameConcatenatorComponent.razor` as it specified in the `NameConcatenatorComponentModel` source code before.
```html
@using WebIfcFix.Shared;
@inherits DynamicComponentWithModelBase<NameConcatenatorComponentModel>

<p>
	@Model.ComponentInformation.FilterInstructions
</p>
<div class="row">
	<div class="form-floating col">
		<InputText id="valueToAdd"
				   placeholder="Text to add to Name"
				   class="form-control"
				   @bind-Value=Model.ValueWrapper.Value />
		<label for="valueToAdd" class="p-3">
			Add to name:
		</label>
	</div>
</div>

@code {
}
```

Result - new component is added as an option to the web UI. It works as a part of pipeline for IFC editing and can be shared with a link.
Here is how our component model looks when we share a pipeline made from it alone, notice that only public properties are serialized and may be shared with other users:
```json
[
  {
    "ValueWrapper": {
      "Value": "myExampleInput"
    },
    "ModelType": "WebIfcFix.Filters.NameConcatenatorComponentModel"
  }
]
```