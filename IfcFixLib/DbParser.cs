using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;
using IfcFixLib.PipelineFilters;

namespace IfcFixLib;
public class DbParser : IPipeOut
{
    public event AsyncEventHandler? ProcessDone;
    public DataIFC? Output { get; private set; }
    public async Task<DatabaseIfc> ParseFromStreamAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        string ifcString;
        using (var reader = new StreamReader(stream))
        {
            ifcString = await reader.ReadToEndAsync(cancellationToken);
        }
        cancellationToken.ThrowIfCancellationRequested();
        DatabaseIfc db = DatabaseIfc.ParseString(ifcString);
        cancellationToken.ThrowIfCancellationRequested();
		List<IfcBuiltElement> elements = FilterResetter.ExtractAllElements(db);
        Output = new DataIFC(db, elements);
        await OnProcessDone(cancellationToken);
        return db;
    }

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
