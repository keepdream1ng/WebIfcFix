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
            _propertySet = null;
            _value = value;
        }
    }
    public string? PropertyName
    {
        get { return _propertyName; }
        set
        {
            _propertySet = null;
            _propertyName = value;
        }
    }

    private string _value = string.Empty;
    private string? _propertyName = null;
    private IfcPropertySet? _propertySet;

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
            var sets = prop.PartOfPset;
            if (sets is not null && sets.Count != 0)
            {
                foreach (var set in sets)
                {
                    prop.NominalValue = new IfcLabel(Value);
                    set.AddProperty(prop);
                }
            }
        }
        else
        {
            if (_propertySet is null)
            {
				var newProp = new IfcPropertySingleValue(element.Database, PropertyName, Value);
				_propertySet = new IfcPropertySet(element.Database, "PSET" + PropertyName);
				_propertySet.AddProperty(newProp);
            }
            _propertySet.RelateObjectDefinition(element);
        }
    }
}
