using IfcFixLib.IfcPipelineDefinition;
using WebIfcFix.Shared;
using IfcFixLib.PipelineFilters;

namespace WebIfcFix.Filters;

public class ChangeClassComponentModel : ComponentModel<ChangeClassComponent>
{
	public override IPipeFilter PipeFilter => classChanger;
	private ClassChanger classChanger;
	public ClassChangerOptions ClassChangerOptions { get; set; }
	public override IComponentInformation ComponentInformation { get ; init; }
	public ChangeClassComponentModel()
	{
		ComponentInformation = new ChangeClassComponentInfo();
		ClassChangerOptions = new ClassChangerOptions();
		classChanger = new ClassChanger(ClassChangerOptions);
	}
}

public class ChangeClassComponentInfo : ComponentInformation<ChangeClassComponentModel>
{
	public override string FilterName => "Change IFC class";

	public override string FilterInstructions => "This component will create copies of the filtered elements with a new class in the new model, providing new model and  copied elements to the next component";
}
