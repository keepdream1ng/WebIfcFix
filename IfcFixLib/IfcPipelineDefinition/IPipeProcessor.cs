namespace IfcFixLib.IfcPipelineDefinition;
public interface IPipeProcessor
{
    public Func<CancellationToken, Task> StartProcess { get; }
}
