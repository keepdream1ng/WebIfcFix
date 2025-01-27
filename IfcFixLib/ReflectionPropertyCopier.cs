using GeometryGym.Ifc;
using System.Reflection;

namespace IfcFixLib;
public class ReflectionPropertyCopier
{
	private Dictionary<string, (Type Type, Dictionary<string, PropertyInfo?> propertyDict)> _propertyCash = new();
	private List<PropertyInfo> _targetPropertiesToCopy = new();

	public ReflectionPropertyCopier(List<PropertyInfo> targetProperties)
	{
		foreach (PropertyInfo property in targetProperties)
		{
			if (property.CanRead && property.CanWrite)
			{
				_targetPropertiesToCopy.Add(property);
			}
		}
	}

	public void CopyProperies(IfcElement sourceElement, IfcElement targetElement)
	{
		if (!_propertyCash.ContainsKey(targetElement.StepClassName))
		{
			_propertyCash[sourceElement.StepClassName] =
				new (BaseClassIfc.GetType(sourceElement.StepClassName), new());
		}

		var currentSourceCash = _propertyCash[sourceElement.StepClassName];

		foreach (PropertyInfo property in _targetPropertiesToCopy)
		{
			if (!currentSourceCash.propertyDict.ContainsKey(property.Name))
			{
				currentSourceCash.propertyDict[property.Name] = currentSourceCash.Type.GetProperty(property.Name);
			}
			var sourceProperty = currentSourceCash.propertyDict[property.Name];
			if (sourceProperty is not null
				&& sourceProperty.CanRead
				&& property.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
			{
				object? value = sourceProperty.GetValue(sourceElement);
				property.SetValue(targetElement, value);
			}
		}
	}
}
