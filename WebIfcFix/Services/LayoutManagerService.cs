using IfcFixLib;
using IfcFixLib.IfcPipelineDefinition;
using WebIfcFix.Shared;

namespace WebIfcFix.Services;

public class LayoutManagerService(IComponentsTypesService componentsTypes)
{
    public DbParser DbParser { get; private set; } = new();
    public DbSerializer DbSerializer { get; private set; } = new();
    public PipelineManager? PipelineManager { get; private set; }
    public IReadOnlyList<SerializableModelBase> ComponentsLayout => _componentsLayout;
    private List<SerializableModelBase> _componentsLayout = new();
    private IComponentsTypesService _typesService = componentsTypes;

    public void ReorderLayout(int oldIndex, int newIndex)
    {
		var itemToMove = _componentsLayout[oldIndex];
        RemoveElementAt(oldIndex);
        InsertAtIndex(itemToMove, newIndex);
    }

    public void ImportLayout(List<SerializableModelBase> components)
    {
        if (PipelineManager is not null)
        {
            PipelineManager.StopProcessing();
            DbSerializer.UnsubscribeFrom(PipelineManager.PipeEnd);
			PipelineManager.Dispose();
        }
        _componentsLayout = components;
        PipelineManager = new PipelineManager(DbParser);
        _componentsLayout.ForEach(c => c.PipelineNode = PipelineManager.AddToPipeline(c.PipeFilter));
        DbSerializer.SubscribeToOutput(PipelineManager.PipeEnd);
    }

    public void RemoveElementAt(int index)
    {
        if (PipelineManager is null || _componentsLayout.Count <= index) return;
        PipelineManager!.StopProcessing();
        ResetFromIndex(index);
		LinkedListNode<IPipeConnector> nodeToRemove = _componentsLayout[index].PipelineNode!;
        PipelineManager!.Remove(nodeToRemove);
        if (index == _componentsLayout.Count - 1)
        {
            DbSerializer.UnsubscribeFrom(nodeToRemove.Value.Filter);
            DbSerializer.SubscribeToOutput(PipelineManager!.PipeEnd);
        }
		_componentsLayout.RemoveAt(index);
    }

    public void InsertNew((int oldIndex, int newIndex) indexes)
    {
		IComponentInformation pickedTypeInfo = _typesService.ComponentsTypes[indexes.oldIndex];
		SerializableModelBase? pickedModel = Activator.CreateInstance(pickedTypeInfo.ModelType) as SerializableModelBase;
        if (pickedModel is null) return;
        InsertAtIndex(pickedModel, indexes.newIndex);
    }

    public void ResetFromIndex(int index)
    {
		LinkedListNode<IPipeConnector>? nodeAtIndex = _componentsLayout[index].PipelineNode!;
        ResetFromNode(nodeAtIndex);
    }

    public void ResetFromNode(LinkedListNode<IPipeConnector> pipelineNode)
    {
        PipelineManager!.ResetFromNode(pipelineNode);
        DbSerializer.Reset();
    }

    private void InsertAtIndex(SerializableModelBase item, int index)
    {
		if (index < _componentsLayout.Count)
		{
            ResetFromIndex(index);
			SerializableModelBase itemToMove = _componentsLayout[index];
            item.PipelineNode = PipelineManager!
                .AddToPipeline(item.PipeFilter, beforeNode: itemToMove.PipelineNode);
			_componentsLayout.Insert(index, item);
		}
		else
		{
			DbSerializer.UnsubscribeFrom(PipelineManager!.PipeEnd);
			item.PipelineNode = PipelineManager.AddToPipeline(item.PipeFilter);
			DbSerializer.SubscribeToOutput(PipelineManager.PipeEnd);
            DbSerializer.Reset();
			_componentsLayout.Add(item);
		}
    }
}
