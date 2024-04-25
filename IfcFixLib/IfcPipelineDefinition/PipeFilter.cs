namespace IfcFixLib.IfcPipelineDefinition;
public abstract class PipeFilter : IPipeFilter
{
    public DataIFC? Input { get; set; }
    public DataIFC? Output {  get; protected set; }
    public event EventHandler? ProcessDone;
    public Func<CancellationToken, Task> StartProcess =>
        async (cancellationToken) =>
        {
            if (Input is null) return;
            await this.ProcessInput(cancellationToken).ConfigureAwait(false);
            if (!cancellationToken.IsCancellationRequested)
            {
                OnProcessDone();
            }
        };
    protected abstract Func<CancellationToken, Task> ProcessInput { get; }
    protected virtual void OnProcessDone()
    {
        ProcessDone?.Invoke(this, EventArgs.Empty);
    }
}
