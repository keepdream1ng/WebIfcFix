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

                List<IfcElementAssembly> assemblies = new();
                if (elements.Any(x => x.Decomposes is not null))
                {
					assemblies = db.Project.Extract<IfcElementAssembly>();
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
                        string assemblyGlobalId = el.Decomposes.RelatingObject.GlobalId;
						IfcElementAssembly assembly = assemblies.Single(x => x.GlobalId.Equals(assemblyGlobalId));
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
