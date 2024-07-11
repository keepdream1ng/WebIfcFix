namespace IfcFixLib.IfcPipelineDefinition;
public class PipeConnector : IPipeConnector
{
    public IPipeFilter Filter { get; private set; }
    public ProcessStatus Status { get; private set; }
    public event EventHandler? StateChanged;
    public string StatusDescription => GetStatusDescription();
    private IPipeOut? _previousPipeLink;
    private string _errorMessage = string.Empty;

    public PipeConnector(IPipeFilter pipeFilter)
    {
        Filter = pipeFilter;
        Filter.ProcessDone += OnOwnPocessDone;
        Status = ProcessStatus.Waiting;
    }
    public void SetUpConnetion(IPipeOut pipeFilterToConnectTo)
    {
        TearDownCurrentConnetion();
        _previousPipeLink = pipeFilterToConnectTo;
        _previousPipeLink.ProcessDone += InitiateOwnProcessAsync;
    }
    public void TearDownCurrentConnetion()
    {
        if (_previousPipeLink is null) return;
        _previousPipeLink.ProcessDone -= InitiateOwnProcessAsync;
        _previousPipeLink = null;
        Reset();
    }
    public void Reset()
    {
        Filter.Input = null;
        Status = ProcessStatus.Waiting;
		StateChanged?.Invoke(this, EventArgs.Empty);
    }
    public async ValueTask InitiateOwnProcessAsync(CancellationToken ct)
    {
        try
        {
            Filter.Input = _previousPipeLink!.Output;
            Status = ProcessStatus.Processing;
            StateChanged?.Invoke(this, EventArgs.Empty);
            await Filter.ProcessAsync(ct);
        }
        catch (OperationCanceledException)
        {
            Status = ProcessStatus.Cancelled;
        }
        catch (Exception ex)
        {
            Status = ProcessStatus.Error;
            _errorMessage = ex.Message;
        }
        finally
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private string GetStatusDescription()
    {
        string description = Status switch
        {
            ProcessStatus.Error => _errorMessage,
            _ => Status.ToString(),
        };
        return description;
    }

    private ValueTask OnOwnPocessDone(CancellationToken ct)
    {
        Status = ProcessStatus.Done;
        return ValueTask.CompletedTask;
    }
}
