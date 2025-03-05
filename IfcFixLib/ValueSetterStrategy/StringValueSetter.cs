using GeometryGym.Ifc;

namespace IfcFixLib;

public class StringValueSetter : IValueSetterStrategy
{
    public ElementStringValueType ValueType { get; set; }
    public string Value
    {
        get { return _value; }
        set
        {
            _value = value;
        }
    }
    public string? PropertyName
    {
        get { return _propertyName; }
        set
        {
            ClearCache();
            _propertyName = value;
        }
    }

    private string _value = string.Empty;
    private string? _propertyName = null;
    private Dictionary<string, IfcPropertySingleValue> _cashedPropertiesByValue = new();
    private Dictionary<string, IfcPropertySet> _cashedNewPropSetsByOldGuid = new();
    private Dictionary<string, IfcPropertySet> _cashedNewPropSetsByValue = new();

    public void SetValue(IfcElement element)
    {
        switch (ValueType)
        {
            case ElementStringValueType.Name:
                element.Name = Value;
                break;
            case ElementStringValueType.Description:
                element.Description = Value;
                break;
            case ElementStringValueType.Tag:
                element.Tag = Value;
                break;
            case ElementStringValueType.Property:
                SetProperty(element);
                break;
			default:
				System.Reflection.PropertyInfo? prop = element.GetType().GetProperty(nameof(ValueType));
                if (prop is not null && prop.CanWrite)
                {
                    prop.SetValue(element, Value);
                }
                else
                {
					throw new NotImplementedException();
                }
                break;
        }
    }

    private void SetProperty(IfcElement element)
    {
        if (PropertyName is null)
        {
            throw new ArgumentNullException(nameof(PropertyName));
        }
		var prop = element.FindProperty(PropertyName) as IfcPropertySingleValue;
        if (prop is not null)
        {
            var sets = prop.PartOfPset.
                Where(x => x.DefinesOccurrence
                    .Any(occurence => occurence.RelatedObjects
						.Any(obj => obj.GlobalId == element.GlobalId)))
                .ToList();
            foreach (var set in sets)
            {
                if (!_cashedNewPropSetsByOldGuid.TryGetValue(set.GlobalId, out IfcPropertySet? newSet))
                {
					newSet = element.Database.Factory.Duplicate<IfcPropertySet>(set);
					newSet.DefinesOccurrence.Clear();
					if (!_cashedPropertiesByValue.ContainsKey(Value))
					{
						_cashedPropertiesByValue[Value] = new IfcPropertySingleValue(element.Database, PropertyName, Value);
					}
					newSet.AddProperty(_cashedPropertiesByValue[Value]);
                    _cashedNewPropSetsByOldGuid[(set.GlobalId)] = newSet;
                }

                foreach (IfcRelDefinesByProperties? occurrence in set.DefinesOccurrence)
                {
                    occurrence?.RelatedObjects.Remove(element);
                }

				newSet.RelateObjectDefinition(element);
			}
        }
        else
        {
            if (!_cashedNewPropSetsByValue.TryGetValue(Value, out IfcPropertySet? brandNewSet))
            {
                if (!_cashedPropertiesByValue.ContainsKey(Value))
                {
					_cashedPropertiesByValue[Value] = new IfcPropertySingleValue(element.Database, PropertyName, Value);
                }
                brandNewSet = new IfcPropertySet(element.Database, "PSET_" + PropertyName);
                brandNewSet.AddProperty(_cashedPropertiesByValue[Value]);
                _cashedNewPropSetsByValue[Value] = brandNewSet;
            }
            brandNewSet.RelateObjectDefinition(element);
        }
    }

    private void ClearCache()
    {
        _cashedPropertiesByValue.Clear();
        _cashedNewPropSetsByOldGuid.Clear();
        _cashedNewPropSetsByValue.Clear();
    }
}
