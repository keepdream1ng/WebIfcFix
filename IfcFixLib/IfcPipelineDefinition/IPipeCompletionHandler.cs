namespace IfcFixLib.IfcPipelineDefinition;
public interface IPipeCompletionHandler
{
    event EventHandler<CancellationToken>? ProcessDone;
}
