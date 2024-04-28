namespace IfcFixLib.IfcPipelineDefinition;
public class PipelineManager(IPipeOut pipeStart)
{
    public IPipeOut PipeStart { get; private set; } = pipeStart;
    public IPipeOut PipeEnd => _pipelineElements.Last?.Value.Filter ?? PipeStart;
    private LinkedList<IPipeConnector> _pipelineElements = new();
    private CancellationTokenSource _tokenSouce = new();

    public LinkedListNode<IPipeConnector> AddToPipeline(
        IPipeFilter filter,
        LinkedListNode<IPipeConnector>? afterNode = null)
    {
        var connector = new PipeConnector(filter);
        LinkedListNode<IPipeConnector> node;
        if (afterNode is null)
        {
            connector.SetUpConnetion(PipeEnd);
            node = _pipelineElements.AddLast(connector);
        }
        else
        {
            afterNode.Next?.Value.SetUpConnetion(connector.Filter);
            connector.SetUpConnetion(afterNode.Value.Filter);
            node = _pipelineElements.AddAfter(afterNode, connector);
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

    public void ContinueProcessing()
    {
        IPipeConnector? stoppedConnector = _pipelineElements
            .FirstOrDefault(c => c.Status != ProcessStatus.Done);
        stoppedConnector?.Filter.StartProcess(GetNewCancelToken());
    }

    public void StopProcessing()
    {
        _tokenSouce.Cancel();
    }
}
