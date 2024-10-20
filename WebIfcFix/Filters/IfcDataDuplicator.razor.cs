using IfcFixLib.IfcPipelineDefinition;
using IfcFixLib.PipelineFilters;
using System.Text.Json.Serialization;
using WebIfcFix.Shared;

namespace WebIfcFix.Filters;

public class IfcDataDuplicatorModel : ComponentModel<IfcDataDuplicator>
{
    public override IPipeFilter PipeFilter => _duplicator;
    public DbDublicatorOptions DbDublicatorOptions { get; set; }
    private DbDuplicator _duplicator;
    public IfcDataDuplicatorModel()
    {
        DbDublicatorOptions = new();
        _duplicator = new DbDuplicator() { Options = DbDublicatorOptions };
    }

    public override IComponentInformation ComponentInformation { get; init; } = new IfcDataDuplicatorInfo();
}

public class IfcDataDuplicatorInfo : ComponentInformation<IfcDataDuplicatorModel>
{
    public override string FilterName => "Put in the new model";

    public override string FilterInstructions => "This component will copy filtered elements to the new ifc model (database). Next filter component will be dealing with this model instead.";
}
