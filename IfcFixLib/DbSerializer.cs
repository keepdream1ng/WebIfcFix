using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib;
public class DbSerializer(IfcFormatOutput formatOutput = IfcFormatOutput.STEP) : IPipeFilterIn, IPipeCompletionHandler
{
	public DataIFC? Input { get; set; }
	public IfcFormatOutput FormatOutput { get; set; } = formatOutput;
	public string? Output { get; private set; }
    public event EventHandler<CancellationToken>? OnProcessStart;
    public event EventHandler<CancellationToken>? ProcessDone;
	private EventHandler<CancellationToken>? _subscribtion;

	public async Task<string> SeializeToStringAsync(DatabaseIfc db, CancellationToken ct = default)
	{
		string dbString = await Task<string>.Run(() =>
		{
			ct.ThrowIfCancellationRequested();
			return db.ToString((FormatIfcSerialization)FormatOutput);
		});
		return dbString;
	}

	public Func<CancellationToken, Task> StartProcess =>
        async (cancellationToken) =>
        {
			ArgumentNullException.ThrowIfNull(nameof(Input));
			OnProcessStart?.Invoke(this, cancellationToken);
			Output = await SeializeToStringAsync(Input!.DatabaseIfc, cancellationToken);
			ProcessDone?.Invoke(this, cancellationToken);
        };

	public void SubscribeToOutput(IPipeOut outFilter)
	{
		_subscribtion = (sender, ct) =>
		{
			Input = outFilter.Output;
			this.StartProcess(ct);
		};

		outFilter.ProcessDone += _subscribtion;
	}

	public void UnsubscribeFrom(IPipeOut outFilter)
	{
		outFilter.ProcessDone -= _subscribtion;
		_subscribtion = null;
	}
}
