namespace IfcFixLib.IfcPipelineDefinition;
public interface IPipeFilter : IPipeIn, IPipeOut, IPipeProcessor
{
}

public interface IPipeFilterIn : IPipeIn, IPipeProcessor
{
}

public interface IPipeFilterOut : IPipeOut, IPipeProcessor
{
}
