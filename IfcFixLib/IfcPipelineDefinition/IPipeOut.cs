namespace IfcFixLib.IfcPipelineDefinition;
public interface IPipeOut
{
    event EventHandler? ProcessDone;
    DataIFC? Output { get; }
}
