using IfcFixLib;
using IfcFixLib.IfcPipelineDefinition;
using WebIfcFix.Shared;

namespace WebIfcFix.Services;

public class LayoutManagerService(IComponentsTypesService componentsTypes)
{
    public DbParser DbParser { get; private set; } = new();
    public DbSerializer DbSerializer { get; private set; } = new();
    public PipelineManager? PipelineManager { get; private set; }
    public IReadOnlyList<SerializableModelBase> ComponentsLayout => _componentsLayout.AsReadOnly();
    private List<SerializableModelBase> _componentsLayout = new();
    private IComponentsTypesService _typesService = componentsTypes;

    public void ReorderLayout((int oldIndex, int newIndex) indexes)
    {
		var itemToMove = _componentsLayout[indexes.oldIndex];
        RemoveElementAt(indexes.oldIndex);
        InsertAtIndex(itemToMove, indexes.newIndex);
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
        PipelineManager!.Remove(_componentsLayout[index].PipelineNode!);
		_componentsLayout.RemoveAt(index);
    }

    public void InsertNew((int oldIndex, int newIndex) indexes)
    {
		IComponentInformation pickedTypeInfo = _typesService.ComponentsTypes[indexes.oldIndex];
		SerializableModelBase? pickedModel = Activator.CreateInstance(pickedTypeInfo.ModelType) as SerializableModelBase;
        if (pickedModel is null) return;
        InsertAtIndex(pickedModel, indexes.newIndex);
    }

    private void InsertAtIndex(SerializableModelBase item, int index)
    {
		if (index < _componentsLayout.Count)
		{
			SerializableModelBase nextComponent = _componentsLayout[index + 1];
            item.PipelineNode = PipelineManager!
                .AddToPipeline(item.PipeFilter, beforeNode: nextComponent.PipelineNode);
			_componentsLayout.Insert(index, item);
		}
		else
		{
			DbSerializer.UnsubscribeFrom(PipelineManager!.PipeEnd);
			item.PipelineNode = PipelineManager.AddToPipeline(item.PipeFilter);
			DbSerializer.SubscribeToOutput(PipelineManager.PipeEnd);
			_componentsLayout.Add(item);
		}
    }
}
