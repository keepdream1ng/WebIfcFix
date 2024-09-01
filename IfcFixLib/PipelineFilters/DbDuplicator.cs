using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib.PipelineFilters;
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

                Dictionary<string, IfcElementAssembly>? assembliesDict = null;
                if (elements.Any(x => x.Decomposes is not null))
                {
                    assembliesDict = db.Project.Extract<IfcElementAssembly>()
						.ToDictionary(x => x.GlobalId);
                }

                foreach (IfcBuiltElement el in elements)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (el.Decomposes is null)
                    {
						newDb.Factory.Duplicate(el, options);
                    }
                    else
                    {
						IfcElementAssembly assembly = assembliesDict![el.Decomposes.RelatingObject.GlobalId];
						newDb.Factory.Duplicate(assembly, options);
                    }
                }
                return newDb;
            }
        );
    }

    protected override async Task<DataIFC> ProcessDataAsync(DataIFC data, CancellationToken cancellationToken)
    {
        DatabaseIfc newDb = await DuplicateDbWithElementsAsync(data.DatabaseIfc, data.Elements, cancellationToken)
            .ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        List<IfcBuiltElement> elements = newDb.Project.Extract<IfcBuiltElement>();
        return new DataIFC(newDb, elements);
    }
}
