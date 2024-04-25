using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib;
public class DbDuplicator : PipeFilter
{
    public static async Task<DatabaseIfc> DuplicateDbWithElementsAsync(
        DatabaseIfc db,
        List<IfcBuiltElement> elements,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(db);
        ArgumentNullException.ThrowIfNull(elements);
        ArgumentOutOfRangeException.ThrowIfZero(elements.Count);
        return await Task.Run(
            () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                DuplicateOptions options = new DuplicateOptions(db.Tolerance);
                options.DuplicateDownstream = false;
                DatabaseIfc newDb = new DatabaseIfc(db);
                newDb.Factory.Duplicate(db.Project, options);
                options.DuplicateDownstream = true;

                foreach (IfcBuiltElement el in elements)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    newDb.Factory.Duplicate(el, options);
                }
                return newDb;
            }
        );
    }
    protected override Func<DataIFC, CancellationToken, Task<DataIFC>> ProcessData =>
        async (data, cancellationToken) =>
        {
            DatabaseIfc newDb = await DuplicateDbWithElementsAsync(data.DatabaseIfc, data.Elements, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return new DataIFC(newDb, newDb.Project.Extract<IfcBuiltElement>());
        };
}
