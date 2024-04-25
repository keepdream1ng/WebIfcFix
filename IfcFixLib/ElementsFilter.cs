using GeometryGym.Ifc;
using IfcFixLib.FilterStrategy;
using IfcFixLib.IfcPipelineDefinition;
using System.Collections.Concurrent;

namespace IfcFixLib;

public class ElementsFilter(IFilterStrategy FilterStrategy) : PipeFilter
{
    public async Task<List<IfcBuiltElement>> ProcessAsync(
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
        return filtered.ToList();
    }
    protected override Func<DataIFC, CancellationToken, Task<DataIFC>> ProcessData =>
        async (data, cancellationToken) =>
        {
            List<IfcBuiltElement> elements = await ProcessAsync(data.Elements, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return new DataIFC(data.DatabaseIfc, elements);
        };
}

