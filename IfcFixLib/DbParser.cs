using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib;
public class DbParser : IPipeOut
{
    public event EventHandler? ProcessDone;
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
        var elements = db.Project.Extract<IfcBuiltElement>();
        Output = new DataIFC(db, elements);
        OnProcessDone();
        return db;
    }

    protected virtual void OnProcessDone()
    {
        ProcessDone?.Invoke(this, EventArgs.Empty);
    }
}
