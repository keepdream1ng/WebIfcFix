using IfcFixLib;
using IfcFixLib.IfcPipelineDefinition;
using WebIfcFix.Shared;

namespace WebIfcFix.Services;

public class LayoutManagerService
{
    public DbParser DbParser { get; private set; } = new();
    public DbSerializer DbSerializer { get; private set; } = new();
    public PipelineManager? PipelineManager { get; private set; }
    public List<SerializableModelBase> ComponentsLayout { get; private set; } = new();

    public void ReorderLayout((int oldIndex, int newIndex) indexes)
    {
		var itemToMove = ComponentsLayout[indexes.oldIndex];
		ComponentsLayout.RemoveAt(indexes.oldIndex);
        PipelineManager!.Remove(itemToMove.PipelineNode!);

		if (indexes.newIndex < ComponentsLayout.Count)
		{
            var nextComponent = ComponentsLayout[indexes.newIndex + 1];
            itemToMove.PipelineNode = PipelineManager.
                AddToPipeline(itemToMove.PipeFilter, beforeNode: nextComponent.PipelineNode);
			ComponentsLayout.Insert(indexes.newIndex, itemToMove);
		}
		else
		{
            DbSerializer.UnsubscribeFrom(PipelineManager.PipeEnd);
            itemToMove.PipelineNode = PipelineManager.AddToPipeline(itemToMove.PipeFilter);
            DbSerializer.SubscribeToOutput(PipelineManager.PipeEnd);
			ComponentsLayout.Add(itemToMove);
		}
    }

    public void ImportLayout(List<SerializableModelBase> components)
    {
        if (PipelineManager is not null)
        {
            PipelineManager.StopProcessing();
            DbSerializer.UnsubscribeFrom(PipelineManager.PipeEnd);
			PipelineManager.Dispose();
        }
        ComponentsLayout = components;
        PipelineManager = new PipelineManager(DbParser);
        ComponentsLayout.ForEach(c => PipelineManager.AddToPipeline(c.PipeFilter));
        DbSerializer.SubscribeToOutput(PipelineManager.PipeEnd);
    }
}
