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
            _hasValuePlaceholder = value.Contains(_valuePlaceholder);
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

    private const string _valuePlaceholder = "{VALUE}";
    private bool _hasValuePlaceholder = true;
    private string _value = $"prefix_{_valuePlaceholder}_postfix";
    private string? _propertyName = null;
    private Dictionary<string, string> _cashedReplacedValues = new();
    private Dictionary<string, IfcPropertySingleValue> _cashedPropertiesByValue = new();
    private Dictionary<string, IfcPropertySet> _cashedNewPropSetsByOldGuid = new();
    private Dictionary<string, IfcPropertySet> _cashedNewPropSetsByValue = new();

    public void SetValue(IfcElement element)
    {
        switch (ValueType)
        {
            case ElementStringValueType.Name:
                element.Name = GetResultValue(element.Name);
                break;
            case ElementStringValueType.Description:
                element.Description = GetResultValue(element.Description);
                break;
            case ElementStringValueType.Tag:
                element.Tag = GetResultValue(element.Tag);
                break;
            case ElementStringValueType.Property:
                SetProperty(element);
                break;
			default:
				System.Reflection.PropertyInfo? prop = element.GetType().GetProperty(nameof(ValueType));
                if (prop is not null && prop.CanWrite && prop.PropertyType == typeof(string))
                {
                    string? currentValue = (string?)prop.GetValue(element);
                    if (currentValue is not null)
                    {
						prop.SetValue(element, GetResultValue(currentValue));
                    }
                }
                else
                {
					throw new NotImplementedException();
                }
                break;
        }
    }

    private string GetResultValue(string originalStringValue)
    {
        if (!_hasValuePlaceholder)
        {
            return _value;
        }
        if (!_cashedReplacedValues.TryGetValue(originalStringValue, out string? replacedValue))
        {
            replacedValue = _value.Replace(_valuePlaceholder, originalStringValue);
            _cashedReplacedValues[originalStringValue] = replacedValue;
        }
        return replacedValue;
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
            string valueToSet = GetResultValue(prop.NominalValue.ValueString);
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
					if (!_cashedPropertiesByValue.ContainsKey(valueToSet))
					{
						_cashedPropertiesByValue[valueToSet] = new IfcPropertySingleValue(element.Database, PropertyName, valueToSet);
					}
					newSet.AddProperty(_cashedPropertiesByValue[valueToSet]);
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
            // Not finding the property means there is no placeholder replacing to do and we just use flat Value string.
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
        _cashedReplacedValues.Clear();
    }
}
