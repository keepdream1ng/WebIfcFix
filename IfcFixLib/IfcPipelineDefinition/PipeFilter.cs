using System.Threading.Tasks;

namespace IfcFixLib.IfcPipelineDefinition;
public abstract class PipeFilter : IPipeFilter
{
    public DataIFC? Input { get; set; }
    public DataIFC? Output {  get; protected set; }
    public event AsyncEventHandler? ProcessDone;
    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
		if (Input is null) return;
		Output = await ProcessDataAsync(Input, cancellationToken).ConfigureAwait(false);
		if (!cancellationToken.IsCancellationRequested)
		{
			await OnProcessDone(cancellationToken).ConfigureAwait(false);
		}
    }
    protected abstract Task<DataIFC> ProcessDataAsync(DataIFC dataIFC, CancellationToken cancellationToken);
    protected virtual ValueTask OnProcessDone(CancellationToken ct)
    {
        if (ProcessDone is not null)
        {
			return ProcessDone.Invoke(ct);
        }
        else
        {
            return ValueTask.CompletedTask;
        }
    }
}
