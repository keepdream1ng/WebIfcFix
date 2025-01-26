using IfcFixLib.IfcPipelineDefinition;
using GeometryGym.Ifc;
using System.Reflection;

namespace IfcFixLib.PipelineFilters;
public class ClassChanger(ClassChangerOptions options) : PipeFilter
{
	protected override async Task<DataIFC> ProcessDataAsync(DataIFC dataIFC, CancellationToken cancellationToken)
	{
		Type ifcTypeTarget = BaseClassIfc.GetType(options.TargetClassName);
		if (ifcTypeTarget is null)
		{
			throw new ArgumentException($"Coudnt find the type ${options.TargetClassName}");
		}
		// Reassigning 'Decomposes' removes element from SpatialStructure, we dont want that.
		List<PropertyInfo> propertiesTarget = ifcTypeTarget.GetProperties()
			.Where(x => x.Name != "Decomposes")
			.ToList();
		FactoryIfc factory = dataIFC.DatabaseIfc.Factory;
		List<IfcElement> elementsCopies = new();
		foreach (var el in dataIFC.Elements)
		{
			IfcObjectDefinition host;
			Type ifcTypeSource = BaseClassIfc.GetType(el.StepClassName);
			if (el.ContainedInStructure is not null)
			{
				host = el.ContainedInStructure.RelatingStructure;
			}
			else
			{
				host = dataIFC.DatabaseIfc.Project;
			}
			var copy = factory.ConstructElement(
				options.TargetClassName,
				host,
				el.ObjectPlacement,
				el.Representation);

			foreach (var property in propertiesTarget)
			{
				if (property.CanRead && property.CanWrite)
				{
					var sourceProperty = ifcTypeSource.GetProperty(property.Name);
					if (sourceProperty is not null
						&& sourceProperty.CanRead
						&& property.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
					{
						object? value = sourceProperty.GetValue(el);
						property.SetValue(copy, value);
					}
				}
			}
			if (el.Decomposes is not null)
			{
				el.Decomposes.RelatedObjects.Add(copy);
				el.Decomposes.RelatedObjects.Remove(el);
			}
			foreach (var setDef in el.IsDefinedBy)
			{
				setDef.RelatedObjects.Add(copy);
			}
			el.MaterialProfile(out IfcMaterial material, out _);
			copy.SetMaterial(material);

			elementsCopies.Add(copy);
		}
		List<IfcElement> allElements = FilterResetter.ExtractAllElements(dataIFC.DatabaseIfc);
		List<IfcElement> elementsToRemove = dataIFC.Elements;
		HashSet<string> globalIdsToRemove = elementsToRemove.Select(x => x.GlobalId).ToHashSet();
		List<IfcElement> elementsToKeep = allElements
			.Where(x => !globalIdsToRemove.Contains(x.GlobalId))
			.ToList();
		elementsToKeep.AddRange(elementsCopies);
		DatabaseIfc newDb = await DbDuplicator.DuplicateDbWithElementsAsync(dataIFC.DatabaseIfc, elementsToKeep, false, cancellationToken);
		return new DataIFC(newDb, elementsCopies);
	}
}

public class ClassChangerOptions
{
	public string TargetClassName { get; set; } = string.Empty;
}
