using IfcFixLib.IfcPipelineDefinition;
using IfcFixLib.PipelineFilters;
using System.Text.Json.Serialization;
using WebIfcFix.Shared;

namespace WebIfcFix.Filters;

public class IfcDataDublicatorModel : ComponentModel<IfcDataDublicator>
{
    public override IPipeFilter PipeFilter => _duplicator;
    private DbDuplicator _duplicator = new DbDuplicator();

    public override IComponentInformation ComponentInformation { get; init; } = new IfcDataDublicatorInfo();
}

public class IfcDataDublicatorInfo : ComponentInformation<IfcDataDublicatorModel>
{
    public override string FilterName => "Put in the new model";

    public override string FilterInstructions => "This component will copy filtered elements to the new ifc model (database). Next filter component will be dealing with this model instead.";
}
