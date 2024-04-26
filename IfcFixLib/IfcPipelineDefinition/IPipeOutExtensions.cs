namespace IfcFixLib.IfcPipelineDefinition;
public static class IPipeOutExtensions
{
    public static IPipeOut PipeInto(this IPipeOut prevFilter, IPipeFilter nextFilter)
    {
        var connector = new PipeConnector(nextFilter);
        connector.SetUpConnetion(prevFilter);
        return nextFilter;
    }
    public static IPipeFilterIn PipeInto(this IPipeOut prevFilter, IPipeFilterIn nextFilter)
    {
        nextFilter.Input = prevFilter.Output;
        prevFilter.ProcessDone += (sender, ct) =>  nextFilter.StartProcess(ct);
        return nextFilter;
    }
}
