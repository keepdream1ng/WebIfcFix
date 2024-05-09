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
        prevFilter.ProcessDone += (sender, ct) =>
        {
            nextFilter.Input = prevFilter.Output;
            nextFilter.StartProcess(ct);
        };
        return nextFilter;
    }

    public static Task GetCompletionTask(this IPipeCompletionHandler filter)
    {
        var tcs = new TaskCompletionSource<bool>();

        EventHandler<CancellationToken>? handler = null;
        handler = (sender, ct) =>
        {
            filter.ProcessDone -= handler;
            tcs.TrySetResult(true);
        };

        filter.ProcessDone += handler;
        return tcs.Task;
    }
}
