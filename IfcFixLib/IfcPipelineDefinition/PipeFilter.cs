namespace IfcFixLib.IfcPipelineDefinition;
public abstract class PipeFilter : IPipeFilter
{
    public DataIFC? Input { get; set; }
    public DataIFC? Output {  get; protected set; }
    public event EventHandler? ProcessDone;
    public Func<Task> StartProcess =>
        async () =>
        {
            if (Input is null) return;
            await this.ProcessData().ConfigureAwait(false);
            OnProcessDone();
        };
    protected abstract Func<Task> ProcessData { get; }
    protected virtual void OnProcessDone()
    {
        ProcessDone?.Invoke(this, EventArgs.Empty);
    }
}
