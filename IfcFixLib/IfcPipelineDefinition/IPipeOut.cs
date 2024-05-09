namespace IfcFixLib.IfcPipelineDefinition;
public interface IPipeOut : IPipeCompletionHandler
{
    DataIFC? Output { get; }
}
