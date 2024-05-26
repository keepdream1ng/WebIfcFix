using IfcFixLib;
using IfcFixLib.IfcPipelineDefinition;
using WebIfcFix.Shared;

namespace WebIfcFix.Services;

public class PipelineManagerService
{
    public DbParser DbParser { get; private set; }
    public DbSerializer DbSerializer { get; private set; }
    public PipelineManager PipelineManager { get; private set; }
    public List<SerializableModelBase> ComponentsLayout { get; private set; } = new();
    public PipelineManagerService()
    {
        DbParser = new DbParser();
        DbSerializer = new DbSerializer();
        PipelineManager = new PipelineManager(DbParser);
    }

    public void ReorderLayout((int oldIndex, int newIndex) indexes)
    {
		var itemToMove = ComponentsLayout[indexes.oldIndex];
		ComponentsLayout.RemoveAt(indexes.oldIndex);
        PipelineManager.Remove(itemToMove.PipelineNode!);

		if (indexes.newIndex < ComponentsLayout.Count)
		{
            var nextComponent = ComponentsLayout[indexes.newIndex + 1];
            itemToMove.PipelineNode = PipelineManager.AddToPipeline(itemToMove.PipeFilter, beforeNode: nextComponent.PipelineNode);
			ComponentsLayout.Insert(indexes.newIndex, itemToMove);
		}
		else
		{
            itemToMove.PipelineNode = PipelineManager.AddToPipeline(itemToMove.PipeFilter);
			ComponentsLayout.Add(itemToMove);
		}
    }
}
