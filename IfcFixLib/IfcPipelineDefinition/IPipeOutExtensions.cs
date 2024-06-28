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
        prevFilter.ProcessDone += async (ct) =>
        {
            nextFilter.Input = prevFilter.Output;
            await nextFilter.ProcessAsync(ct).ConfigureAwait(false);
        };
        return nextFilter;
    }

    public static Task GetCompletionTask(this IPipeCompletionHandler filter)
    {
        var tcs = new TaskCompletionSource<bool>();

        AsyncEventHandler? handler = null;
        handler = (ct) =>
        {
            filter.ProcessDone -= handler;
            tcs.TrySetResult(true);
            return ValueTask.CompletedTask;
        };

        filter.ProcessDone += handler;
        return tcs.Task;
    }
}
