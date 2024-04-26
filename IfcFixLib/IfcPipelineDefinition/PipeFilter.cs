namespace IfcFixLib.IfcPipelineDefinition;
public abstract class PipeFilter : IPipeFilter
{
    public DataIFC? Input { get; set; }
    public DataIFC? Output {  get; protected set; }
    public event EventHandler<CancellationToken>? ProcessDone;
    public Func<CancellationToken, Task> StartProcess =>
        async (cancellationToken) =>
        {
            if (Input is null) return;
            Output = await this.ProcessData(Input, cancellationToken).ConfigureAwait(false);
            if (!cancellationToken.IsCancellationRequested)
            {
                OnProcessDone(cancellationToken);
            }
        };
    protected abstract Func<DataIFC, CancellationToken, Task<DataIFC>> ProcessData { get; }
    protected virtual void OnProcessDone(CancellationToken ct)
    {
        ProcessDone?.Invoke(this, ct);
    }
}
