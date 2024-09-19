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
            ClearCache();
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
    private IfcPropertySet? _cachedPropertySet;
    private IfcPropertySingleValue? _cachedProperty;

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
                Where(x => x.DefinesOccurrence.Any(occurence => occurence.RelatedObjects.Any(obj => obj.GlobalId == element.GlobalId)))
                .ToList();
            foreach (var set in sets)
            {
                var newSet = element.Database.Factory.Duplicate<IfcPropertySet>(set);
                foreach (IfcRelDefinesByProperties? occurrence in set.DefinesOccurrence)
                {
                    occurrence?.RelatedObjects.Remove(element);
                }

				newSet.DefinesOccurrence.Clear();
                if (_cachedProperty is null)
                {
                    _cachedProperty = new IfcPropertySingleValue(element.Database, PropertyName, Value);
                }
				newSet.AddProperty(_cachedProperty);
				newSet.RelateObjectDefinition(element);
			}
        }
        else
        {
            if (_cachedPropertySet is null)
            {
                if (_cachedProperty is null)
                {
					_cachedProperty = new IfcPropertySingleValue(element.Database, PropertyName, Value);
                }
				_cachedPropertySet = new IfcPropertySet(element.Database, "PSET_" + PropertyName);
				_cachedPropertySet.AddProperty(_cachedProperty);
            }
            _cachedPropertySet.RelateObjectDefinition(element);
        }
    }

    private void ClearCache()
    {
        _cachedProperty = null;
        _cachedPropertySet = null;
    }
}
