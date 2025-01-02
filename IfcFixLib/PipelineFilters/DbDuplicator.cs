using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib.PipelineFilters;
public class DbDuplicator : PipeFilter
{
    public DbDublicatorOptions Options { get; set; } = new();
    public static async Task<DatabaseIfc> DuplicateDbWithElementsAsync(
        DatabaseIfc db,
        List<IfcElement> elements,
        bool dublicateOrphanComponents,
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

                foreach (IfcElement el in elements)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (el.Decomposes is null)
                    {
						newDb.Factory.Duplicate(el, options);
                    }
                    else
                    {
                        string assemblyId = el.Decomposes.RelatingObject.GlobalId;
                        if (assembliesDict!.TryGetValue(assemblyId, out IfcElementAssembly? assembly))
                        {
                            if (assembly.IsDecomposedBy.All(a => a.RelatedObjects.All(asseblyItem => elements.Contains(asseblyItem))))
                            {
                                newDb.Factory.Duplicate(assembly, options);
                                assembliesDict.Remove(assemblyId);
                            }
                            else
                            {
                                if (!dublicateOrphanComponents && el is IfcElementComponent)
                                {
                                    continue;
                                }
								newDb.Factory.Duplicate(el, options);
                            }
                        }
                    }
                }

                return newDb;
            }
        );
    }

    protected override async Task<DataIFC> ProcessDataAsync(DataIFC data, CancellationToken cancellationToken)
    {
        DatabaseIfc newDb = await DuplicateDbWithElementsAsync(data.DatabaseIfc, data.Elements, Options.CopyOrphanComponents, cancellationToken)
            .ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        List<IfcElement> elements = newDb.Project.Extract<IfcElement>();
        return new DataIFC(newDb, elements);
    }
}

public class DbDublicatorOptions
{
    public bool CopyOrphanComponents { get; set; } = true;
}
