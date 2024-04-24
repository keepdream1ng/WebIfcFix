namespace IfcFixLib.IfcPipelineDefinition;
public interface IPipeProcessor
{
    public Func<Task> StartProcess { get; }
}
