namespace IfcFixLib.IfcPipelineDefinition;
public interface IPipeOut
{
    event EventHandler<CancellationToken>? ProcessDone;
    DataIFC? Output { get; }
}
