namespace IfcFixLib.IfcPipelineDefinition;

public interface IPipeConnector
{
    IPipeFilter Filter { get; }
    ProcessStatus Status { get; }
    string StatusDescription { get; }

    void SetUpConnetion(IPipeOut pipeFilterToConnectTo);
    void TearDownCurrentConnetion();
    void InitiateOwnProcess(object? eventSender, CancellationToken ct);
}