using GeometryGym.Ifc;
using IfcFixLib.FilterStrategy;
using IfcFixLib.IfcPipelineDefinition;
using System.Collections.Concurrent;

namespace IfcFixLib.PipelineFilters;

public class ElementsFilter(IFilterStrategy FilterStrategy) : PipeFilter
{
    public async Task<List<IfcElement>> ProcessAsync(
        List<IfcElement> elements,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ConcurrentBag<IfcElement> filtered = [];
        await Parallel.ForEachAsync(elements, cancellationToken, (el, token) =>
        {
            token.ThrowIfCancellationRequested();
            if (FilterStrategy.IsMatch(el))
            {
                filtered.Add(el);
            }
            return ValueTask.CompletedTask;
        });
        return filtered.ToList();
    }

    protected override async Task<DataIFC> ProcessDataAsync(DataIFC data, CancellationToken cancellationToken)
    {
        List<IfcElement> elements = await ProcessAsync(data.Elements, cancellationToken)
            .ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        return new DataIFC(data.DatabaseIfc, elements);
    }
}

