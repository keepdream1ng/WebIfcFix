using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib;
public class DbSerializer(IfcFormatOutput formatOutput = IfcFormatOutput.STEP) : IPipeFilterIn, IPipeCompletionHandler
{
	public DataIFC? Input { get; set; }
	public IfcFormatOutput FormatOutput { get; set; } = formatOutput;
	public string? Output { get; private set; }
    public event EventHandler<CancellationToken>? ProcessDone;

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
			Output = await SeializeToStringAsync(Input!.DatabaseIfc, cancellationToken);
			ProcessDone?.Invoke(this, cancellationToken);
        };
}
