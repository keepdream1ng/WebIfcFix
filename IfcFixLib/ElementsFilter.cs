using GeometryGym.Ifc;
using IfcFixLib.FilterStrategy;
using IfcFixLib.IfcPipelineDefinition;
using System.Collections.Concurrent;

namespace IfcFixLib;

public class ElementsFilter(IFilterStrategy FilterStrategy) : PipeFilter
{
    public async Task<DataIFC> ProcessAsync(DatabaseIfc db, List<IfcBuiltElement> elements)
    {
        ConcurrentBag<IfcBuiltElement> filtered = [];
        await Task.Run(() =>
            Parallel.ForEach(elements, el =>
            {
                if (FilterStrategy.IsMatch(el))
                {
                    filtered.Add(el);
                }
            })
        );
        return new DataIFC(db, filtered.ToList());
    }
    protected override Func<Task> ProcessData =>
        async () =>
        {
            Output = await ProcessAsync(Input!.DatabaseIfc, Input!.Elements);
        };
}

