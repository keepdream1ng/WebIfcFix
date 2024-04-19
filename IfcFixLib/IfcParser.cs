using GeometryGym.Ifc;

namespace IfcFixLib;
public class IfcParser
{
    public event EventHandler? ProcessDone;
    public DatabaseIfc Output { get; private set; }
    public async Task<DatabaseIfc> ParseFromStreamAsync(Stream stream)
    {
        string ifcString;
        using (var reader = new StreamReader(stream))
        {
            ifcString = await reader.ReadToEndAsync();
        }
        Output = DatabaseIfc.ParseString(ifcString);
        OnProcessDone();
        return Output;
    }

    protected virtual void OnProcessDone()
    {
        ProcessDone?.Invoke(this, EventArgs.Empty);
    }
}
