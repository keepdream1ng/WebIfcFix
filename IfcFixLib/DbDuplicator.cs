using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib;
public class DbDuplicator : PipeFilter
{
    public static async Task<DatabaseIfc> DuplicateDbWithElementsAsync(DatabaseIfc db, List<IfcBuiltElement> elements)
    {
        ArgumentNullException.ThrowIfNull(db);
        ArgumentNullException.ThrowIfNull(elements);
        ArgumentOutOfRangeException.ThrowIfZero(elements.Count);
        return await Task.Run(
            () =>
            {
                DuplicateOptions options = new DuplicateOptions(db.Tolerance);
                options.DuplicateDownstream = false;
                DatabaseIfc newDb = new DatabaseIfc(db);
                newDb.Factory.Duplicate(db.Project, options);
                options.DuplicateDownstream = true;

                foreach (IfcBuiltElement el in elements)
                {
                    newDb.Factory.Duplicate(el, options);
                }
                return newDb;
            }
        );
    }
    protected override Func<Task> ProcessData =>
        async () =>
        {
            DatabaseIfc newDb = await DuplicateDbWithElementsAsync(Input!.DatabaseIfc, Input!.Elements);
            Output = new DataIFC(newDb, newDb.Project.Extract<IfcBuiltElement>());
        };
}
