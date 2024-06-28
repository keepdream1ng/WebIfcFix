namespace IfcFixLib.IfcPipelineDefinition;
public interface IPipeProcessor
{
    Task ProcessAsync(CancellationToken cancellationToken);
}
