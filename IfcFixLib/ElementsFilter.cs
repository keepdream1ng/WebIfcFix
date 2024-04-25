using GeometryGym.Ifc;
using IfcFixLib.FilterStrategy;
using IfcFixLib.IfcPipelineDefinition;
using System.Collections.Concurrent;

namespace IfcFixLib;

public class ElementsFilter(IFilterStrategy FilterStrategy) : PipeFilter
{
    public async Task<DataIFC> ProcessAsync(
        DatabaseIfc db,
        List<IfcBuiltElement> elements,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ConcurrentBag<IfcBuiltElement> filtered = [];
        await Parallel.ForEachAsync(elements, cancellationToken, (el, token) =>
        {
            token.ThrowIfCancellationRequested();
            if (FilterStrategy.IsMatch(el))
            {
                filtered.Add(el);
            }
            return ValueTask.CompletedTask;
        });
        return new DataIFC(db, filtered.ToList());
    }
    protected override Func<CancellationToken, Task> ProcessInput =>
        async (cancellationToken) =>
        {
            Output = await ProcessAsync(Input!.DatabaseIfc, Input!.Elements, cancellationToken);
        };
}

