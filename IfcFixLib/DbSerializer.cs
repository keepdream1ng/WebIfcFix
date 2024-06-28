using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib;
public class DbSerializer(IfcFormatOutput formatOutput = IfcFormatOutput.STEP) : IPipeFilterIn, IPipeCompletionHandler
{
	public DataIFC? Input { get; set; }
	public IfcFormatOutput FormatOutput { get; set; } = formatOutput;
	public string? Output { get; private set; }
    public event AsyncEventHandler? ProcessStart;
    public event AsyncEventHandler? ProcessDone;
	private AsyncEventHandler? _subscribtion;

	public async Task<string> SeializeToStringAsync(DatabaseIfc db, CancellationToken ct = default)
	{
		string dbString = await Task<string>.Run(() =>
		{
			ct.ThrowIfCancellationRequested();
			return db.ToString((FormatIfcSerialization)FormatOutput);
		});
		return dbString;
	}

	public async Task ProcessAsync(CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(nameof(Input));
		if (ProcessStart is not null)
		{
			await ProcessStart.Invoke(cancellationToken)
				.ConfigureAwait(false);
		}
		Output = await SeializeToStringAsync(Input!.DatabaseIfc, cancellationToken)
			.ConfigureAwait(false);
		if (ProcessDone is not null)
		{
			await ProcessDone.Invoke(cancellationToken)
				.ConfigureAwait(false);
		}
	}

	public void SubscribeToOutput(IPipeOut outFilter)
	{
		_subscribtion = async (ct) =>
		{
			Input = outFilter.Output;
			await ProcessAsync(ct)
			.ConfigureAwait(false);
		};

		outFilter.ProcessDone += _subscribtion;
	}

	public void UnsubscribeFrom(IPipeOut outFilter)
	{
		outFilter.ProcessDone -= _subscribtion;
		_subscribtion = null;
	}

}
