using IfcFixLib;
using IfcFixLib.IfcPipelineDefinition;
using System.Text.Json.Serialization;

namespace WebIfcFix.Shared;

public class IfcDataDublicatorModel : ComponentModel<IfcDataDublicator>
{
	public override IPipeFilter PipeFilter { get; init; } = new DbDuplicator();

	public override IComponentInformation ComponentInformation { get ; init; } = new IfcDataDublicatorInfo();
}

public class IfcDataDublicatorInfo : ComponentInformation<IfcDataDublicatorModel>
{
	public override string FilterName => "Ifc Data Dublicator";

	public override string FilterInstructions => "This component will copy filtered elements to the new ifc model (database).";
}
