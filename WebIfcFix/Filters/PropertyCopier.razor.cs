using IfcFixLib;
using IfcFixLib.FilterStrategy;
using IfcFixLib.IfcPipelineDefinition;
using IfcFixLib.PipelineFilters;
using WebIfcFix.Shared;

namespace WebIfcFix.Filters;

public class PropertyCopierModel : ComponentModel<PropertyCopier>
{
	public override IPipeFilter PipeFilter => _valueCopier;
	public StringValueSetter ValueSetterStrategy { get; set; }
	public StringValueGetter ValueGetterStrategy { get; set; }

	public override IComponentInformation ComponentInformation { get; init;} = new PropertyCopierInfo();

	private ValueCopier _valueCopier;

    public PropertyCopierModel()
    {
		ValueGetterStrategy = new StringValueGetter();
		ValueSetterStrategy = new StringValueSetter(); 
		_valueCopier = new ValueCopier(ValueGetterStrategy, ValueSetterStrategy);
    }
}

public class PropertyCopierInfo : ComponentInformation<PropertyCopierModel>
{
	public override string FilterName => "Copy property";

	public override string FilterInstructions => "Copy property value and assign it to another property inside of the element.";
}
