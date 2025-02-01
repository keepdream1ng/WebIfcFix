using GeometryGym.Ifc;
using IfcFixLib.IfcPipelineDefinition;

namespace IfcFixLib.PipelineFilters;
public class DbDuplicator : PipeFilter
{
    public DbDublicatorOptions Options { get; set; } = new();
    public static async Task<DatabaseIfc> DuplicateDbWithElementsAsync(
        DatabaseIfc db,
        List<IfcElement> elements,
        bool copyWholeAssembly,
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

                Dictionary<string, IfcElementAssembly>? assembliesDict = null;
                if (copyWholeAssembly &&
					(elements.Any(x => x.Decomposes is not null) ||
                    elements.Any(x => x is IfcElementAssembly)))
                {
                    assembliesDict = db.Project.Extract<IfcElementAssembly>()
						.ToDictionary(x => x.GlobalId);
                }

                foreach (IfcElement el in elements)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (copyWholeAssembly &&
						(el.Decomposes is not null ||
						el is IfcElementAssembly))
                    {
                        string assemblyId;
                        if (el.Decomposes is not null)
                        {
							assemblyId = el.Decomposes.RelatingObject.GlobalId;
                        }
                        else
                        {
							assemblyId = el.GlobalId;
                        }
                        if (assembliesDict!.TryGetValue(assemblyId, out IfcElementAssembly? assembly))
                        {
							options.DuplicateDownstream = true;
							newDb.Factory.Duplicate(assembly, options);
							options.DuplicateDownstream = false;
							assembliesDict!.Remove(assemblyId);
                        }
                    }
                    else
                    {
                        newDb.Factory.Duplicate(el, options);
                    }
                }

                return newDb;
            }
        );
    }

    protected override async Task<DataIFC> ProcessDataAsync(DataIFC data, CancellationToken cancellationToken)
    {
        DatabaseIfc newDb = await DuplicateDbWithElementsAsync(data.DatabaseIfc, data.Elements, Options.CopyWholeAssembly, cancellationToken)
            .ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        List<IfcElement> elements = FilterResetter.ExtractAllElements(newDb);
        return new DataIFC(newDb, elements);
    }
}

public class DbDublicatorOptions
{
    public bool CopyWholeAssembly { get; set; } = false;
}
