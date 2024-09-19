using IfcFixLib;
using IfcFixLib.IfcPipelineDefinition;
using IfcFixLib.PipelineFilters;
using WebIfcFix.Shared;

namespace WebIfcFix.Filters;

public class PropertyEditorModel : ComponentModel<PropertyEditor>
{
	public override IPipeFilter PipeFilter => _valueSetter;
	public StringValueSetter ValueSetterStrategy { get; set; }

	public override IComponentInformation ComponentInformation { get; init;} = new PropertyEditorInfo();

	private ValueSetter _valueSetter;

    public PropertyEditorModel()
    {
		ValueSetterStrategy = new StringValueSetter(); 
		_valueSetter = new ValueSetter(ValueSetterStrategy);
    }
}

public class PropertyEditorInfo : ComponentInformation<PropertyEditorModel>
{
	public override string FilterName => "Edit property";

	public override string FilterInstructions => "Edit properties of the filtered previously elements.";
}
