namespace IfcFixLib.IfcPipelineDefinition;

public interface IPipeConnector
{
    IPipeFilter Filter { get; }
    ProcessStatus Status { get; }

    event EventHandler? StateChanged;
    string StatusDescription { get; }

    void SetUpConnetion(IPipeOut pipeFilterToConnectTo);
    void TearDownCurrentConnetion();
    ValueTask InitiateOwnProcessAsync(CancellationToken ct);
    void Reset();
}