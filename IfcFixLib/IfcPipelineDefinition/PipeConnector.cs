namespace IfcFixLib.IfcPipelineDefinition;
public class PipeConnector : IPipeConnector
{
    public IPipeFilter Filter { get; private set; }
    public ProcessStatus Status { get; private set; }
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
        Status = ProcessStatus.Waiting;
        Filter.Input = null;
        _previousPipeLink = null;
    }
    public async ValueTask InitiateOwnProcessAsync(CancellationToken ct)
    {
        try
        {
            Filter.Input = _previousPipeLink!.Output;
            Status = ProcessStatus.Processing;
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
