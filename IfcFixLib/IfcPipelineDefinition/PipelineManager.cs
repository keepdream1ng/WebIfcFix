namespace IfcFixLib.IfcPipelineDefinition;
public class PipelineManager(IPipeOut pipeStart) : IDisposable
{
    public IPipeOut PipeStart { get; private set; } = pipeStart;
    public IPipeOut PipeEnd => _pipelineElements.Last?.Value.Filter ?? PipeStart;
    private LinkedList<IPipeConnector> _pipelineElements = new();
    private CancellationTokenSource _tokenSouce = new();

    public LinkedListNode<IPipeConnector> AddToPipeline(
        IPipeFilter filter,
        LinkedListNode<IPipeConnector>? beforeNode = null)
    {
        var connector = new PipeConnector(filter);
        LinkedListNode<IPipeConnector> node;
        if (beforeNode is null)
        {
            connector.SetUpConnetion(PipeEnd);
            node = _pipelineElements.AddLast(connector);
        }
        else
        {
            beforeNode.Value.SetUpConnetion(connector.Filter);
            IPipeOut connectTo = beforeNode.Previous?.Value.Filter ?? PipeStart;
            connector.SetUpConnetion(connectTo);
            node = _pipelineElements.AddBefore(beforeNode, connector);
        }
        return node;
    }

    public void Remove(LinkedListNode<IPipeConnector> node)
    {
        if (node.Next is not null)
        {
            IPipeOut previousPipeElem = node.Previous?.Value.Filter ?? PipeStart;
            node.Next.Value.SetUpConnetion(previousPipeElem);
        }
        node.Value.TearDownCurrentConnetion();
        _pipelineElements.Remove(node);
    }

    public CancellationToken GetNewCancelToken()
    {
        using (var oldSource = _tokenSouce)
        {
            _tokenSouce = new CancellationTokenSource();
            oldSource.Dispose();
        }
        return _tokenSouce.Token;
    }

    public async Task ContinueProcessingAsync()
    {
        IPipeConnector? stoppedConnector = _pipelineElements
            .FirstOrDefault(c => c.Status != ProcessStatus.Done);
        if (stoppedConnector is not null)
        {

			await stoppedConnector.InitiateOwnProcessAsync(GetNewCancelToken())
                .ConfigureAwait(false);
        }
    }

    public void StopProcessing()
    {
        _tokenSouce.Cancel();
    }
    public void ResetFromNode(LinkedListNode<IPipeConnector> node)
    {
        LinkedListNode<IPipeConnector>? currentNode = node;
        while(currentNode is not null)
        {
            currentNode.Value.Reset();
            currentNode = currentNode.Next;
        }
    }

	public void Dispose()
	{
        StopProcessing();
        foreach(var node in _pipelineElements)
        {
            node.TearDownCurrentConnetion();
        }
        _tokenSouce?.Dispose();
        _pipelineElements.Clear();
	}
}
